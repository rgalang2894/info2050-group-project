using System.Linq;
using System.Threading.Tasks;
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

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null) return NotFound();

            var booking = await _context.Bookings
                .Include(b => b.Room)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (booking == null) return NotFound();

            // Prevent Broken Access Control: User MUST own booking OR be Admin[cite: 1]
            if (!User.IsInRole("Admin") && booking.CustomerName != User.Identity?.Name)
            {
                return Forbid();
            }

            Booking = booking;

            var rooms = await _context.Rooms.ToListAsync();
            ViewData["RoomId"] = new SelectList(rooms, "Id", "RoomNumber", Booking.RoomId);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Verify record exists and ownership before allowing update
            var existingBooking = await _context.Bookings.AsNoTracking().FirstOrDefaultAsync(b => b.Id == Booking.Id);
            if (existingBooking == null) return NotFound();

            if (!User.IsInRole("Admin") && existingBooking.CustomerName != User.Identity?.Name)
            {
                return Forbid();
            }

            // Keep original customer name intact
            Booking.CustomerName = existingBooking.CustomerName;

            _context.Attach(Booking).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BookingExists(Booking.Id)) return NotFound();
                else throw;
            }

            return RedirectToPage("./Index");
        }

        private bool BookingExists(int id)
        {
            return _context.Bookings.Any(e => e.Id == id);
        }
    }
}