using HotelManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace HotelManagementSystem.Pages.Rooms
{
    public class DetailsModel : PageModel
    {
        private readonly HotelManagementSystem.Data.ApplicationDbContext _context;

        public DetailsModel(HotelManagementSystem.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Room? Room { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            Room = await _context.Rooms.FirstOrDefaultAsync(m => m.Id == id);
            return Room == null ? NotFound() : Page();
        }
    }
}
