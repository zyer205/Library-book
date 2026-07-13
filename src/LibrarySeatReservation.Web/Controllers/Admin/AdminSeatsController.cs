using Microsoft.AspNetCore.Mvc;
using LibrarySeatReservation.Web.Models.Entities;
using LibrarySeatReservation.Web.Models.ViewModels;
using LibrarySeatReservation.Web.Services;

namespace LibrarySeatReservation.Web.Controllers.Admin;

[Route("Admin/Seats")]
public class AdminSeatsController : Controller
{
    private readonly ISeatService _seatService;

    public AdminSeatsController(ISeatService seatService)
    {
        _seatService = seatService;
    }

    private bool IsAdmin() => HttpContext.Session.GetInt32("IsAdmin") == 1;

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        if (!IsAdmin()) return Redirect("/Admin/Login");

        var seats = await _seatService.GetAllSeatsAsync();
        var viewModel = new AdminSeatsViewModel { Seats = seats };
        return View("~/Views/Admin/Seats.cshtml", viewModel);
    }

    [HttpPost]
    [Route("Create")]
    public async Task<IActionResult> Create(SeatCreateViewModel model)
    {
        if (!IsAdmin()) return Redirect("/Admin/Login");

        if (!ModelState.IsValid)
            return View("~/Views/Admin/Seats.cshtml", new AdminSeatsViewModel());

        var seat = new Seat
        {
            Floor = model.Floor,
            Area = model.Area,
            SeatNumber = model.SeatNumber,
            IsEnabled = true
        };

        var result = await _seatService.CreateAsync(seat);

        if (result.Success)
            TempData["Success"] = "座位已添加";
        else
            TempData["Error"] = result.ErrorMessage;

        return Redirect("/Admin/Seats");
    }

    [HttpGet]
    [Route("Edit/{id}")]
    public async Task<IActionResult> Edit(int id)
    {
        if (!IsAdmin()) return Redirect("/Admin/Login");

        var seat = await _seatService.GetByIdAsync(id);
        if (seat == null) return NotFound();

        var viewModel = new SeatEditViewModel
        {
            Id = seat.Id,
            Floor = seat.Floor,
            Area = seat.Area,
            SeatNumber = seat.SeatNumber
        };

        return View("~/Views/Admin/SeatEdit.cshtml", viewModel);
    }

    [HttpPost]
    [Route("Edit/{id}")]
    public async Task<IActionResult> Edit(int id, SeatEditViewModel model)
    {
        if (!IsAdmin()) return Redirect("/Admin/Login");

        if (!ModelState.IsValid)
            return View("~/Views/Admin/SeatEdit.cshtml", model);

        var seat = new Seat
        {
            Id = id,
            Floor = model.Floor,
            Area = model.Area,
            SeatNumber = model.SeatNumber
        };

        var result = await _seatService.UpdateAsync(seat);

        if (result.Success)
            TempData["Success"] = "座位已更新";
        else
            TempData["Error"] = result.ErrorMessage;

        return Redirect("/Admin/Seats");
    }

    [HttpPost]
    [Route("Toggle/{id}")]
    public async Task<IActionResult> Toggle(int id)
    {
        if (!IsAdmin()) return Redirect("/Admin/Login");

        var result = await _seatService.ToggleEnabledAsync(id);

        if (result.Success)
            TempData["Success"] = "座位状态已切换";
        else
            TempData["Error"] = result.ErrorMessage;

        return Redirect("/Admin/Seats");
    }
}
