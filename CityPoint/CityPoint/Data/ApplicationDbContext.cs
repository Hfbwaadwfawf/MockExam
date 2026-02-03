using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using CityPoint.Models;

namespace CityPoint.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<CityPoint.Models.Rooms> Rooms { get; set; } = default!;
        public DbSet<CityPoint.Models.Staff> Staff { get; set; } = default!;
        public DbSet<CityPoint.Models.Bookings> Bookings { get; set; } = default!;
    }
}
