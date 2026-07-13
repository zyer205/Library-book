using LibrarySeatReservation.Web.Models.Entities;
using LibrarySeatReservation.Web.Models.ViewModels;

namespace LibrarySeatReservation.Web.Services;

public interface ISeatService
{
    Task<int> GetEnabledCountAsync();
    Task<int> GetAvailableCountAsync();
    Task<List<string>> GetDistinctFloorsAsync();
    Task<List<Seat>> GetAllAsync(string floor = null);
    Task<List<Seat>> GetAllSeatsAsync();
    Task<bool> IsOccupiedNowAsync(int seatId);
    Task<Seat?> GetByIdAsync(int id);
    Task<List<TimeSlotVM>> GetTimeSlotsAsync(int seatId, DateTime date);
    Task<(bool Success, string ErrorMessage)> CreateAsync(Seat seat);
    Task<(bool Success, string ErrorMessage)> UpdateAsync(Seat seat);
    Task<(bool Success, string ErrorMessage)> ToggleEnabledAsync(int id);
}
