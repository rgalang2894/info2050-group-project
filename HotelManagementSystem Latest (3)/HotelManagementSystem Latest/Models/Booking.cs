using System.ComponentModel.DataAnnotations;

namespace HotelManagementSystem.Models
{
    public class Booking
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Guest Name")]
        public string CustomerName { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Check-In Date")]
        public DateTime CheckInDate { get; set; }

        [Required]
        [Display(Name = "Check-Out Date")]
        public DateTime CheckOutDate { get; set; }

        [Required]
        [Display(Name = "Room")]
        public int RoomId { get; set; }

        public Room? Room { get; set; }
    }
}
