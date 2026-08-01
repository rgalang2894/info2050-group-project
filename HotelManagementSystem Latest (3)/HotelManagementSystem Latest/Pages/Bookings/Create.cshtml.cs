using HotelManagementSystem.Data;
using HotelManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace HotelManagementSystem.Pages.Bookings
{
    [Authorize]
    public class CreateBookingModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        public CreateBookingModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Booking Booking { get; set; } = default!;

        public SelectList RoomOptions { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync()
        {
            await LoadRoomOptionsAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await LoadRoomOptionsAsync();
                return Page();
            }

            bool validRoom = await _context.Rooms.AnyAsync(r =>
                r.Id == Booking.RoomId && r.IsAvailable);

            if (!validRoom)
            {
                ModelState.AddModelError(
                    "Booking.RoomId",
                    "The selected room does not exist or is unavailable.");
            }

            bool isOverlapping = false;

            if (validRoom)
            {
                isOverlapping = await _context.Bookings.AnyAsync(b =>
                    b.RoomId == Booking.RoomId &&
                    Booking.CheckInDate < b.CheckOutDate &&
                    Booking.CheckOutDate > b.CheckInDate);
            }

            if (isOverlapping)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "The selected room is not available for the chosen dates.");
            }

            if (!ModelState.IsValid)
            {
                await LoadRoomOptionsAsync();
                return Page();
            }

            _context.Bookings.Add(Booking);
            await _context.SaveChangesAsync();
            return RedirectToPage("./Index");
        }

        private async Task LoadRoomOptionsAsync()
        {
            List<Room> rooms = await _context.Rooms
                .Where(r => r.IsAvailable)
                .OrderBy(r => r.RoomNumber)
                .ToListAsync();

            ViewData["RoomOptions"] = new SelectList(
                rooms,
                nameof(Room.Id),
                nameof(Room.RoomNumber));
        }
    }
}
