using Microsoft.EntityFrameworkCore;
using LibrarySeatReservation.Web.Models;
using LibrarySeatReservation.Web.Models.Entities;

namespace LibrarySeatReservation.Web.Data;

public static class SeedData
{
    public static async Task InitAsync(AppDbContext context)
    {
        if (await context.Admins.AnyAsync() || await context.Seats.AnyAsync())
            return;

        context.Admins.Add(new Admin
        {
            Username = "admin",
            Password = "admin123"
        });

        var seats = new List<Seat>
        {
            new() { Floor = "2 楼", Area = "自习区", SeatNumber = "ZX-01", IsEnabled = true },
            new() { Floor = "2 楼", Area = "自习区", SeatNumber = "ZX-02", IsEnabled = true },
            new() { Floor = "2 楼", Area = "自习区", SeatNumber = "ZX-03", IsEnabled = true },
            new() { Floor = "2 楼", Area = "自习区", SeatNumber = "ZX-04", IsEnabled = true },
            new() { Floor = "2 楼", Area = "自习区", SeatNumber = "ZX-05", IsEnabled = true },
            new() { Floor = "2 楼", Area = "A 区",  SeatNumber = "A-01", IsEnabled = true },
            new() { Floor = "2 楼", Area = "A 区",  SeatNumber = "A-02", IsEnabled = true },
            new() { Floor = "2 楼", Area = "A 区",  SeatNumber = "A-03", IsEnabled = true },
            new() { Floor = "2 楼", Area = "A 区",  SeatNumber = "A-04", IsEnabled = true },
            new() { Floor = "2 楼", Area = "A 区",  SeatNumber = "A-05", IsEnabled = true },
            new() { Floor = "3 楼", Area = "期刊阅览区", SeatNumber = "QK-01", IsEnabled = true },
            new() { Floor = "3 楼", Area = "期刊阅览区", SeatNumber = "QK-02", IsEnabled = true },
            new() { Floor = "3 楼", Area = "期刊阅览区", SeatNumber = "QK-03", IsEnabled = true },
            new() { Floor = "3 楼", Area = "期刊阅览区", SeatNumber = "QK-04", IsEnabled = true },
            new() { Floor = "3 楼", Area = "期刊阅览区", SeatNumber = "QK-05", IsEnabled = true },
            new() { Floor = "3 楼", Area = "B 区",  SeatNumber = "B-01", IsEnabled = true },
            new() { Floor = "3 楼", Area = "B 区",  SeatNumber = "B-02", IsEnabled = true },
            new() { Floor = "3 楼", Area = "B 区",  SeatNumber = "B-03", IsEnabled = true },
            new() { Floor = "3 楼", Area = "B 区",  SeatNumber = "B-04", IsEnabled = true },
            new() { Floor = "3 楼", Area = "B 区",  SeatNumber = "B-05", IsEnabled = true },
        };

        context.Seats.AddRange(seats);
        await context.SaveChangesAsync();
    }
}
