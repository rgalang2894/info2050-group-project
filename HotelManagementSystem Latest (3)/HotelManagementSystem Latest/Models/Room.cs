using System.ComponentModel.DataAnnotations;

namespace HotelManagementSystem.Models
{
    public class Room
    {
        public enum RoomType { Standard, Deluxe, Suite }
        public int Id { get; set; }

        [Required(ErrorMessage = "Room number is required.")]
        [StringLength(10)]
        [Display(Name = "Room Number")]
        public string? RoomNumber { get; set; }

        [Required(ErrorMessage = "Room type is required.")]
        [Display(Name = "Room Type")]
        public RoomType roomType { get; set; }

        [Required(ErrorMessage = "Price per night is required.")]
        [Range(50, 1000)]
        public decimal PricePerNight { get; set; }

        public ICollection<Booking>? Bookings { get; set; }

        public bool IsAvailable { get; set; }
    }
}
