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
    [Authorize] // All actions require login
    public class RoomsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RoomsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Rooms
        [AllowAnonymous] // Anyone can see the rooms list if you want, else remove AllowAnonymous
        public async Task<IActionResult> Index(float? minPrice, float? maxPrice, bool? isAvailable)
        {
            var room = _context.Room.AsQueryable();

            // Filter by minimum price
            if (minPrice.HasValue)
            {
                room = room.Where(r => r.HourlyRate >= (decimal)maxPrice.Value);
            }

            // Filter by maximum price
            if (maxPrice.HasValue)
            {
                room = room.Where(r => r.HourlyRate <= (decimal)maxPrice.Value);
            }

            // Filter by availability
            if (isAvailable.HasValue)
            {
                room = room.Where(r => r.IsAvailable == isAvailable.Value);
            }

            // Store filter values in ViewData to maintain them in the form
            ViewData["MinPrice"] = minPrice;
            ViewData["MaxPrice"] = maxPrice;
            ViewData["IsAvailable"] = isAvailable;

            return View(await room.ToListAsync());
        }

        // GET: Rooms/Details/5
        [AllowAnonymous] // Customers and Staff can view details
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

        // GET: Rooms/Create
        [Authorize(Roles = "Staff")] // Only Staff can create rooms
        public IActionResult Create()
        {
            return View();
        }

        // POST: Rooms/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Staff")] // Only Staff
        public async Task<IActionResult> Create([Bind("Name,Description,HourlyRate,Location,IsAvailable")] Room room)
        {
            if (ModelState.IsValid)
            {
                _context.Add(room);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(room);
        }

        // GET: Rooms/Edit/5
        [Authorize(Roles = "Staff")] // Only Staff can edit
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

        // POST: Rooms/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Staff")] // Only Staff
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

        // GET: Rooms/Delete/5
        [Authorize(Roles = "Staff")] // Only Staff can delete
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

        // POST: Rooms/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Staff")] // Only Staff
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var room = await _context.Room.FindAsync(id);
            if (room != null)
            {
                _context.Room.Remove(room);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RoomExists(int id)
        {
            return _context.Room.Any(e => e.RoomId == id);
        }
    }
}
