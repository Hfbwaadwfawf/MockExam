namespace CityPoint.Models
{
    public class Users
    {
        public int UserId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string? PhoneNumber { get; set; }
        public bool IsActive { get; set; } = true;

        // Navigation Property
        public ICollection<Bookings>? Bookings { get; set; }
    }
}
