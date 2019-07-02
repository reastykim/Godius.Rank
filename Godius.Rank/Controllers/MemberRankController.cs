using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Godius.Data;
using Godius.Data.Models;
using Godius.RankSite.Helpers;
using Godius.RankSite.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.IISIntegration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;

namespace Godius.RankSite.Controllers
{
    public class MemberRankController : Controller
    {
        private readonly RankContext _context;
        private readonly IMemoryCache _memoryCache;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly string _defaultSubDomain;

        public MemberRankController(RankContext context, IMemoryCache memoryCache, IHostingEnvironment hostingEnvironment,
            IConfiguration configuration)
        {
            _context = context;
            _memoryCache = memoryCache;
            _hostingEnvironment = hostingEnvironment;

            _defaultSubDomain = configuration.GetValue<string>("DefaultSubDomain");
        }

        public async Task<IActionResult> Index(DateTime? rankingDate = null)
        {
            var subDomain = HttpContext.Request.Host.Host.Split('.', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();
            if (User.IsInRole(Roles.Admin))
            {
                subDomain = _defaultSubDomain;
            }
            else if (subDomain == "localhost" || subDomain == "127")
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Role, Roles.Admin),
                };
                var identity = new ClaimsIdentity(claims);
                var principal = new ClaimsPrincipal(identity);
                await HttpContext.SignInAsync(principal);
                return RedirectToAction("Index");
            }
            var guild = await _context.Guilds.Include(G => G.Characters).ThenInclude(C => C.Ranks)
											 .Include(G => G.WeeklyRanks)
											 .FirstOrDefaultAsync(G => G.Alias == subDomain);

            if (guild == null)
            {
                return NotFound();
            }

            rankingDate = GetRankingUpdatedDate(rankingDate);

            ViewData["Date"] = rankingDate;

            var cacheKey = $"{guild.Name}_{rankingDate.Value.ToString("yyyy-MM-dd")}";
            List<WeeklyRank> weeklyRanks;

            // Look for cache key.
            if (!_memoryCache.TryGetValue(cacheKey, out weeklyRanks))
            {
				// Key not in cache, so get data.
				weeklyRanks = guild.WeeklyRanks.ToList();

                // Set cache options.
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    // Keep in cache for this time, reset time if accessed.
                    .SetSlidingExpiration(TimeSpan.FromHours(12));

                // Save data in cache.
                _memoryCache.Set(cacheKey, weeklyRanks, cacheEntryOptions);
            }

            ViewData["WeeklyRanks"] = weeklyRanks;

            return View(guild);
        }

        public async Task<IActionResult> GetAllRanks(Guid characterId, DateTime date)
        {
            var character = await _context.Characters.FirstOrDefaultAsync(C => C.Id == characterId);
            if (character == null)
            {
                return NotFound();
            }

            var position = GuildPositionsToImageConverter.GetPosisionImage(character.GuildPosition.GetValueOrDefault(GuildPositions.Newbie));

            var ranks = from rank in _context.Ranks
                        where rank.CharacterId == characterId && rank.Date <= date
                        orderby rank.Date
                        select new
                        {
                            rank.Date,
                            rank.Ranking
                        };

            return new JsonResult(new
            {
                position,
                character.Name,
                Ranks = await ranks.ToListAsync()
            });
        }

        public async Task<IActionResult> GetAllWeeklyRanks(Guid characterId, DateTime date)
        {
            var character = await _context.Characters.FirstOrDefaultAsync(C => C.Id == characterId);
            if (character == null)
            {
                return NotFound();
            }

            var position = GuildPositionsToImageConverter.GetPosisionImage(character.GuildPosition.GetValueOrDefault(GuildPositions.Newbie));

            var ranks = from weeklyRank in _context.WeeklyRanks
                        where weeklyRank.CharacterId == characterId && weeklyRank.Date <= date
                        orderby weeklyRank.Date
                        select new
                        {
                            weeklyRank.Date,
                            weeklyRank.Ranking
                        };

            return new JsonResult(new
            {
                position,
                character.Name,
                Ranks = await ranks.ToListAsync()
            });
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