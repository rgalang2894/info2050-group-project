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
            List<Room> rooms = await _context.Rooms.ToListAsync();
            ViewData["RoomOptions"] = new SelectList(rooms, "Id", "RoomNumber");
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid || _context.Bookings == null || Booking == null)
            {
                List<Room> rooms = await _context.Rooms.ToListAsync();
                ViewData["RoomOptions"] = new SelectList(rooms, "Id", "RoomNumber");
                return Page();
            }

            if (Booking.CheckInDate >= Booking.CheckOutDate)
            {
                ModelState.AddModelError(string.Empty, "Check-out date must be after check-in date.");
                List<Room> rooms = await _context.Rooms.ToListAsync();
                ViewData["RoomOptions"] = new SelectList(rooms, "Id", "RoomNumber");
                return Page();
            }

            bool isOverlapping = await _context.Bookings.AnyAsync(b =>
                b.RoomId == Booking.RoomId &&
                ((Booking.CheckInDate >= b.CheckInDate && Booking.CheckInDate < b.CheckOutDate) ||
                 (Booking.CheckOutDate > b.CheckInDate && Booking.CheckOutDate <= b.CheckOutDate) ||
                 (Booking.CheckInDate <= b.CheckInDate && Booking.CheckOutDate >= b.CheckOutDate)));

            if (isOverlapping)
            {
                ModelState.AddModelError(string.Empty, "The selected room is not available for the chosen dates.");
                List<Room> rooms = await _context.Rooms.ToListAsync();
                ViewData["RoomOptions"] = new SelectList(rooms, "Id", "RoomNumber");
                return Page();
            }

            _context.Bookings.Add(Booking);
            await _context.SaveChangesAsync();
            return RedirectToPage("./Index");
        }
    }
}
