using HotelManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace HotelManagementSystem.Pages.Bookings
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly HotelManagementSystem.Data.ApplicationDbContext _context;

        public IndexModel(HotelManagementSystem.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Booking> Bookings { get; set; } = default!;

        [BindProperty(SupportsGet = true)]
        public string? FilterByRoomId { get; set; }

        public SelectList RoomList { get; set; } = default!;
        public async Task OnGetAsync()
        {
            List<Room> rooms = await _context.Rooms
                .OrderBy(r => r.RoomNumber)
                .ToListAsync();

            RoomList = new SelectList(rooms, "Id", "RoomNumber");

            IQueryable<Booking> booking = _context.Bookings
                .Include(b => b.Room)
                .AsQueryable();

            if (!string.IsNullOrEmpty(FilterByRoomId) && int.TryParse(FilterByRoomId, out int roomId))
            {
                // Compare numeric RoomId directly to keep the expression translatable to SQL
                booking = booking.Where(b => b.RoomId == roomId);
            }

            Bookings = await booking.ToListAsync();
        }
    }
}
