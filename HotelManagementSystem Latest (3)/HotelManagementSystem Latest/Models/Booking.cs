using System.ComponentModel.DataAnnotations;

namespace HotelManagementSystem.Models
{
    public class Booking : IValidatableObject
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Guest name is required.")]
        [StringLength(100, MinimumLength = 2,
            ErrorMessage = "Guest name must be between 2 and 100 characters.")]
        [Display(Name = "Guest Name")]
        public string CustomerName { get; set; } = string.Empty;

        [Display(Name = "Check-In Date")]
        [DataType(DataType.Date)]
        public DateTime CheckInDate { get; set; }

        [Display(Name = "Check-Out Date")]
        [DataType(DataType.Date)]
        public DateTime CheckOutDate { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Please select a valid room.")]
        [Display(Name = "Room")]
        public int RoomId { get; set; }

        public Room? Room { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrWhiteSpace(CustomerName))
            {
                yield return new ValidationResult(
                    "Guest name cannot contain only spaces.",
                    new[] { nameof(CustomerName) });
            }

            if (CheckInDate.Date < DateTime.Today)
            {
                yield return new ValidationResult(
                    "Check-in date cannot be in the past.",
                    new[] { nameof(CheckInDate) });
            }

            if (CheckOutDate.Date <= CheckInDate.Date)
            {
                yield return new ValidationResult(
                    "Check-out date must be after check-in date.",
                    new[] { nameof(CheckOutDate) });
            }
        }
    }
}
