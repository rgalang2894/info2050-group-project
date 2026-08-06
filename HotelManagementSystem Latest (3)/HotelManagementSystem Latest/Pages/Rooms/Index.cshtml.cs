using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HotelManagementSystem.Data;
using HotelManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace HotelManagementSystem.Pages.Rooms
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Room> Rooms { get; set; } = new List<Room>();

        [BindProperty(SupportsGet = true)]
        public string? SearchType { get; set; }

        public async Task OnGetAsync()
        {
            var rawRooms = await _context.Rooms.ToListAsync();

            if (!string.IsNullOrWhiteSpace(SearchType))
            {
                // Accessing via string formatting avoids direct enum name syntax conflicts in Rider
                rawRooms = rawRooms.Where(r =>
                    r.roomType.ToString().Contains(SearchType!, StringComparison.OrdinalIgnoreCase)
                ).ToList();
            }

            Rooms = rawRooms;
        }
    }
}