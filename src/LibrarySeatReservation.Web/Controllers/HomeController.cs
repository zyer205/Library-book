using Microsoft.AspNetCore.Mvc;
using LibrarySeatReservation.Web.Models.ViewModels;
using LibrarySeatReservation.Web.Services;

namespace LibrarySeatReservation.Web.Controllers;

public class HomeController : Controller
{
    private readonly ISeatService _seatService;
    private readonly IReservationService _reservationService;

    public HomeController(ISeatService seatService, IReservationService reservationService)
    {
        _seatService = seatService;
        _reservationService = reservationService;
    }

    public async Task<IActionResult> Index()
    {
        var userName = HttpContext.Session.GetString("UserName");

        if (string.IsNullOrEmpty(userName))
        {
            var viewModel = new HomeIndexViewModel
            {
                IsLoggedIn = false,
                Users = new List<UserCard>
                {
                    new() { Name = "学生 A", Role = "普通学生", AvatarColor = "#4A90D9" },
                    new() { Name = "学生 B", Role = "普通学生", AvatarColor = "#7B61FF" },
                    new() { Name = "学生 C", Role = "普通学生", AvatarColor = "#2ECC71" }
                }
            };
            return View(viewModel);
        }

        var totalSeats = await _seatService.GetEnabledCountAsync();
        var availableSeats = await _seatService.GetAvailableCountAsync();
        var todayReservations = await _reservationService.GetTodayCountAsync();

        var loggedInVm = new HomeIndexViewModel
        {
            IsLoggedIn = true,
            CurrentUserName = userName,
            TotalSeats = totalSeats,
            AvailableSeats = availableSeats,
            TodayReservations = todayReservations
        };
        return View(loggedInVm);
    }
}
