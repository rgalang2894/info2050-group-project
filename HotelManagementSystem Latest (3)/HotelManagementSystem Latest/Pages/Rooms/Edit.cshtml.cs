using HotelManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HotelManagementSystem.Pages.Rooms
{
    [Authorize(Roles = "Admin")]
    public class EditModel : PageModel
    {
        private readonly HotelManagementSystem.Data.ApplicationDbContext _context;

        public EditModel(HotelManagementSystem.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Room? Room { get; set; }
        public IActionResult OnGet(int? id)
        {
            Room = _context.Rooms.Find(id);

            return Room == null ? NotFound() : Page();
        }

        public IActionResult OnPost(int? id)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (Room == null)
            {
                // No bound Room data - return to form with validation
                return Page();
            }

            Room? room = _context.Rooms.Find(id);
            if (room == null)
            {
                return NotFound();
            }

            // Update only when bound values are provided
            room.RoomNumber = Room.RoomNumber ?? room.RoomNumber;
            room.roomType = Room.roomType;
            room.PricePerNight = Room.PricePerNight;
            _context.SaveChanges();
            TempData["SuccessMessage"] = "Room updated successfully!";
            return RedirectToPage("Index");

        }
    }
}
