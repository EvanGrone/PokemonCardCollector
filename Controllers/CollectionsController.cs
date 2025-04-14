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
    public class CollectionsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CollectionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Collections
        public async Task<IActionResult> Index()
        {
            return View(await _context.Collection.ToListAsync());
        }

        // GET: Collections/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var collection = await _context.Collection
                .Include(c => c.Cards) // include the cards in the collection
                .FirstOrDefaultAsync(m => m.Id == id);
            if (collection == null)
            {
                return NotFound();
            }

            return View(collection);
        }

        // GET: Collections/Create
        [Authorize]
        public IActionResult Create()
        {
            //In ASP.NET MVC, ViewBag is a dynamic property of the ControllerBase class, used to pass data from the controller to the view.
            //It functions as a wrapper around the ViewData object, providing a more concise syntax for accessing data in views.
            //ViewBag allows you to dynamically add properties, set values, and retrieve them from the view using the same names, without requiring explicit type definitions
            // get our currently logged in user's email
            var userEmail = User.FindFirstValue(ClaimTypes.Email);

            // pass the email to the view
            ViewBag.UserEmail = userEmail;

            // grab user's cards to populate the multi-select box 
            ViewBag.UserCards = new MultiSelectList(_context.PokemonCard.Where(c => c.UserEmail == userEmail), "Id", "Name");

            return View();
        }

        // POST: Collections/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create([Bind("Id,Name,Description")] Collection collection, int[] SelectedCards)
        {
            if (ModelState.IsValid)
            {
                var email = User.FindFirstValue(ClaimTypes.Email);
                collection.UserEmail = email;
                collection.CreatedDate = DateTime.Now;

                _context.Add(collection);
                await _context.SaveChangesAsync();

                // add our selected cards to the collection
                if (SelectedCards != null && SelectedCards.Length > 0)
                {
                    foreach (var cardId in SelectedCards)
                    {
                        var card = await _context.PokemonCard.FindAsync(cardId);
                        if (card != null && card.UserEmail == email)
                        {
                            collection.Cards.Add(card);
                        }
                    }
                    await _context.SaveChangesAsync();
                }

                return RedirectToAction(nameof(Index));
            }

            // if we get here, something failed, redisplay our form
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            ViewBag.UserEmail = userEmail;
            ViewBag.UserCards = new MultiSelectList(_context.PokemonCard.Where(c => c.UserEmail == userEmail), "Id", "Name");

            return View(collection);
        }

        // GET: Collections/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var currentEmail = User.FindFirstValue(ClaimTypes.Email);
            var collection = await _context.Collection
                .Include(c => c.Cards)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (collection == null)
            {
                return NotFound();
            }

            if (collection.UserEmail != currentEmail)
            {
                return Forbid(); // Returns 403 Forbidden
            }

            // grab all the user’s cards for the multi-select, and check which ones are already in this collection
            var userCards = await _context.PokemonCard
                .Where(c => c.UserEmail == currentEmail)
                .ToListAsync();

            ViewBag.UserEmail = currentEmail;
            ViewBag.UserCards = new MultiSelectList(userCards, "Id", "Name",
                collection.Cards.Select(c => c.Id));

            return View(collection);
        }

        // POST: Collections/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,CreatedDate")] Collection collection, int[] SelectedCards)
        {
            if (id != collection.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // check if the user is owner of the collection 
                    var currentEmail = User.FindFirstValue(ClaimTypes.Email);
                    var existingCollection = await _context.Collection
                        .Include(c => c.Cards)
                        .AsNoTracking()
                        .FirstOrDefaultAsync(c => c.Id == id);

                    if (existingCollection == null)
                    {
                        return NotFound();
                    }
                    if (existingCollection.UserEmail != currentEmail)
                    {
                        return Forbid();
                    }

                    // keep the original email for the collection
                    collection.UserEmail = existingCollection.UserEmail;

                    _context.Update(collection);

                    // update the cards in collection
                    var collectionToUpdate = await _context.Collection
                        .Include(c => c.Cards)
                        .FirstOrDefaultAsync(c => c.Id == id);

                    collectionToUpdate.Cards.Clear();

                    // add the selected cards that are owned by logged in user 
                    if (SelectedCards != null)
                    {
                        foreach (var cardId in SelectedCards)
                        {
                            var card = await _context.PokemonCard.FindAsync(cardId);
                            if (card != null && card.UserEmail == currentEmail)
                            {
                                collectionToUpdate.Cards.Add(card);
                            }
                        }
                    }

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CollectionExists(collection.Id))
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

            // if we get here, something failed, redisplay our form
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            ViewBag.UserEmail = userEmail;
            ViewBag.UserCards = new MultiSelectList(_context.PokemonCard.Where(c => c.UserEmail == userEmail), "Id", "Name");

            return View(collection);
        }

        // GET: Collections/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var collection = await _context.Collection
                .FirstOrDefaultAsync(m => m.Id == id);
            if (collection == null)
            {
                return NotFound();
            }

            // Check if our current user is the creator
            var currentEmail = User.FindFirstValue(ClaimTypes.Email);
            if (collection.UserEmail != currentEmail)
            {
                return Forbid();
            }

            return View(collection);
        }

        // POST: Collections/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var collection = await _context.Collection
                .Include(c => c.Cards)
                .FirstOrDefaultAsync(c => c.Id == id);

            var currentEmail = User.FindFirstValue(ClaimTypes.Email);


            // make sure collection belongs to logged in user
            if (collection == null || collection.UserEmail != currentEmail)
            {
                return Forbid();
            }

            // remove the relationship between cards and this collection as to not delete the card 
            foreach (var card in collection.Cards.ToList())
            {
                collection.Cards.Remove(card);
            }

            // save the changes to remove relationships
            await _context.SaveChangesAsync();

            // delete the collection 
            _context.Collection.Remove(collection);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        // .NET hekoer function to check for collections
        private bool CollectionExists(int id)
        {
            return _context.Collection.Any(e => e.Id == id);
        }
    }
}