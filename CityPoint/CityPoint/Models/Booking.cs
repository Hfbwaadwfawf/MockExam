using Microsoft.AspNetCore.Identity;

namespace CityPoint.Models
{
    public class Booking
    {
        public int BookingId { get; set; }
        public required string UserId { get; set; } // FK to AspNetUsers
        public required int RoomId { get; set; } // FK to Rooms.RoomsId 
        public required DateTime CheckInDate { get; set; }
        public required DateTime CheckOutDate { get; set; }
        public required int NumberOfGuests { get; set; }
        public string? SpecialRequests { get; set; }
        public required bool IsPaid { get; set; }
        public required string Status { get; set; } = "Pending";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public Room? Room { get; set; }
        public IdentityUser? User { get; set; }
    }
}
