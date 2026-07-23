using HotelManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HotelManagementSystem.Pages.Rooms
{
    [Authorize]
    public class DeleteModel : PageModel
    {
        private readonly HotelManagementSystem.Data.ApplicationDbContext _context;

        public DeleteModel(HotelManagementSystem.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Room Room { get; set; }

        public IActionResult OnGet(int? id)
        {
            Room = _context.Rooms.Find(id);

            return Room == null ? NotFound() : Page();
        }

        public IActionResult OnPost(int? id)
        {
            Room? room = _context.Rooms.Find(id);

            if (room == null)
            {
                return NotFound();
            }
            _context.Rooms.Remove(room);
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Room deleted successfully!";
            return RedirectToPage("Index");
        }
    }
}
