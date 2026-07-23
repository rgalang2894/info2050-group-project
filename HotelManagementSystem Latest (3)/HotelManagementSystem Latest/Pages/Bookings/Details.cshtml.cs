using HotelManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace HotelManagementSystem.Pages.Bookings
{
    [Authorize]
    public class DetailsModel : PageModel
    {
        private readonly HotelManagementSystem.Data.ApplicationDbContext _context;
        public DetailsModel(HotelManagementSystem.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public Booking Booking { get; set; } = default!;
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Booking? booking = await _context.Bookings
                .Include(b => b.Room)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (booking == null)
            {
                return NotFound();
            }

            Booking = booking;
            return Page();
        }
    }
}
