using Microsoft.EntityFrameworkCore;
using LibrarySeatReservation.Web.Models;
using LibrarySeatReservation.Web.Models.Entities;

namespace LibrarySeatReservation.Web.Services;

public class ReservationService : IReservationService
{
    private readonly AppDbContext _context;

    public ReservationService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<(bool Success, string ErrorMessage)> CreateAsync(Reservation reservation)
    {
        if (reservation.StartTime >= reservation.EndTime)
            return (false, "结束时间必须大于开始时间");

        var seat = await _context.Seats.FindAsync(reservation.SeatId);
        if (seat == null)
            return (false, "座位不存在");
        if (!seat.IsEnabled)
            return (false, "该座位已停用，无法预约");

        var conflict = await _context.Reservations.AnyAsync(r =>
            r.SeatId == reservation.SeatId &&
            r.Status != "已取消" &&
            r.StartTime < reservation.EndTime &&
            r.EndTime > reservation.StartTime);

        if (conflict)
            return (false, "该时段已被预约，请重新选择");

        _context.Reservations.Add(reservation);
        await _context.SaveChangesAsync();
        return (true, null);
    }

    public async Task<List<Reservation>> GetMyAsync(string userName)
    {
        return await _context.Reservations
            .Where(r => r.UserName == userName)
            .Include(r => r.Seat)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
    }

    public async Task<int> GetTodayCountAsync()
    {
        var today = DateTime.Today;
        return await _context.Reservations
            .CountAsync(r => r.CreatedAt >= today && r.CreatedAt < today.AddDays(1));
    }

    public async Task<List<Reservation>> GetAllAsync(string statusFilter = null)
    {
        var query = _context.Reservations
            .Include(r => r.Seat)
            .AsQueryable();

        if (!string.IsNullOrEmpty(statusFilter) && statusFilter != "全部")
            query = query.Where(r => r.Status == statusFilter);

        return await query
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
    }

    public async Task<(bool Success, string ErrorMessage)> CancelAsync(int id, string userName)
    {
        var reservation = await _context.Reservations.FindAsync(id);
        if (reservation == null)
            return (false, "预约记录不存在");

        if (reservation.UserName != userName)
            return (false, "无权操作此预约");

        if (reservation.Status != "待使用")
            return (false, "只有'待使用'状态的预约可以取消");

        reservation.Status = "已取消";
        await _context.SaveChangesAsync();
        return (true, null);
    }

    public async Task<(bool Success, string ErrorMessage)> AdminCancelAsync(int id)
    {
        var reservation = await _context.Reservations.FindAsync(id);
        if (reservation == null)
            return (false, "预约记录不存在");

        if (reservation.Status != "待使用")
            return (false, "只有'待使用'状态的预约可以取消");

        reservation.Status = "已取消";
        await _context.SaveChangesAsync();
        return (true, null);
    }

    public async Task<(bool Success, string ErrorMessage)> MarkDoneAsync(int id)
    {
        var reservation = await _context.Reservations.FindAsync(id);
        if (reservation == null)
            return (false, "预约记录不存在");

        if (reservation.Status != "待使用")
            return (false, "只有'待使用'状态的预约可以标记完成");

        reservation.Status = "已完成";
        await _context.SaveChangesAsync();
        return (true, null);
    }

    public async Task<(int TodayReservations, int ActiveReservations, double SeatUsageRate, int TotalReservations)> GetStatisticsAsync()
    {
        var today = DateTime.Today;
        var todayCount = await _context.Reservations
            .CountAsync(r => r.CreatedAt >= today && r.CreatedAt < today.AddDays(1));

        var activeCount = await _context.Reservations
            .CountAsync(r => r.Status == "待使用");

        var totalCount = await _context.Reservations.CountAsync();

        var enabledSeats = await _context.Seats.CountAsync(s => s.IsEnabled);

        var todayReservations = await _context.Reservations
            .Where(r => r.Status != "已取消"
                     && r.StartTime >= today
                     && r.StartTime < today.AddDays(1))
            .ToListAsync();

        int todaySlots = 0;
        foreach (var r in todayReservations)
        {
            var hours = (r.EndTime - r.StartTime).TotalHours;
            todaySlots += (int)Math.Ceiling(hours);
        }

        double usageRate = enabledSeats > 0
            ? Math.Round((double)todaySlots / (enabledSeats * 14) * 100, 1)
            : 0;

        return (todayCount, activeCount, usageRate, totalCount);
    }
}
