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
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public EditModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Booking Booking { get; set; } = default!;
        public SelectList RoomOptions { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null) return NotFound();

            Booking? booking = await _context.Bookings.FirstOrDefaultAsync(m => m.Id == id);

            if (booking == null) return NotFound();

            Booking = booking;

            List<Room> rooms = await _context.Rooms.ToListAsync();
            ViewData["RoomOptions"] = new SelectList(rooms, "Id", "RoomNumber");
            return Page();
        }
        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (!ModelState.IsValid)
            {
                List<Room> rooms = await _context.Rooms.ToListAsync();
                ViewData["RoomOptions"] = new SelectList(rooms, "Id", "RoomNumber");
                return Page();
            }

            Booking? bookingToUpdate = await _context.Bookings.FindAsync(id);

            if (bookingToUpdate == null) return NotFound();
            bookingToUpdate.CustomerName = Booking.CustomerName;
            bookingToUpdate.RoomId = Booking.RoomId;
            bookingToUpdate.CheckInDate = Booking.CheckInDate;
            bookingToUpdate.CheckOutDate = Booking.CheckOutDate;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Booking updated successfully!";
            return RedirectToPage("Index");
        }
    }
}