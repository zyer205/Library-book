using Microsoft.AspNetCore.Mvc;
using LibrarySeatReservation.Web.Models.ViewModels;
using LibrarySeatReservation.Web.Services;

namespace LibrarySeatReservation.Web.Controllers.Admin;

[Route("Admin/Statistics")]
public class StatisticsController : Controller
{
    private readonly IReservationService _reservationService;

    public StatisticsController(IReservationService reservationService)
    {
        _reservationService = reservationService;
    }

    private bool IsAdmin() => HttpContext.Session.GetInt32("IsAdmin") == 1;

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        if (!IsAdmin()) return Redirect("/Admin/Login");

        var stats = await _reservationService.GetStatisticsAsync();

        var viewModel = new AdminStatisticsViewModel
        {
            TodayReservations = stats.TodayReservations,
            ActiveReservations = stats.ActiveReservations,
            SeatUsageRate = stats.SeatUsageRate,
            TotalReservations = stats.TotalReservations
        };

        return View("~/Views/Admin/Statistics.cshtml", viewModel);
    }
}
