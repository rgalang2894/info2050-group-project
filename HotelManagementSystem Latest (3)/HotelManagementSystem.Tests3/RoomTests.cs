using Xunit;
using Microsoft.EntityFrameworkCore;
using HotelManagementSystem.Data;
using HotelManagementSystem.Models;
using System.Threading.Tasks;
using System.Linq;

public class RoomTests
{
    [Fact]
    public async Task AddRoom_ShouldStoreRoomInDatabase()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "RoomTestDb")
            .Options;

        using var context = new ApplicationDbContext(options);

        var room = new Room
        {
            RoomNumber = "101",
            roomType = Room.RoomType.Standard,
            PricePerNight = 120,
            IsAvailable = true
        };

        // Act
        context.Rooms.Add(room);
        await context.SaveChangesAsync();

        var rooms = await context.Rooms.ToListAsync();

        // Assert
        Assert.Single(rooms);
        Assert.Equal("101", rooms[0].RoomNumber);
    }
}