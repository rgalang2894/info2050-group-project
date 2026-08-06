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

        [BindProperty]
        public Booking Booking { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync()
        {
            await PopulateRoomsDropDownListAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Auto-assign current logged-in user email
            Booking.CustomerName = User.Identity?.Name ?? "Guest";

            // Remove navigation properties from validation check so ModelState doesn't fail
            ModelState.Remove("Booking.Room");
            ModelState.Remove("Booking.CustomerName");

            if (!ModelState.IsValid)
            {
                // Re-populate dropdown so user doesn't see a blank list if submission fails
                await PopulateRoomsDropDownListAsync();
                return Page();
            }

            _context.Bookings.Add(Booking);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }

        private async Task PopulateRoomsDropDownListAsync()
        {
            var rooms = await _context.Rooms.ToListAsync();
            // Display RoomNumber (or RoomType) in dropdown, value is room.Id
            ViewData["RoomId"] = new SelectList(rooms, "Id", "RoomNumber");
        }
    }
}