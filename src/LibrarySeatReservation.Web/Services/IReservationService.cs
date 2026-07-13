using LibrarySeatReservation.Web.Models.Entities;

namespace LibrarySeatReservation.Web.Services;

public interface IReservationService
{
    Task<(bool Success, string? ErrorMessage)> CreateAsync(Reservation reservation);
    Task<List<Reservation>> GetMyAsync(string userName);
    Task<int> GetTodayCountAsync();
    Task<List<Reservation>> GetAllAsync(string? statusFilter = null);
    Task<(bool Success, string? ErrorMessage)> CancelAsync(int id, string userName);
    Task<(bool Success, string? ErrorMessage)> AdminCancelAsync(int id);
    Task<(bool Success, string? ErrorMessage)> MarkDoneAsync(int id);
    Task<(int TodayReservations, int ActiveReservations, double SeatUsageRate, int TotalReservations)> GetStatisticsAsync();
}
