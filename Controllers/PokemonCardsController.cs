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

            var pokemonCard = await _context.PokemonCard // get the card details from the database that match the card id to what is in database
                .FirstOrDefaultAsync(m => m.Id == id);
            if (pokemonCard == null)
            {
                return NotFound();
            }

            return View(pokemonCard);
        }

        // GET: PokemonCards/Create
        [Authorize] // Authorize is .NET way of making sure the user is logged in and not a guest 
        public IActionResult Create()
        {
            return View();
        }

        // POST: PokemonCards/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create([Bind("Id,Name,SetName,SetNumber,Type,Price,IsOwned,IsWanted")] PokemonCard pokemonCard)
        {
            if (ModelState.IsValid)
            {
                // grab the currently logged in user's email
                var email = User.FindFirstValue(ClaimTypes.Email);

                // attach the current user email to the card so user doesn't have to input it when creating card
                pokemonCard.UserEmail = email;

                // save the created card to the database
                _context.Add(pokemonCard);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index)); // go back to the list of cards which is index page
            }
            return View(pokemonCard); // if validation fails anywhere (controller or js) then just resubmit the view of a blank form
        }

        // GET: PokemonCards/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            // check if our current user is the creator by grabbing thier email
            var currentEmail = User.FindFirstValue(ClaimTypes.Email);

            // get the card from the database
            var pokemonCard = await _context.PokemonCard.FindAsync(id);

            if (pokemonCard.UserEmail != currentEmail) // forbids the user from editing the card if their email is not same on card when created
            {
                return Forbid(); // Returns 403 Forbidden
            }

        
            if (pokemonCard == null)
            {
                return NotFound();
            }
            return View(pokemonCard); // after done editing show the edit form with updated values 
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
                    var currentEmail = User.FindFirstValue(ClaimTypes.Email); // make sure cuurent user is owner of card 

                    // get the card from the database matching making sure ids match
                    var existingCard = await _context.PokemonCard.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);

                    if (existingCard == null)
                    {
                        return NotFound(); // original card wasnt found in db
                    }
                    if (existingCard.UserEmail != currentEmail)
                    {
                        return Forbid(); // requested card to edit had mismatch emails
                    }

                    // making sure UserEmail is preserved and not lost 
                    pokemonCard.UserEmail = existingCard.UserEmail;

                    // update teh database with edited card
                    _context.Update(pokemonCard);
                    await _context.SaveChangesAsync();
                }
                // database excpetions from .NET
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
            return View(pokemonCard); // validation failed at some point, reload the edit form
        }

        // GET: PokemonCards/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // get card from database
            var pokemonCard = await _context.PokemonCard
                .FirstOrDefaultAsync(m => m.Id == id);
            if (pokemonCard == null)
            {
                return NotFound();
            }
            // check if our current user is the creator since only they can delete it 
            var currentEmail = User.FindFirstValue(ClaimTypes.Email);
            if (pokemonCard.UserEmail != currentEmail)
            {
                return Forbid();
            }

            return View(pokemonCard); // show page asking for confirmation of deletion
        }

        // POST: PokemonCards/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // get card from db, and get current user email
            var pokemonCard = await _context.PokemonCard.FindAsync(id);
            var currentEmail = User.FindFirstValue(ClaimTypes.Email);

            if (pokemonCard == null || pokemonCard.UserEmail != currentEmail) // if current user is not creator of card forbid it
            {
                return Forbid();
            }

            // update the database to remove the card 
            _context.PokemonCard.Remove(pokemonCard);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        // .NET helper function to see if card exists by matching ids
        private bool PokemonCardExists(int id)
        {
            return _context.PokemonCard.Any(e => e.Id == id);
        }
    }
}
