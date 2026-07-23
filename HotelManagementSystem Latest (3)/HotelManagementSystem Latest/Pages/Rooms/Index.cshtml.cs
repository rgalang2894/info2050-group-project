using HotelManagementSystem.Data;
using HotelManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HotelManagementSystem.Pages.Rooms
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }
        public List<Room>? Rooms { get; private set; }

        [BindProperty(SupportsGet = true)]
        public Room.RoomType? roomType { get; set; }

        public void OnGet()
        {
            IQueryable<Room> rooms = _context.Rooms.AsQueryable();

            if (roomType.HasValue && roomType.Value > 0)
            {
                rooms = rooms.Where(r => r.roomType >= roomType.Value);
            }

            Rooms = rooms.ToList();

        }
    }
}
