using HotelManagementSystem.Data;
using HotelManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HotelManagementSystem.Pages.Rooms
{
    [Authorize(Roles = "Admin")]
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public CreateModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Room Room { get; set; } = new Room();
        public void OnGet()
        {
            Room? lastRoom = _context.Rooms.OrderByDescending(r => r.Id).FirstOrDefault();

            string newRoomNumber = "101";

            if (lastRoom != null)
            {
                int lastRoomNumber;
                if (int.TryParse(lastRoom.RoomNumber, out lastRoomNumber))
                {
                    newRoomNumber = (lastRoomNumber + 1).ToString();
                }
            }

            Room room = new Room
            {
                RoomNumber = newRoomNumber
            };
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            _context.Rooms.Add(Room);
            _context.SaveChanges();
            TempData["SuccessMessage"] = "Room created successfully!";
            return RedirectToPage("./Create");
        }
    }
}