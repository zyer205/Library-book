using Microsoft.AspNetCore.Mvc;
using LibrarySeatReservation.Web.Models.ViewModels;
using LibrarySeatReservation.Web.Services;

namespace LibrarySeatReservation.Web.Controllers;

public class SeatsController : Controller
{
    private readonly ISeatService _seatService;

    public SeatsController(ISeatService seatService)
    {
        _seatService = seatService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string? floor = null)
    {
        var userName = HttpContext.Session.GetString("UserName");
        var isGuest = string.IsNullOrEmpty(userName);

        var seats = await _seatService.GetAllAsync(floor);
        var floors = await _seatService.GetDistinctFloorsAsync();
        var occupiedSeatIds = await _seatService.GetCurrentlyOccupiedSeatIdsAsync();

        var seatDisplays = seats.Select(seat => new SeatDisplayVM
        {
            Id = seat.Id,
            Floor = seat.Floor,
            Area = seat.Area,
            SeatNumber = seat.SeatNumber,
            IsEnabled = seat.IsEnabled,
            IsOccupied = occupiedSeatIds.Contains(seat.Id)
        }).ToList();

        var viewModel = new SeatsIndexViewModel
        {
            Seats = seatDisplays,
            CurrentFloor = floor ?? "",
            Floors = floors,
            IsGuest = isGuest,
            CurrentUserName = userName ?? ""
        };

        return View(viewModel);
    }

    [HttpGet]
    public async Task<IActionResult> Detail(int id)
    {
        var seat = await _seatService.GetByIdAsync(id);
        if (seat == null)
            return NotFound();

        var timeSlots = await _seatService.GetTimeSlotsAsync(id, DateTime.Today);

        var viewModel = new SeatDetailViewModel
        {
            Id = seat.Id,
            Floor = seat.Floor,
            Area = seat.Area,
            SeatNumber = seat.SeatNumber,
            IsEnabled = seat.IsEnabled,
            TimeSlots = timeSlots,
            CurrentUserName = HttpContext.Session.GetString("UserName") ?? ""
        };

        return View(viewModel);
    }
}
