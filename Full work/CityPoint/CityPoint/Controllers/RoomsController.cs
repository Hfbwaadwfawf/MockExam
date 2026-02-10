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

namespace CityPoint.Controllers
{
    // Manages meeting rooms (all users view, only staff modify)
    [Authorize]
    public class RoomsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RoomsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Filterable room list, customers see only available by default
        public async Task<IActionResult> Index(decimal? minPrice, decimal? maxPrice, bool? isAvailable)
        {
            var rooms = _context.Room.AsQueryable();

            if (minPrice.HasValue)
            {
                rooms = rooms.Where(r => r.HourlyRate >= minPrice.Value);
            }

            if (maxPrice.HasValue)
            {
                rooms = rooms.Where(r => r.HourlyRate <= maxPrice.Value);
            }

            if (isAvailable.HasValue)
            {
                rooms = rooms.Where(r => r.IsAvailable == isAvailable.Value);
            }
            else
            {
                // Customers only see available rooms by default
                if (!User.IsInRole("Staff"))
                {
                    rooms = rooms.Where(r => r.IsAvailable);
                }
            }

            ViewData["MinPrice"] = minPrice;
            ViewData["MaxPrice"] = maxPrice;
            ViewData["IsAvailable"] = isAvailable;

            return View(await rooms.ToListAsync());
        }

        // Display room details
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var room = await _context.Room
                .FirstOrDefaultAsync(m => m.RoomId == id);
            if (room == null)
            {
                return NotFound();
            }

            return View(room);
        }

        // Show room creation form (staff only)
        [Authorize(Roles = "Staff")]
        public IActionResult Create()
        {
            return View();
        }

        // Create new room (staff only)
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Staff")]
        public async Task<IActionResult> Create([Bind("Name,Description,HourlyRate,Location,IsAvailable")] Room room)
        {
            if (ModelState.IsValid)
            {
                _context.Add(room);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Room created successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View(room);
        }

        // Show room edit form (staff only)
        [Authorize(Roles = "Staff")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var room = await _context.Room.FindAsync(id);
            if (room == null)
            {
                return NotFound();
            }
            return View(room);
        }

        // Update room (staff only)
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Staff")]
        public async Task<IActionResult> Edit(int id, [Bind("RoomId,Name,Description,HourlyRate,Location,IsAvailable")] Room room)
        {
            if (id != room.RoomId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(room);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Room updated successfully!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RoomExists(room.RoomId))
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
            return View(room);
        }

        // Show delete confirmation (staff only)
        [Authorize(Roles = "Staff")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var room = await _context.Room
                .FirstOrDefaultAsync(m => m.RoomId == id);
            if (room == null)
            {
                return NotFound();
            }

            return View(room);
        }

        // Delete room if no bookings exist (staff only)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Staff")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var room = await _context.Room.FindAsync(id);
            if (room != null)
            {
                // Prevent deletion if bookings exist
                var hasBookings = await _context.Booking.AnyAsync(b => b.RoomId == id);
                if (hasBookings)
                {
                    TempData["Error"] = "Cannot delete room with existing bookings.";
                    return RedirectToAction(nameof(Index));
                }

                _context.Room.Remove(room);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Room deleted successfully!";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool RoomExists(int id)
        {
            return _context.Room.Any(e => e.RoomId == id);
        }
    }
}