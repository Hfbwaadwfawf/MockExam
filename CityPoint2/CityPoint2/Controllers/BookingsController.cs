using CityPoint2.Data;
using CityPoint2.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CityPoint2.Controllers
{
    public class BookingsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BookingsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Bookings
        public async Task<IActionResult> Index()
        {
            var bookings = await _context.Bookings
                .Include(b => b.Rooms)
                .ToListAsync();

            return View(bookings);
        }

        // GET: Bookings/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bookings = await _context.Bookings
                .Include(b => b.Rooms)
                .FirstOrDefaultAsync(m => m.BookingsId == id);
            if (bookings == null)
            {
                return NotFound();
            }

            return View(bookings);
        }

        // GET: Bookings/Create
        public IActionResult Create()
        {
            ViewBag.RoomsId = new SelectList(_context.Rooms, "RoomsId", "Name");
            return View();
        }

        // POST: Bookings/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("RoomsId,CheckInDate,CheckOutDate,NumberOfGuests,SpecialRequests")] Bookings bookings)
        {
            // Get the logged-in user's ID from claims
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // If no user is logged in, use a default user for testing
            // REMOVE THIS IN PRODUCTION - require authentication instead
            if (string.IsNullOrEmpty(userId))
            {
                // Try to get the customer user from seed data
                var defaultUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == "customer@example.com");
                if (defaultUser != null)
                {
                    userId = defaultUser.Id;
                }
                else
                {
                    TempData["Error"] = "You must be logged in to create a booking.";
                    return RedirectToAction(nameof(Index));
                }
            }

            // Remove ModelState validation for properties we're setting manually
            ModelState.Remove("UserId");
            ModelState.Remove("IsPaid");
            ModelState.Remove("Status");
            ModelState.Remove("CreatedAt");

            if (ModelState.IsValid)
            {
                // Set UserId to the logged-in user's ID
                bookings.UserId = userId;

                // Set default values 
                bookings.IsPaid = false;
                bookings.Status = "Pending";
                bookings.CreatedAt = DateTime.UtcNow;

                _context.Add(bookings);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Booking created successfully!";
                return RedirectToAction(nameof(Index));
            }

            // Re-populate dropdown if validation fails
            ViewBag.RoomsId = new SelectList(_context.Rooms, "RoomsId", "Name");
            return View(bookings);
        }

        // GET: Bookings/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bookings = await _context.Bookings.FindAsync(id);
            if (bookings == null)
            {
                return NotFound();
            }

            ViewBag.RoomsId = new SelectList(_context.Rooms, "RoomsId", "Name", bookings.RoomsId);
            return View(bookings);
        }

        // POST: Bookings/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BookingsId,UserId,RoomsId,CheckInDate,CheckOutDate,NumberOfGuests,SpecialRequests,IsPaid,Status,CreatedAt")] Bookings bookings)
        {
            if (id != bookings.BookingsId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(bookings);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookingsExists(bookings.BookingsId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                TempData["Success"] = "Booking updated successfully!";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.RoomsId = new SelectList(_context.Rooms, "RoomsId", "Name", bookings.RoomsId);
            return View(bookings);
        }

        // GET: Bookings/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bookings = await _context.Bookings
                .Include(b => b.Rooms)
                .FirstOrDefaultAsync(m => m.BookingsId == id);
            if (bookings == null)
            {
                return NotFound();
            }

            return View(bookings);
        }

        // POST: Bookings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var bookings = await _context.Bookings.FindAsync(id);
            if (bookings != null)
            {
                _context.Bookings.Remove(bookings);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Booking deleted successfully!";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool BookingsExists(int id)
        {
            return _context.Bookings.Any(e => e.BookingsId == id);
        }
    }
}