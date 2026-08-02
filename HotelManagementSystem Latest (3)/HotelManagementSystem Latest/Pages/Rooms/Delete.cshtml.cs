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
        private readonly ILogger<DeleteModel> _logger;

        public DeleteModel(
            HotelManagementSystem.Data.ApplicationDbContext context,
            ILogger<DeleteModel> logger)
        {
            _context = context;
            _logger = logger;
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
            string userName = User.Identity?.Name ?? "unknown";

            if (id == null)
            {
                _logger.LogWarning(
                    "Security event: Room deletion rejected because no RoomId was provided. UserName={UserName}",
                    userName);
                return NotFound();
            }

            Room? room = _context.Rooms.Find(id);

            if (room == null)
            {
                _logger.LogWarning(
                    "Security event: Room deletion rejected because the room was not found. UserName={UserName}, RoomId={RoomId}",
                    userName,
                    id);
                return NotFound();
            }

            int roomId = room.Id;
            string roomNumber = room.RoomNumber ?? "unknown";

            _context.Rooms.Remove(room);
            _context.SaveChanges();

            _logger.LogInformation(
                "Security event: Room deleted. UserName={UserName}, RoomId={RoomId}, RoomNumber={RoomNumber}",
                userName,
                roomId,
                roomNumber);

            TempData["SuccessMessage"] = "Room deleted successfully!";
            return RedirectToPage("Index");
        }
    }
}
