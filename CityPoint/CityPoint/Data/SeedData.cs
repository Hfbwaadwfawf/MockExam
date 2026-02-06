using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using CityPoint.Models;

namespace CityPoint.Data
{
    public class SeedData
    {
        // MAIN METHOD CALLED FROM PROGRAM.CS
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            using var context = new ApplicationDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>());

            var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            // Ensure database exists
            context.Database.Migrate();

            // Seed roles + users
            await SeedRoles(userManager, roleManager);

            // Seed rooms
            await SeedRooms(context);
        }

        // CREATE ROLES + DEFAULT USERS
        private static async Task SeedRoles(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            string[] roles = { "Staff", "Customer" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            // ===== STAFF USER =====
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

            // ===== CUSTOMER USER =====
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

        // SEED ROOMS
        private static async Task SeedRooms(ApplicationDbContext context)
        {
            if (context.Room.Any())
                return; // already seeded

            var rooms = new Room[]
            {
                new Room
                {
                    Name = "Executive Boardroom",
                    Description = "Premium boardroom with video conferencing and seating for 12.",
                    HourlyRate = 75.00m,
                    Location = "5th Floor, Building A",
                    IsAvailable = true
                },
                new Room
                {
                    Name = "Conference Hall",
                    Description = "Large hall for events and seminars up to 50 people.",
                    HourlyRate = 125.00m,
                    Location = "Ground Floor, Building B",
                    IsAvailable = true
                },
                new Room
                {
                    Name = "Meeting Pod",
                    Description = "Small collaborative meeting space for teams.",
                    HourlyRate = 35.00m,
                    Location = "3rd Floor, Building A",
                    IsAvailable = true
                }
            };

            context.Room.AddRange(rooms);
            await context.SaveChangesAsync();
        }
    }
}
