using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CityPoint.Data;
using CityPoint.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace CityPoint.Controllers
{
    [Authorize] // All actions require login
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
            if (User.IsInRole("Staff"))
            {
                // Staff see all bookings
                var allBookings = await _context.Booking
                    .Include(b => b.Room)
                    .Include(b => b.User)
                    .OrderByDescending(b => b.CreatedAt)
                    .ToListAsync();
                return View(allBookings);
            }

            // Customer: only own bookings
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var bookings = await _context.Booking
                .Include(b => b.Room)
                .Include(b => b.User)
                .Where(b => b.UserId == userId)
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();
            return View(bookings);
        }

        // GET: Bookings/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            Booking booking;

            if (User.IsInRole("Staff"))
            {
                booking = await _context.Booking
                    .Include(b => b.Room)
                    .Include(b => b.User)
                    .FirstOrDefaultAsync(m => m.BookingId == id);
            }
            else
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                booking = await _context.Booking
                    .Include(b => b.Room)
                    .Include(b => b.User)
                    .FirstOrDefaultAsync(m => m.BookingId == id && m.UserId == userId);
            }

            if (booking == null) return NotFound();
            return View(booking);
        }

        // GET: Bookings/Create
        public IActionResult Create()
        {
            ViewData["RoomId"] = new SelectList(
                _context.Room.Where(r => r.IsAvailable)
                    .Select(r => new { r.RoomId, DisplayText = r.Name + " - " + r.Location + " (£" + r.HourlyRate + "/hr)" }),
                "RoomId",
                "DisplayText"
            );
            return View();
        }

        // POST: Bookings/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("RoomId,CheckInDate,CheckOutDate,NumberOfGuests,SpecialRequests")] Booking booking)
        {
            // Remove validation for fields we're setting manually
            ModelState.Remove("UserId");
            ModelState.Remove("Status");
            ModelState.Remove("IsPaid");
            ModelState.Remove("CreatedAt");

            if (ModelState.IsValid)
            {
                booking.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                booking.CreatedAt = DateTime.UtcNow;
                booking.Status = "Pending";
                booking.IsPaid = false;

                if (booking.CheckInDate < DateTime.Today)
                {
                    ModelState.AddModelError("CheckInDate", "Check-in date cannot be in the past.");
                    ViewData["RoomId"] = new SelectList(
                        _context.Room.Where(r => r.IsAvailable)
                            .Select(r => new { r.RoomId, DisplayText = r.Name + " - " + r.Location + " (£" + r.HourlyRate + "/hr)" }),
                        "RoomId",
                        "DisplayText"
                    );
                    return View(booking);
                }
                if (booking.CheckOutDate <= booking.CheckInDate)
                {
                    ModelState.AddModelError("CheckOutDate", "Check-out date must be after check-in date.");
                    ViewData["RoomId"] = new SelectList(
                        _context.Room.Where(r => r.IsAvailable)
                            .Select(r => new { r.RoomId, DisplayText = r.Name + " - " + r.Location + " (£" + r.HourlyRate + "/hr)" }),
                        "RoomId",
                        "DisplayText"
                    );
                    return View(booking);
                }

                _context.Add(booking);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Booking created successfully!";
                return RedirectToAction(nameof(Index));
            }

            ViewData["RoomId"] = new SelectList(
                _context.Room.Where(r => r.IsAvailable)
                    .Select(r => new { r.RoomId, DisplayText = r.Name + " - " + r.Location + " (£" + r.HourlyRate + "/hr)" }),
                "RoomId",
                "DisplayText",
                booking.RoomId
            );
            return View(booking);
        }

        // GET: Bookings/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            Booking booking;

            if (User.IsInRole("Staff"))
            {
                booking = await _context.Booking.FindAsync(id);
            }
            else
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                booking = await _context.Booking.FirstOrDefaultAsync(b => b.BookingId == id && b.UserId == userId);
            }

            if (booking == null) return NotFound();

            if (booking.Status != "Pending" && !User.IsInRole("Staff"))
            {
                TempData["Error"] = "Cannot edit a booking that is not pending.";
                return RedirectToAction(nameof(Index));
            }

            ViewData["RoomId"] = new SelectList(
                _context.Room.Where(r => r.IsAvailable || r.RoomId == booking.RoomId)
                    .Select(r => new { r.RoomId, DisplayText = r.Name + " - " + r.Location + " (£" + r.HourlyRate + "/hr)" }),
                "RoomId",
                "DisplayText",
                booking.RoomId
            );
            return View(booking);
        }

        // POST: Bookings/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BookingId,RoomId,CheckInDate,CheckOutDate,NumberOfGuests,SpecialRequests,Status,IsPaid")] Booking booking)
        {
            if (id != booking.BookingId) return NotFound();

            Booking existingBooking;
            if (User.IsInRole("Staff"))
            {
                existingBooking = await _context.Booking.AsNoTracking().FirstOrDefaultAsync(b => b.BookingId == id);
            }
            else
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                existingBooking = await _context.Booking.AsNoTracking().FirstOrDefaultAsync(b => b.BookingId == id && b.UserId == userId);
            }

            if (existingBooking == null) return NotFound();

            // Remove validation for fields we're preserving
            ModelState.Remove("UserId");
            ModelState.Remove("CreatedAt");

            if (ModelState.IsValid)
            {
                try
                {
                    booking.UserId = existingBooking.UserId;
                    booking.CreatedAt = existingBooking.CreatedAt;

                    // If not staff, preserve Status and IsPaid
                    if (!User.IsInRole("Staff"))
                    {
                        booking.Status = existingBooking.Status;
                        booking.IsPaid = existingBooking.IsPaid;
                    }

                    if (booking.CheckInDate < DateTime.Today)
                    {
                        ModelState.AddModelError("CheckInDate", "Check-in date cannot be in the past.");
                        ViewData["RoomId"] = new SelectList(
                            _context.Room.Where(r => r.IsAvailable || r.RoomId == booking.RoomId)
                                .Select(r => new { r.RoomId, DisplayText = r.Name + " - " + r.Location + " (£" + r.HourlyRate + "/hr)" }),
                            "RoomId",
                            "DisplayText",
                            booking.RoomId
                        );
                        return View(booking);
                    }

                    if (booking.CheckOutDate <= booking.CheckInDate)
                    {
                        ModelState.AddModelError("CheckOutDate", "Check-out date must be after check-in date.");
                        ViewData["RoomId"] = new SelectList(
                            _context.Room.Where(r => r.IsAvailable || r.RoomId == booking.RoomId)
                                .Select(r => new { r.RoomId, DisplayText = r.Name + " - " + r.Location + " (£" + r.HourlyRate + "/hr)" }),
                            "RoomId",
                            "DisplayText",
                            booking.RoomId
                        );
                        return View(booking);
                    }

                    _context.Update(booking);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Booking updated successfully!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Booking.Any(e => e.BookingId == id)) return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["RoomId"] = new SelectList(
                _context.Room.Where(r => r.IsAvailable || r.RoomId == booking.RoomId)
                    .Select(r => new { r.RoomId, DisplayText = r.Name + " - " + r.Location + " (£" + r.HourlyRate + "/hr)" }),
                "RoomId",
                "DisplayText",
                booking.RoomId
            );
            return View(booking);
        }

        // GET: Bookings/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            Booking booking;

            if (User.IsInRole("Staff"))
            {
                booking = await _context.Booking.Include(b => b.Room).Include(b => b.User).FirstOrDefaultAsync(b => b.BookingId == id);
            }
            else
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                booking = await _context.Booking.Include(b => b.Room).Include(b => b.User).FirstOrDefaultAsync(b => b.BookingId == id && b.UserId == userId);
            }

            if (booking == null) return NotFound();
            return View(booking);
        }

        // POST: Bookings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            Booking booking;
            if (User.IsInRole("Staff"))
            {
                booking = await _context.Booking.FindAsync(id);
            }
            else
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                booking = await _context.Booking.FirstOrDefaultAsync(b => b.BookingId == id && b.UserId == userId);
            }

            if (booking != null)
            {
                _context.Booking.Remove(booking);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Booking deleted successfully!";
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: Bookings/Approve/5
        [HttpPost]
        [Authorize(Roles = "Staff")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(int id)
        {
            var booking = await _context.Booking.FindAsync(id);
            if (booking == null) return NotFound();

            if (booking.Status != "Pending")
            {
                TempData["Error"] = "Only pending bookings can be approved.";
                return RedirectToAction(nameof(Index));
            }

            booking.Status = "Approved";
            _context.Update(booking);
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Booking #{booking.BookingId} has been approved.";
            return RedirectToAction(nameof(Index));
        }

        // POST: Bookings/Deny/5
        [HttpPost]
        [Authorize(Roles = "Staff")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Deny(int id)
        {
            var booking = await _context.Booking.FindAsync(id);
            if (booking == null) return NotFound();

            if (booking.Status != "Pending")
            {
                TempData["Error"] = "Only pending bookings can be denied.";
                return RedirectToAction(nameof(Index));
            }

            booking.Status = "Denied";
            _context.Update(booking);
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Booking #{booking.BookingId} has been denied.";
            return RedirectToAction(nameof(Index));
        }

        private bool BookingExists(int id)
        {
            return _context.Booking.Any(e => e.BookingId == id);
        }
    }
}