using Microsoft.AspNetCore.Mvc;
using LibrarySeatReservation.Web.Models.ViewModels;
using LibrarySeatReservation.Web.Services;

namespace LibrarySeatReservation.Web.Controllers.Admin;

[Route("Admin/Reservations")]
public class ReservationsController : Controller
{
    private readonly IReservationService _reservationService;

    public ReservationsController(IReservationService reservationService)
    {
        _reservationService = reservationService;
    }

    private bool IsAdmin() => HttpContext.Session.GetInt32("IsAdmin") == 1;

    [HttpGet]
    public async Task<IActionResult> Index(string status = null)
    {
        if (!IsAdmin()) return Redirect("/Admin/Login?timeout=1");

        var reservations = await _reservationService.GetAllAsync(status);

        var viewModel = new AdminReservationsViewModel
        {
            Reservations = reservations,
            CurrentStatus = status ?? "全部",
            StatusOptions = new List<string> { "全部", "待使用", "已完成", "已取消" }
        };

        return View("~/Views/Admin/Reservations.cshtml", viewModel);
    }

    [HttpPost]
    [Route("MarkDone/{id}")]
    public async Task<IActionResult> MarkDone(int id)
    {
        if (!IsAdmin()) return Redirect("/Admin/Login?timeout=1");

        var result = await _reservationService.MarkDoneAsync(id);

        if (result.Success)
            TempData["Success"] = "预约已标记为完成";
        else
            TempData["Error"] = result.ErrorMessage;

        return Redirect("/Admin/Reservations");
    }

    [HttpPost]
    [Route("Cancel/{id}")]
    public async Task<IActionResult> Cancel(int id)
    {
        if (!IsAdmin()) return Redirect("/Admin/Login?timeout=1");

        var result = await _reservationService.AdminCancelAsync(id);

        if (result.Success)
            TempData["Success"] = "预约已取消";
        else
            TempData["Error"] = result.ErrorMessage;

        return Redirect("/Admin/Reservations");
    }
}
