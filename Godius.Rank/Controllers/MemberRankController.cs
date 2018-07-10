using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godius.Data;
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

        public async Task<IActionResult> Index()
        {
            var subDomain = HttpContext.Request.Host.Host.Split('.', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();
            var guildName = String.Empty;
            switch (subDomain?.ToUpper())
            {
                case "GISADAN":
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

            var guild = await _context.Guilds.Include(G => G.Characters)
                                      .ThenInclude(C => C.Ranks)
                                      .FirstOrDefaultAsync(G => G.Name == guildName);


            return View(guild);
        }
    }
}