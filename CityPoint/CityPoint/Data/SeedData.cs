using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using CityPoint.Models;

namespace CityPoint.Data
{
    public class SeedData
    {
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
            await SeedRooms(context, userManager);

            // Seed staff
            await SeedStaff(context);

            // Seed bookings
            await SeedBookingsAsync(serviceProvider, userManager);
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

            var user1 = await userManager.FindByEmailAsync("user1@example.com");
            if (user1 == null)
            {
                user1 = new IdentityUser
                {
                    UserName = "user1@example.com",
                    Email = "user1@example.com",
                    EmailConfirmed = true
                };

                await userManager.CreateAsync(user1, "User1@123");
            }

            if (!await userManager.IsInRoleAsync(user1, "Customer"))
            {
                await userManager.AddToRoleAsync(user1, "Customer");
            }

            var user2 = await userManager.FindByEmailAsync("user2@example.com");
            if (user2 == null)
            {
                user2 = new IdentityUser
                {
                    UserName = "user2@example.com",
                    Email = "user2@example.com",
                    EmailConfirmed = true
                };

                await userManager.CreateAsync(user2, "User2@123");
            }

            if (!await userManager.IsInRoleAsync(user2, "Customer"))
            {
                await userManager.AddToRoleAsync(user2, "Customer");
            }
        }

        // SEED ROOMS
        private static async Task SeedRooms(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            if (context.Room.Any())
                return;

            var staffUser = await userManager.FindByEmailAsync("staff@example.com");
            if (staffUser == null)
                return;

            var rooms = new List<Room>
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

        // SEED STAFF
        private static async Task SeedStaff(ApplicationDbContext context)
        {
            if (context.Staff.Any())
                return; // Already seeded

            var staffMembers = new List<Staff>
            {
                new Staff
                {
                    Name = "Sarah Johnson",
                    Role = "Front Desk Manager",
                    Bio = "Sarah has over 10 years of experience in hospitality management. She's your first point of contact for any booking inquiries or general questions."
                },
                new Staff
                {
                    Name = "Michael Chen",
                    Role = "Technical Support Specialist",
                    Bio = "Michael handles all technical issues including AV equipment, video conferencing setup, and IT support for our meeting rooms."
                },
                new Staff
                {
                    Name = "Emma Williams",
                    Role = "Event Coordinator",
                    Bio = "Emma specializes in organizing corporate events, conferences, and special occasions. Contact her for catering and event planning assistance."
                },
                new Staff
                {
                    Name = "David Martinez",
                    Role = "Facilities Manager",
                    Bio = "David ensures all our rooms are maintained to the highest standards. Report any facility issues or maintenance requests to him."
                },
                new Staff
                {
                    Name = "Lisa Anderson",
                    Role = "Customer Service Representative",
                    Bio = "Lisa is here to help with account inquiries, billing questions, and general customer support. She's always ready to assist with a smile."
                },
                new Staff
                {
                    Name = "James Thompson",
                    Role = "Security Coordinator",
                    Bio = "James oversees building security and access control. Contact him for security badges, after-hours access, or safety concerns."
                }
            };

            context.Staff.AddRange(staffMembers);
            await context.SaveChangesAsync();
        }

        // SEED BOOKINGS
        public static async Task SeedBookingsAsync(IServiceProvider serviceProvider, UserManager<IdentityUser> userManager)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            if (!context.Booking.Any())
            {
                var user1 = await userManager.FindByEmailAsync("user1@example.com");
                var user2 = await userManager.FindByEmailAsync("user2@example.com");

                if (user1 != null && user2 != null)
                {
                    var bookings = new List<Booking>()
                    {
                        new Booking
                        {
                            UserId = user1.Id,
                            RoomId = 1,
                            CheckInDate = DateTime.Today.AddDays(7),
                            CheckOutDate = DateTime.Today.AddDays(7).AddHours(3),
                            NumberOfGuests = 8,
                            SpecialRequests = "Need projector and whiteboard",
                            IsPaid = false,
                            Status = "Pending",
                            CreatedAt = DateTime.UtcNow
                        },
                        new Booking
                        {
                            UserId = user2.Id,
                            RoomId = 2,
                            CheckInDate = DateTime.Today.AddDays(14),
                            CheckOutDate = DateTime.Today.AddDays(14).AddHours(5),
                            NumberOfGuests = 25,
                            SpecialRequests = "Catering required",
                            IsPaid = false,
                            Status = "Pending",
                            CreatedAt = DateTime.UtcNow
                        },
                    };

                    context.Booking.AddRange(bookings);
                    await context.SaveChangesAsync();
                }
            }
        }
    }
}