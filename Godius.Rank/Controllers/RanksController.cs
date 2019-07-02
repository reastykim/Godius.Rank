using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Godius.Data;
using Godius.Data.Models;
using Godius.WebCrawler;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.Authorization;

namespace Godius.RankSite.Controllers
{
    public class RanksController : Controller
    {
        private readonly RankContext _context;
        private readonly IMemoryCache _memoryCache;
        private readonly IRankingCrawler _rankingCrawler;

        public RanksController(RankContext context, IMemoryCache memoryCache)
        {
            _context = context;
            _memoryCache = memoryCache;
            _rankingCrawler = new RankingCrawlerV2();
        }

        // GET: Ranks
        public async Task<IActionResult> Index()
        {
            var rankContext = _context.Ranks.Include(r => r.Character).ThenInclude(C => C.Guild)
                                            .OrderBy(R => R.Character.Guild.Name)
                                            .ThenBy(R => R.Character.Name).ThenByDescending(R => R.Date);
            return View(await rankContext.ToListAsync());
        }

        // GET: Ranks/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rank = await _context.Ranks
                .Include(r => r.Character)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (rank == null)
            {
                return NotFound();
            }

            return View(rank);
        }

        // GET: Ranks/Create
        public IActionResult Create()
        {
            ViewData["CharacterId"] = new SelectList(_context.Characters.Include(C => C.Guild)
                                                                        .OrderBy(C => C.Guild.Name)
                                                                        .ThenBy(C => C.Name), "Id", "Name", null, "Guild.Name");
            return View();
        }

        // POST: Ranks/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,CharacterId,Date,Ranking")] Rank rank)
        {
            if (ModelState.IsValid)
            {
                rank.Id = Guid.NewGuid();
                _context.Add(rank);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CharacterId"] = new SelectList(_context.Characters, "Id", "Id", rank.CharacterId);
            return View(rank);
        }

        // GET: Ranks/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rank = await _context.Ranks.SingleOrDefaultAsync(m => m.Id == id);
            if (rank == null)
            {
                return NotFound();
            }
            ViewData["CharacterId"] = new SelectList(_context.Characters, "Id", "Id", rank.CharacterId);
            return View(rank);
        }

        // POST: Ranks/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,CharacterId,Date,Ranking")] Rank rank)
        {
            if (id != rank.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(rank);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RankExists(rank.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CharacterId"] = new SelectList(_context.Characters, "Id", "Id", rank.CharacterId);
            return View(rank);
        }

        // GET: Ranks/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rank = await _context.Ranks
                .Include(r => r.Character)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (rank == null)
            {
                return NotFound();
            }

            return View(rank);
        }

        // POST: Ranks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var rank = await _context.Ranks.SingleOrDefaultAsync(m => m.Id == id);
            _context.Ranks.Remove(rank);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateAll()
        {
            var rankingDate = GetRankingUpdatedDate();
            var characters = _context.Characters.Include(C => C.Guild).Include(C => C.Ranks);
            foreach (var character in characters)
            {
                var rankText = await _rankingCrawler.GetCharacterRankingAsync(character.Name);
                if (String.IsNullOrWhiteSpace(rankText))
                {
                    continue;
                }

                // Update Character Ranking
                var rank = character.Ranks.FirstOrDefault(R => R.Date.Value == rankingDate);
                if (rank == null)
                {
                    rank = new Rank { CharacterId = character.Id, Ranking = Int32.Parse(rankText), Date = rankingDate };
                    _context.Ranks.Add(rank);
                }
                //else // Warning!!
                //{
                //    rank.Ranking = Int32.Parse(rankText);
                //}
            }

            // Update Guild Weekly Ranking
            foreach (var guild in characters.Where(C => C.IsActivated && C.GuildId != null).GroupBy(C => C.GuildId))
            {
                var currentWeekRanks = guild.Select(C => new { C.Id, C.Name, Rank = C.Ranks?.FirstOrDefault(R => R.Date.GetValueOrDefault() == rankingDate) })
                                            .Where(CWR => CWR.Rank != null)
                                            .OrderBy(CWR => CWR.Rank.Ranking)
                                            .ToList();

                for (int i = 1; i <= currentWeekRanks.Count; i++)
                {
                    var character = currentWeekRanks[i - 1];
                    var weeklyRank = _context.WeeklyRanks.FirstOrDefault(WR => WR.CharacterId == character.Id && WR.Date == rankingDate);
                    if (weeklyRank == null)
                    {
                        weeklyRank = new WeeklyRank { CharacterId = character.Id, GuildId = guild.Key, Ranking = i, Date = rankingDate };
                        _context.WeeklyRanks.Add(weeklyRank);
                    }
                    else // Warning!!
                    {
                        weeklyRank.Ranking = i;
                    }
                }
            }

            await _context.SaveChangesAsync();

            // Clear cache for each Guild
            foreach (var guildName in _context.Guilds.Select(G => G.Name))
            {
                _memoryCache.Remove($"{guildName}_{rankingDate.ToString("yyyy-MM-dd")}");
            }            

            return RedirectToAction(nameof(Index));
        }

        private bool RankExists(Guid id)
        {
            return _context.Ranks.Any(e => e.Id == id);
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
