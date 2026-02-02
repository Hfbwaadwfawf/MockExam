using System;

namespace CityPoint.Models
{
    public class Bookings
    {
        public int BookingId { get; set; }

        // FK to Users.UserId
        public int UserId { get; set; }

        // FK to Rooms.RoomId
        public int RoomId { get; set; }

        public string CustomerName { get; set; } = string.Empty;
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public int NumberOfGuests { get; set; }
        public string? SpecialRequests { get; set; }
        public bool Paid { get; set; }
        public decimal TotalPrice { get; set; }
        public string? PaymentMethod { get; set; }
        public string Status { get; set; } = "Pending";
        public string? ConfirmationNumber { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public Rooms? Rooms { get; set; }
        public Users? Users { get; set; }
    }
}
