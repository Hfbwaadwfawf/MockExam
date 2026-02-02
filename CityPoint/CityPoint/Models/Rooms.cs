using System.Collections.Generic;

namespace CityPoint.Models
{
    public class Rooms
    {
        public int RoomId { get; set; }
        public string RoomName { get; set; }
        public decimal Rate { get; set; }
        public string Location { get; set; }
        public bool IsAvailable { get; set; }
        public int Capacity { get; set; }
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }

        // Navigation Property
        public ICollection<Bookings>? Bookings { get; set; }
    }
}
