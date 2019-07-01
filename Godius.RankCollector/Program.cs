using Godius.Data;
using Godius.Data.Helper;
using Godius.Data.Models;
using Godius.WebCrawler;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Godius.RankCollector
{
    class Program 
    {
        static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("Need a Character list file that type is csv.");
                Console.Read();
                return;
            }

            // Validate a Character List file
            var characterListFilePath = args[0];
            var characterListFileInfo = new FileInfo(characterListFilePath);
            if (!characterListFileInfo.Exists || !characterListFileInfo.Extension.Equals(".csv", StringComparison.CurrentCultureIgnoreCase))
            {
                Console.WriteLine($"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}] Invalid a Character List file");
                Console.Read();
                return;
            }

            var rankingDate = GetRankingUpdatedDate();
			IRankingCrawler rankingCrawler = new RankingCrawlerV2();

			// Initialize a DB
			var connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            var options = SqlServerDbContextOptionsExtensions.UseSqlServer(new DbContextOptionsBuilder<RankContext>(), connectionString).Options;
            using (var context = new RankContext(options))
            {
                DbInitializer.Initialize(context);

                // Read a Character List file
                using (var sr = characterListFileInfo.OpenText())
                {
                    var characterInfos = sr.ReadToEnd().Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var characterInfo in characterInfos)
                    {
                        var splitedCharacterInfos = characterInfo.Split(',');
                        if (splitedCharacterInfos.Length < 3)
                        {
                            Console.WriteLine($"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}] Invalid a Character Info. Input = [{splitedCharacterInfos}]");
                            continue;
                        }

                        var guildName = splitedCharacterInfos[0];
                        var guildPosition = splitedCharacterInfos[1];
                        var characterName = splitedCharacterInfos[2];

                        // Gets or Create a Guild to Database
                        Guild guild = null;
                        if (String.IsNullOrWhiteSpace(guildName) != true)
                        {
                            guild = GetOrCreateGuild(context, guildName);
                        }

                        // Gets or Create a Character to Database
                        var character = GetOrCreateCharacter(context, characterName, guild?.Id, guildPosition);

                        // Get a Ranking of Character vis web parsing
                        var ranking = rankingCrawler.GetCharacterRanking(character.Name);
                        if (String.IsNullOrWhiteSpace(ranking))
                        {
                            Console.WriteLine($"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}] Cannot found a ranking of Member '{character.Name}'");
                            continue;
                        }

                        

                        // Update Character Ranking
                        UpdateCharacterRanking(context, character, ranking, rankingDate);
                    }

                    // Update Guild Weekly Ranking
                    UpdateGuildWeeklyRanking(context, rankingDate);
                }
            }

            Console.WriteLine($"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}] Finish a getting the ranking of character list!");
            Console.ReadLine();
        }

        private static Guild GetOrCreateGuild(RankContext context, string guildName)
        {
            if (String.IsNullOrWhiteSpace(guildName))
                throw new ArgumentException("The guild name is invalid.", nameof(guildName));

            var guild = context.Guilds.FirstOrDefault(G => G.Name == guildName);
            if (guild == null)
            {
                guild = new Guild { Name = guildName };
                context.Guilds.Add(guild);
                context.SaveChanges();
            }

            return guild;
        }

        private static Character GetOrCreateCharacter(RankContext context, string characterName, Guid? guildId, string guildPositionDisplay)
        {
            var character = context.Characters.FirstOrDefault(C => C.Name == characterName);
            GuildPositions? guildPosition = null;
            if (String.IsNullOrWhiteSpace(guildPositionDisplay) != true)
            {
                guildPosition = EnumHelper.ParseDisplayToEnum<GuildPositions>(guildPositionDisplay);
            }

            if (character == null)
            {
                character = new Character { Name = characterName, GuildId = guildId, GuildPosition = guildPosition };
                context.Characters.Add(character);
            }
            else
            {
                if (character.GuildId != guildId)
                {
                    character.GuildId = guildId;
                }
                if (character.GuildPosition != guildPosition)
                {
                    character.GuildPosition = guildPosition;
                }
            }

            context.SaveChanges();

            return character;
        }

        private static Rank UpdateCharacterRanking(RankContext context, Character character, string ranking, DateTime rankingDate)
        {
            var rank = context.Ranks?.FirstOrDefault(R => R.Date.Value == rankingDate && R.CharacterId == character.Id);
            if (rank == null)
            {
                rank = new Rank { CharacterId = character.Id, Ranking = Int32.Parse(ranking), Date = rankingDate };
                context.Ranks.Add(rank);
                context.SaveChanges();

                Console.WriteLine($"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}] Updated the Ranking. Name={character.Name}, Rank={ranking}");
            }

            return rank;
        }

        private static int UpdateGuildWeeklyRanking(RankContext context, DateTime? rankingDate = null)
        {
            rankingDate = GetRankingUpdatedDate(rankingDate);

            foreach (var guild in context.Characters.Include(C => C.Guild)
                                                    .Include(C => C.Ranks)
                                         .Where(C => C.IsActivated && C.GuildId != null)
                                         .GroupBy(C => C.GuildId))
            {
                var currentWeekRanks = guild.Select(C => new { C.Id, C.Name, Rank = C.Ranks?.FirstOrDefault(R => R.Date.GetValueOrDefault() == rankingDate) })
                                            .Where(CWR => CWR.Rank != null)
                                            .OrderBy(CWR => CWR.Rank.Ranking)
                                            .ToList();

                for (int i = 1; i <= currentWeekRanks.Count; i++)
                {
                    var rank = currentWeekRanks[i - 1];
                    if (context.WeeklyRanks.Any(WR => WR.CharacterId == rank.Id && WR.Date == rankingDate))
                        continue;
                    
                    var weeklyRank = new WeeklyRank { CharacterId = rank.Id, GuildId = guild.Key, Ranking = i, Date = rankingDate };
                    context.WeeklyRanks.Add(weeklyRank);
                }
            }

            return context.SaveChanges();
        }

        private static DateTime GetRankingUpdatedDate(DateTime? date = null)
        {
            if (date == null)
            {
                date = DateTime.Now.Date;
            }

            while (date.Value.DayOfWeek != DayOfWeek.Friday)
            {
                date = date.Value.AddDays(-1);
            }

            return date.Value;
        }
    }
}
