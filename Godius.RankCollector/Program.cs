using Godius.Data;
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

            // Validate a Guild Member file
            var characterListFilePath = args[0];
            var characterListFileInfo = new FileInfo(characterListFilePath);
            if (!characterListFileInfo.Exists || !characterListFileInfo.Extension.Equals(".csv", StringComparison.CurrentCultureIgnoreCase))
            {
                Console.WriteLine($"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}] Invalid a Character list file");
                Console.Read();
                return;
            }

            //// Initialize a DB
            //var connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            //var options = SqlServerDbContextOptionsExtensions.UseSqlServer(new DbContextOptionsBuilder<RankContext>(), connectionString).Options;
            //using (var context = new RankContext(options))
            //{
            //    DbInitializer.Initialize(context);
            //    var guildName = characterListFileInfo.Name.Replace(characterListFileInfo.Extension, "");
            //    var guild = CreateGuild(context, guildName);

            //    // Read a Guild Member file
            //    using (var sr = characterListFileInfo.OpenText())
            //    {
            //        var membersInfo = sr.ReadToEnd().Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            //        foreach (var memberInfo in membersInfo)
            //        {
            //            var splitedMemberInfo = memberInfo.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            //            if (splitedMemberInfo.Length != 2)
            //            {
            //                Console.WriteLine($"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}] Invalid a Member Info");
            //                continue;
            //            }

            //            // Gets or Create the Guild and Member to Database
            //            var guildPosition = splitedMemberInfo[0];
            //            var characterName = splitedMemberInfo[1];
            //            var member = CreateCharacter(context, characterName, guild, guildPosition);

            //            // Get a Ranking of Character vis web parsing
            //            var ranking = GetCharacterRanking(member.Name, UsedEncoding);
            //            if (String.IsNullOrWhiteSpace(ranking))
            //            {
            //                Console.WriteLine($"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}] Cannot found a ranking of Member '{member.Name}'");
            //                continue;
            //            }

            //            var rankingDate = GetRankingUpdatedDate();

            //            // Update
            //            UpdateRanking(context, member, ranking, rankingDate);
            //        }
            //    }

            //    // Add Weekly Ranking
            //    AddWeeklyRanking(context, guild);
            //}

            Console.WriteLine($"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}] Finish a getting the ranking of memeber!");
            Console.ReadLine();
        }

        /*
        private static void UpdateWeeklyRanking(RankContext context, Guild guild, DateTime rankingDate)
        {
            AddWeeklyRanking(context, guild, rankingDate);
        }

        private static Guild CreateGuild(RankContext context, string guildName)
        {
            var guild = context.Guilds.Include("Characters").FirstOrDefault(G => G.Name == guildName);
            if (guild == null)
            {
                guild = new Guild { Name = guildName };
                context.Guilds.Add(guild);
                context.SaveChanges();
            }

            return guild;
        }

        private static Character CreateCharacter(RankContext context, string characterName, Guild guild, string guildPositionDisplay)
        {
            var character = context.Characters.Include("Guild").Include("Ranks").FirstOrDefault(C => C.Name == characterName);
            var guildPosition = EnumHelper.ParseDisplayToEnum<GuildPositions>(guildPositionDisplay);

            if (character == null)
            {

                character = new Character { Name = characterName, GuildId = guild.Id, GuildPosition = guildPosition };
                context.Characters.Add(character);
            }

            if (character.GuildId != guild.Id)
            {
                character.GuildId = guild.Id;
            }

            if (character.GuildPosition != guildPosition)
            {
                character.GuildPosition = guildPosition;
            }

            context.SaveChanges();

            return character;
        }

        private static Rank UpdateRanking(RankContext context, Character member, string ranking, DateTime rankingDate)
        {
            var rank = member.Ranks?.FirstOrDefault(R => R.Date.Value == rankingDate);
            if (rank == null)
            {
                rank = new Rank { CharacterId = member.Id, Ranking = Int32.Parse(ranking), Date = rankingDate };
                context.Ranks.Add(rank);
                context.SaveChanges();

                Console.WriteLine($"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}] Updated the Ranking. Name={member.Name}, Rank={ranking}");
            }

            return rank;
        }

        private static string GetCharacterRanking(string characterName, Encoding encoding)
        {
            try
            {
                var request = WebRequest.Create(API_ADDRESS) as HttpWebRequest;
                request.Method = METHOD;
                request.ContentType = CONTENT_TYPE;

                var requestStream = request.GetRequestStream();
                using (var sw = new StreamWriter(requestStream))
                {
                    sw.WriteLine($"{PARAMETER_KEY}={HttpUtility.UrlEncode(characterName, UsedEncoding)}");
                }
                //request.ContentLength = requestStream.Length;

                var response = request.GetResponse() as HttpWebResponse;
                using (var sr = new StreamReader(response.GetResponseStream(), UsedEncoding))
                {
                    var responseText = sr.ReadToEnd();

                    var begin = responseText.IndexOf(PARSE_BEGIN_TEXT) + PARSE_BEGIN_TEXT.Length;
                    var end = responseText.IndexOf(PARSE_END_TEXT, begin);
                    var rankingText = responseText.Substring(begin, end - begin);
                    return rankingText;
                }
            }
            catch (WebException we)
            {

            }
            catch (Exception e)
            {

            }

            return String.Empty;
        }

        private static void AddWeeklyRanking(RankContext context, Guild guild, DateTime? rankingDate = null)
        {
            rankingDate = GetRankingUpdatedDate(rankingDate);

            List<Rank> currentWeekRanks = new List<Rank>();
            foreach (var characterId in guild.Characters.Where(C => C.IsActivated).Select(C => C.Id))
            {
                var character = context.Characters.Include("Guild").Include("Ranks")
                                                  .FirstOrDefault(C => C.Id == characterId);

                var currentWeekRank = character.Ranks.FirstOrDefault(R => R.Date.GetValueOrDefault() == rankingDate);
                if (currentWeekRank != null)
                {
                    currentWeekRanks.Add(currentWeekRank);
                }
            }

            currentWeekRanks = currentWeekRanks.OrderBy(R => R.Ranking).ToList();

            for (int i = 1; i <= currentWeekRanks.Count; i++)
            {
                var rank = currentWeekRanks[i - 1];
                var weeklyRank = new WeeklyRank { CharacterId = rank.CharacterId, GuildId = guild.Id, Ranking = i, Date = rankingDate };
                context.WeeklyRanks.Add(weeklyRank);
                context.SaveChanges();
            }
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
        }*/
    }
}
