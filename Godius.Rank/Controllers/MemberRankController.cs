using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godius.Data;
using Godius.Data.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Godius.RankSite.Controllers
{
    public class MemberRankController : Controller
    {
        private readonly RankContext _context;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IConfiguration _configuration;

        public MemberRankController(RankContext context,
            IHostingEnvironment hostingEnvironment, IConfiguration configuration)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
            _configuration = configuration;
        }

        public async Task<IActionResult> Index(DateTime? rankingDate = null)
        {
            var subDomain = HttpContext.Request.Host.Host.Split('.', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();
            var guildName = String.Empty;
            switch (subDomain?.ToUpper())
            {
                case "GISADAN":
                default:
                    guildName = "기사단";
                    break;
                case "AVENGERS":
                    guildName = "어벤져스";
                    break;
            }

            if (String.IsNullOrWhiteSpace(guildName))
            {
                return NotFound();
            }

            rankingDate = GetRankingUpdatedDate(rankingDate);

            ViewData["Date"] = rankingDate;

            var targetGuild = await _context.Guilds.Include(G => G.Characters).ThenInclude(C => C.Ranks)
                                                   .Include(G => G.Characters).ThenInclude(C => C.WeeklyRanks)
                                            .Where(G => G.Name == guildName)
                                            .FirstOrDefaultAsync(G => G.Name == guildName);

            return View(targetGuild);
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