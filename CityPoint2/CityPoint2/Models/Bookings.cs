namespace CityPoint2.Models
{
    public class Bookings
    {
        public int BookingsId { get; set; }
        public required string UserId { get; set; } // FK to AspNetUsers (string)
        public required int RoomsId { get; set; } // FK to Rooms.RoomsId (int, not string!)
        public required DateTime CheckInDate { get; set; }
        public required DateTime CheckOutDate { get; set; }
        public required int NumberOfGuests { get; set; }
        public string? SpecialRequests { get; set; }
        public required bool IsPaid { get; set; }
        public required string Status { get; set; } = "Pending";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public Rooms? Rooms { get; set; }
    }
}