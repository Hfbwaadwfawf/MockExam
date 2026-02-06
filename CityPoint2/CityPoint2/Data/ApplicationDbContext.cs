using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using CityPoint2.Models;

namespace CityPoint2.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<CityPoint2.Models.Bookings> Bookings { get; set; } = default!;
        public DbSet<CityPoint2.Models.Rooms> Rooms { get; set; } = default!;
        public DbSet<CityPoint2.Models.Staff> Staff { get; set; } = default!;
    }
}
