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
    public class GuildsController : Controller
    {
        private readonly RankContext _context;

        public GuildsController(RankContext context)
        {
            _context = context;
        }

        // GET: Guilds
        public async Task<IActionResult> Index()
        {
            return View(await _context.Guilds.ToListAsync());
        }

        // GET: Guilds/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var guild = await _context.Guilds
                .SingleOrDefaultAsync(m => m.Id == id);
            if (guild == null)
            {
                return NotFound();
            }

            return View(guild);
        }

        // GET: Guilds/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Guilds/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Alias,Image")] Guild guild)
        {
            if (ModelState.IsValid)
            {
                guild.Id = Guid.NewGuid();
                _context.Add(guild);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(guild);
        }

        // GET: Guilds/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var guild = await _context.Guilds.SingleOrDefaultAsync(m => m.Id == id);
            if (guild == null)
            {
                return NotFound();
            }
            return View(guild);
        }

        // POST: Guilds/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Name,Alias,Image")] Guild guild)
        {
            if (id != guild.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(guild);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GuildExists(guild.Id))
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
            return View(guild);
        }

        // GET: Guilds/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var guild = await _context.Guilds
                .SingleOrDefaultAsync(m => m.Id == id);
            if (guild == null)
            {
                return NotFound();
            }

            return View(guild);
        }

        // POST: Guilds/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var guild = await _context.Guilds.SingleOrDefaultAsync(m => m.Id == id);
            _context.Guilds.Remove(guild);
			await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GuildExists(Guid id)
        {
            return _context.Guilds.Any(e => e.Id == id);
        }
    }
}
