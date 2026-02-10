namespace CityPoint.Models
{
    public class Room
    {
        public int RoomId { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public required decimal HourlyRate { get; set; }
        public required string Location { get; set; }
        public required bool IsAvailable { get; set; }
        // Navigation Property
        public ICollection<Booking>? Booking { get; set; }
    }
}
