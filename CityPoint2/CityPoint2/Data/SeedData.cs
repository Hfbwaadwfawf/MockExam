using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CityPoint2.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CityPoint2.Data
{
    public class SeedData
    {
        public static async Task SeedRoomsAsync(ApplicationDbContext context)
        {
            if (!await context.Rooms.AnyAsync())
            {
                var rooms = new List<Rooms>
                {
                    new Rooms
                    {
                        Name = "Focus Room A",
                        Description = "Intimate 4-person meeting space with whiteboard, 42-inch display, and video conferencing setup.",
                        HourlyRate = 25.00f,
                        Location = "Building A, Floor 2",
                        IsAvailable = true
                    },
                    new Rooms
                    {
                        Name = "Collaboration Suite",
                        Description = "Modern 10-person meeting room featuring a 55-inch 4K display, conference phone, and whiteboard walls.",
                        HourlyRate = 50.00f,
                        Location = "Building A, Floor 3",
                        IsAvailable = true
                    },
                    new Rooms
                    {
                        Name = "Executive Boardroom",
                        Description = "Premium 16-person boardroom with professional video conferencing, 75-inch display, and dedicated tech support.",
                        HourlyRate = 85.00f,
                        Location = "Building A, Floor 5",
                        IsAvailable = false
                    },
                    new Rooms
                    {
                        Name = "Training Center",
                        Description = "30-person classroom-style setup with projection screen, microphone system, and individual desks.",
                        HourlyRate = 95.00f,
                        Location = "Building B, Floor 2",
                        IsAvailable = true
                    },
                    new Rooms
                    {
                        Name = "Privacy Pod",
                        Description = "Soundproof individual booth for private calls and focused work with desk, power outlets, and USB charging.",
                        HourlyRate = 15.00f,
                        Location = "Building A, Floor 1",
                        IsAvailable = true
                    }
                };

                await context.Rooms.AddRangeAsync(rooms);
                await context.SaveChangesAsync();
            }
        }

        public static async Task SeedRoles(IServiceProvider serviceProvider, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            // Create roles if they don't exist
            string[] roles = { "Admin", "Staff", "Customer" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            // Create admin user
            var adminUser = await userManager.FindByEmailAsync("admin@example.com");
            if (adminUser == null)
            {
                adminUser = new IdentityUser
                {
                    UserName = "admin@example.com",
                    Email = "admin@example.com",
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(adminUser, "Admin@123");
            }

            if (!await userManager.IsInRoleAsync(adminUser, "Admin"))
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }

            // Create staff user
            var staffUser = await userManager.FindByEmailAsync("staff@example.com");
            if (staffUser == null)
            {
                staffUser = new IdentityUser
                {
                    UserName = "staff@example.com",
                    Email = "staff@example.com",
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(staffUser, "Staff@123");
            }

            if (!await userManager.IsInRoleAsync(staffUser, "Staff"))
            {
                await userManager.AddToRoleAsync(staffUser, "Staff");
            }

            // Create customer user
            var customerUser = await userManager.FindByEmailAsync("customer@example.com");
            if (customerUser == null)
            {
                customerUser = new IdentityUser
                {
                    UserName = "customer@example.com",
                    Email = "customer@example.com",
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(customerUser, "Customer@123");
            }

            if (!await userManager.IsInRoleAsync(customerUser, "Customer"))
            {
                await userManager.AddToRoleAsync(customerUser, "Customer");
            }
        }

        public static async Task SeedBookingsAsync(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            if (!await context.Bookings.AnyAsync())
            {
                // Get user IDs
                var adminUser = await userManager.FindByEmailAsync("admin@example.com");
                var staffUser = await userManager.FindByEmailAsync("staff@example.com");
                var customerUser = await userManager.FindByEmailAsync("customer@example.com");

                var bookings = new List<Bookings>
                {
                    new Bookings
                    {
                        UserId = customerUser.Id,
                        RoomsId = 1,
                        CheckInDate = DateTime.Now.AddDays(2).AddHours(9),
                        CheckOutDate = DateTime.Now.AddDays(2).AddHours(11),
                        NumberOfGuests = 3,
                        SpecialRequests = "Need extra whiteboard markers",
                        IsPaid = true,
                        Status = "Confirmed"
                    },
                    new Bookings
                    {
                        UserId = customerUser.Id,
                        RoomsId = 2,
                        CheckInDate = DateTime.Now.AddDays(1).AddHours(14),
                        CheckOutDate = DateTime.Now.AddDays(1).AddHours(16),
                        NumberOfGuests = 8,
                        SpecialRequests = null,
                        IsPaid = true,
                        Status = "Confirmed"
                    },
                    new Bookings
                    {
                        UserId = adminUser.Id,
                        RoomsId = 3,
                        CheckInDate = DateTime.Now.AddDays(5).AddHours(10),
                        CheckOutDate = DateTime.Now.AddDays(5).AddHours(12),
                        NumberOfGuests = 12,
                        SpecialRequests = "Coffee and tea service required",
                        IsPaid = false,
                        Status = "Pending"
                    },
                    new Bookings
                    {
                        UserId = staffUser.Id,
                        RoomsId = 5,
                        CheckInDate = DateTime.Now.AddHours(3),
                        CheckOutDate = DateTime.Now.AddHours(4),
                        NumberOfGuests = 1,
                        SpecialRequests = "Quiet space for important call",
                        IsPaid = true,
                        Status = "Confirmed"
                    },
                    new Bookings
                    {
                        UserId = customerUser.Id,
                        RoomsId = 4,
                        CheckInDate = DateTime.Now.AddDays(-2).AddHours(9),
                        CheckOutDate = DateTime.Now.AddDays(-2).AddHours(17),
                        NumberOfGuests = 25,
                        SpecialRequests = "Projector and sound system",
                        IsPaid = true,
                        Status = "Completed"
                    }
                };

                await context.Bookings.AddRangeAsync(bookings);
                await context.SaveChangesAsync();
            }
        }
    }
}
