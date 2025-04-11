using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PokemonCardCollector.Data;
using PokemonCardCollector.Models;

namespace PokemonCardCollector.Controllers
{
    public class PokemonCardsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PokemonCardsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: PokemonCards
        public async Task<IActionResult> Index()
        {
            return View(await _context.PokemonCard.ToListAsync());
        }

        // GET: PokemonCards/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pokemonCard = await _context.PokemonCard
                .FirstOrDefaultAsync(m => m.Id == id);
            if (pokemonCard == null)
            {
                return NotFound();
            }

            return View(pokemonCard);
        }

        // GET: PokemonCards/Create
        [Authorize] // Add this to ensure user is logged in
        public IActionResult Create()
        {
            return View();
        }

        // POST: PokemonCards/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize] // Add this to ensure user is logged in
        public async Task<IActionResult> Create([Bind("Id,Name,SetName,SetNumber,Type,Price,IsOwned,IsWanted")] PokemonCard pokemonCard)
        {
            if (ModelState.IsValid)
            {
                // Get the currently logged in user's email
                var email = User.FindFirstValue(ClaimTypes.Email);

                // Set the UserEmail properly - no "Guest" option if the method is protected by [Authorize]
                pokemonCard.UserEmail = email;

                _context.Add(pokemonCard);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(pokemonCard);
        }

        // GET: PokemonCards/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            // Check if our current user is the creator
            var currentEmail = User.FindFirstValue(ClaimTypes.Email);
            var pokemonCard = await _context.PokemonCard.FindAsync(id);
            if (pokemonCard.UserEmail != currentEmail)
            {
                return Forbid(); // Returns 403 Forbidden
            }

        
            if (pokemonCard == null)
            {
                return NotFound();
            }
            return View(pokemonCard);
        }

        // POST: PokemonCards/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,SetName,SetNumber,Type,Price,IsOwned,IsWanted")] PokemonCard pokemonCard)
        {
            if (id != pokemonCard.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // re-fetch our original card to compare email before updating
                    var currentEmail = User.FindFirstValue(ClaimTypes.Email);
                    var existingCard = await _context.PokemonCard.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);
                    if (existingCard == null)
                    {
                        return NotFound();
                    }
                    if (existingCard.UserEmail != currentEmail)
                    {
                        return Forbid();
                    }

                    // making sure UserEmail is preserved
                    pokemonCard.UserEmail = existingCard.UserEmail;

                    _context.Update(pokemonCard);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PokemonCardExists(pokemonCard.Id))
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
            return View(pokemonCard);
        }

        // GET: PokemonCards/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pokemonCard = await _context.PokemonCard
                .FirstOrDefaultAsync(m => m.Id == id);
            if (pokemonCard == null)
            {
                return NotFound();
            }
            // Check if our current user is the creator
            var currentEmail = User.FindFirstValue(ClaimTypes.Email);
            if (pokemonCard.UserEmail != currentEmail)
            {
                return Forbid();
            }

            return View(pokemonCard);
        }

        // POST: PokemonCards/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var pokemonCard = await _context.PokemonCard.FindAsync(id);
            var currentEmail = User.FindFirstValue(ClaimTypes.Email);

            if (pokemonCard == null || pokemonCard.UserEmail != currentEmail)
            {
                return Forbid();
            }

            _context.PokemonCard.Remove(pokemonCard);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PokemonCardExists(int id)
        {
            return _context.PokemonCard.Any(e => e.Id == id);
        }
    }
}
