namespace CityPoint2.Models
{
    public class Rooms
    {
        public int RoomsId { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public required float HourlyRate { get; set; }
        public required string Location { get; set; }
        public required bool IsAvailable { get; set; }
        // Navigation Property
        public ICollection<Bookings>? Bookings { get; set; }
    }
}
