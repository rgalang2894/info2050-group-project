using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HotelManagementSystem.Data;
using HotelManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace HotelManagementSystem.Pages.Bookings
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Booking> Booking { get; set; } = default!;

        public async Task OnGetAsync()
        {
            var query = _context.Bookings
                .Include(b => b.Room)
                .AsQueryable();

            // Server-side authorization check: Regular users only see their own bookings[cite: 1]
            if (!User.IsInRole("Admin"))
            {
                var currentUser = User.Identity?.Name ?? string.Empty;
                query = query.Where(b => b.CustomerName == currentUser);
            }

            Booking = await query.ToListAsync();
        }
    }
}