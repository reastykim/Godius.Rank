using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Godius.RankSite.Data;
using Godius.RankSite.Models;
using Godius.RankSite.Extensions;

namespace Godius.RankSite.Controllers
{
    public class CharactersController : Controller
    {
        private readonly RankContext _context;

        public CharactersController(RankContext context)
        {
            _context = context;
        }

        // GET: Characters
        public async Task<IActionResult> Index()
        {
            var rankContext = _context.Characters.Include(c => c.Guild);
            return View(await rankContext.ToListAsync());
        }

        // GET: Characters/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var character = await _context.Characters
                .Include(c => c.Guild)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (character == null)
            {
                return NotFound();
            }

            return View(character);
        }

        // GET: Characters/Create
        public IActionResult Create()
        {
            ViewData["GuildId"] = new SelectList(_context.Guilds, "Id", "Name");
            var guildPositions = EnumHelper<GuildPositions>.GetValues().Select(GP => new { Key = GP, Value = EnumHelper<GuildPositions>.GetDisplayValue(GP) });
            ViewData["GuildPosition"] = new SelectList(guildPositions, "Key", "Value");
            return View();
        }

        // POST: Characters/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,GuildId,GuildPosition")] Character character)
        {
            if (ModelState.IsValid)
            {
                character.Id = Guid.NewGuid();
                _context.Add(character);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["GuildId"] = new SelectList(_context.Guilds, "Id", "Name", character.GuildId);
            var guildPositions = EnumHelper<GuildPositions>.GetValues().Select(GP => new { Key = GP, Value = EnumHelper<GuildPositions>.GetDisplayValue(GP) });
            ViewData["GuildPosition"] = new SelectList(guildPositions, "Key", "Value", character.GuildPosition);
            return View(character);
        }

        // GET: Characters/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var character = await _context.Characters.SingleOrDefaultAsync(m => m.Id == id);
            if (character == null)
            {
                return NotFound();
            }
            ViewData["GuildId"] = new SelectList(_context.Guilds, "Id", "Name", character.GuildId);
            var guildPositions = EnumHelper<GuildPositions>.GetValues().Select(GP => new { Key = GP, Value = EnumHelper<GuildPositions>.GetDisplayValue(GP) });
            ViewData["GuildPosition"] = new SelectList(guildPositions, "Key", "Value", character.GuildPosition);
            return View(character);
        }

        // POST: Characters/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Name,GuildId,GuildPosition")] Character character)
        {
            if (id != character.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(character);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CharacterExists(character.Id))
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
            ViewData["GuildId"] = new SelectList(_context.Guilds, "Id", "Id", character.GuildId);
            return View(character);
        }

        // GET: Characters/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var character = await _context.Characters
                .Include(c => c.Guild)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (character == null)
            {
                return NotFound();
            }

            return View(character);
        }

        // POST: Characters/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var character = await _context.Characters.SingleOrDefaultAsync(m => m.Id == id);
            _context.Characters.Remove(character);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CharacterExists(Guid id)
        {
            return _context.Characters.Any(e => e.Id == id);
        }
    }
}
