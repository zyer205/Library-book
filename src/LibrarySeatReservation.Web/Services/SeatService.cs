using Microsoft.EntityFrameworkCore;
using LibrarySeatReservation.Web.Models;
using LibrarySeatReservation.Web.Models.Entities;
using LibrarySeatReservation.Web.Models.ViewModels;

namespace LibrarySeatReservation.Web.Services;

public class SeatService : ISeatService
{
    private readonly AppDbContext _context;

    public SeatService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<int> GetEnabledCountAsync()
    {
        return await _context.Seats.CountAsync(s => s.IsEnabled);
    }

    public async Task<int> GetAvailableCountAsync()
    {
        var total = await _context.Seats.CountAsync(s => s.IsEnabled);
        var now = DateTime.Now;
        var occupied = await _context.Reservations
            .Where(r => r.Status == "待使用"
                     && r.StartTime <= now
                     && r.EndTime > now)
            .Select(r => r.SeatId)
            .Distinct()
            .CountAsync();
        return total - occupied;
    }

    public async Task<List<string>> GetDistinctFloorsAsync()
    {
        return await _context.Seats
            .Where(s => s.IsEnabled)
            .Select(s => s.Floor)
            .Distinct()
            .OrderBy(f => f)
            .ToListAsync();
    }

    public async Task<List<Seat>> GetAllAsync(string floor = null)
    {
        var query = _context.Seats.Where(s => s.IsEnabled);
        if (!string.IsNullOrEmpty(floor))
            query = query.Where(s => s.Floor == floor);
        return await query.OrderBy(s => s.SeatNumber).ToListAsync();
    }

    public async Task<bool> IsOccupiedNowAsync(int seatId)
    {
        var now = DateTime.Now;
        return await _context.Reservations.AnyAsync(r =>
            r.SeatId == seatId &&
            r.Status == "待使用" &&
            r.StartTime <= now &&
            r.EndTime > now);
    }

    public async Task<Seat?> GetByIdAsync(int id)
    {
        return await _context.Seats.FindAsync(id);
    }

    public async Task<List<TimeSlotVM>> GetTimeSlotsAsync(int seatId, DateTime date)
    {
        var reservations = await _context.Reservations
            .Where(r => r.SeatId == seatId
                     && r.StartTime >= date
                     && r.StartTime < date.AddDays(1)
                     && r.Status != "已取消")
            .ToListAsync();

        var slots = new List<TimeSlotVM>();
        for (int hour = 8; hour < 22; hour++)
        {
            var slotStart = date.AddHours(hour);
            var slotEnd = date.AddHours(hour + 1);

            var isOccupied = reservations.Any(r =>
                r.StartTime < slotEnd &&
                r.EndTime > slotStart);

            slots.Add(new TimeSlotVM
            {
                StartTime = slotStart,
                EndTime = slotEnd,
                Label = $"{hour}:00-{hour + 1}:00",
                IsOccupied = isOccupied
            });
        }
        return slots;
    }

    public async Task<List<Seat>> GetAllSeatsAsync()
    {
        return await _context.Seats
            .OrderBy(s => s.Floor)
            .ThenBy(s => s.SeatNumber)
            .ToListAsync();
    }

    public async Task<(bool Success, string ErrorMessage)> CreateAsync(Seat seat)
    {
        try
        {
            _context.Seats.Add(seat);
            await _context.SaveChangesAsync();
            return (true, null);
        }
        catch (DbUpdateException)
        {
            return (false, "座位编号已存在");
        }
    }

    public async Task<(bool Success, string ErrorMessage)> UpdateAsync(Seat seat)
    {
        try
        {
            var existing = await _context.Seats.FindAsync(seat.Id);
            if (existing == null)
                return (false, "座位不存在");

            existing.Floor = seat.Floor;
            existing.Area = seat.Area;
            existing.SeatNumber = seat.SeatNumber;
            await _context.SaveChangesAsync();
            return (true, null);
        }
        catch (DbUpdateException)
        {
            return (false, "座位编号已存在");
        }
    }

    public async Task<(bool Success, string ErrorMessage)> ToggleEnabledAsync(int id)
    {
        var seat = await _context.Seats.FindAsync(id);
        if (seat == null)
            return (false, "座位不存在");

        seat.IsEnabled = !seat.IsEnabled;
        await _context.SaveChangesAsync();
        return (true, null);
    }
}
