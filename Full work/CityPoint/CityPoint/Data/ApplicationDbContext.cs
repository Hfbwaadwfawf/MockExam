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

        public DbSet<Room> Room { get; set; } = default!;
        public DbSet<Booking> Booking { get; set; } = default!;
        public DbSet<Staff> Staff { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Room>()
                .Property(r => r.HourlyRate)
                .HasPrecision(18, 2); // 18 total digits, 2 after decimal point
        }
    }
}