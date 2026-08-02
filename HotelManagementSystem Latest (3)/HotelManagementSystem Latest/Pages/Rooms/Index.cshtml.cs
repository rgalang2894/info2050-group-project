using HotelManagementSystem.Data;
using HotelManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Threading;

namespace HotelManagementSystem.Pages.Rooms
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }
        public List<Room> Rooms { get; private set; } = new();

        [BindProperty(SupportsGet = true)]
        public Room.RoomType? roomType { get; set; }

        public async Task OnGetAsync(CancellationToken cancellationToken = default)
        {
            IQueryable<Room> rooms = _context.Rooms.AsQueryable();

            if (roomType.HasValue && roomType.Value > 0)
            {
                rooms = rooms.Where(r => r.roomType >= roomType.Value);
            }

            // Use EF Core async APIs so queries remain parameterized and executed server-side
            Rooms = await rooms.ToListAsync(cancellationToken);

        }
    }
}
