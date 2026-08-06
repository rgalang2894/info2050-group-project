using System;
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
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public CreateModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var rooms = await _context.Rooms.ToListAsync();
            ViewData["RoomId"] = new SelectList(rooms, "Id", "RoomNumber");
            return Page();
        }

        [BindProperty]
        public Booking Booking { get; set; } = default!;

        public async Task<IActionResult> OnPostAsync()
        {
            // Automatically assign the logged-in user's email as CustomerName
            Booking.CustomerName = User.Identity?.Name ?? string.Empty;

            if (string.IsNullOrEmpty(Booking.CustomerName))
            {
                ModelState.AddModelError(string.Empty, "Unable to determine user identity.");
                var rooms = await _context.Rooms.ToListAsync();
                ViewData["RoomId"] = new SelectList(rooms, "Id", "RoomNumber");
                return Page();
            }

            _context.Bookings.Add(Booking);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}