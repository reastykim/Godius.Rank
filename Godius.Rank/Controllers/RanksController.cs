using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Godius.Data;
using Godius.Data.Models;

namespace Godius.RankSite.Controllers
{
    public class RanksController : Controller
    {
        private readonly RankContext _context;

        public RanksController(RankContext context)
        {
            _context = context;
        }

        // GET: Ranks
        public async Task<IActionResult> Index()
        {
            var rankContext = _context.Ranks.Include(r => r.Character).ThenInclude(C => C.Guild)
                                            .OrderBy(R => R.Character.Guild.Name)
                                            .ThenBy(R => R.Character.Name).ThenBy(R => R.Date);
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

        private bool RankExists(Guid id)
        {
            return _context.Ranks.Any(e => e.Id == id);
        }
    }
}
