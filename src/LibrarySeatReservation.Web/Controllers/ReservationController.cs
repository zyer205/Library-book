using Microsoft.AspNetCore.Mvc;
using LibrarySeatReservation.Web.Models.Entities;
using LibrarySeatReservation.Web.Models.ViewModels;
using LibrarySeatReservation.Web.Services;

namespace LibrarySeatReservation.Web.Controllers;

public class ReservationController : Controller
{
    private readonly IReservationService _reservationService;
    private readonly ISeatService _seatService;

    public ReservationController(IReservationService reservationService, ISeatService seatService)
    {
        _reservationService = reservationService;
        _seatService = seatService;
    }

    [HttpGet]
    public async Task<IActionResult> Create(int seatId)
    {
        var userName = HttpContext.Session.GetString("UserName");
        if (string.IsNullOrEmpty(userName))
        {
            TempData["Error"] = "会话已超时，请重新选择账号登录";
            return RedirectToAction("Index", "Home");
        }

        var seat = await _seatService.GetByIdAsync(seatId);
        if (seat == null || !seat.IsEnabled)
            return NotFound();

        var today = DateTime.Today;
        var timeSlots = await _seatService.GetTimeSlotsAsync(seatId, today);

        var viewModel = new ReservationCreateViewModel
        {
            SeatId = seatId,
            SeatNumber = seat.SeatNumber,
            Floor = seat.Floor,
            Area = seat.Area,
            SelectedDate = today,
            AvailableTimeSlots = timeSlots,
            UserName = userName
        };

        return View(viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> Create(ReservationCreateViewModel model)
    {
        var userName = HttpContext.Session.GetString("UserName");
        if (string.IsNullOrEmpty(userName))
        {
            TempData["Error"] = "会话已超时，请重新选择账号登录";
            return RedirectToAction("Index", "Home");
        }

        if (!ModelState.IsValid)
        {
            model.AvailableTimeSlots = await _seatService.GetTimeSlotsAsync(model.SeatId, model.SelectedDate);
            return View(model);
        }

        var reservation = new Reservation
        {
            SeatId = model.SeatId,
            UserName = userName,
            StartTime = model.SelectedDate.Add(model.StartTimeSpan),
            EndTime = model.SelectedDate.Add(model.EndTimeSpan),
            Status = "待使用"
        };

        var result = await _reservationService.CreateAsync(reservation);

        if (!result.Success)
        {
            ModelState.AddModelError("", result.ErrorMessage ?? "");
            model.AvailableTimeSlots = await _seatService.GetTimeSlotsAsync(model.SeatId, model.SelectedDate);
            return View(model);
        }

        TempData["Success"] = "预约成功！";
        return RedirectToAction("My");
    }

    [HttpGet]
    public async Task<IActionResult> My()
    {
        var userName = HttpContext.Session.GetString("UserName");
        if (string.IsNullOrEmpty(userName))
        {
            TempData["Error"] = "会话已超时，请重新选择账号登录";
            return RedirectToAction("Index", "Home");
        }

        var reservations = await _reservationService.GetMyAsync(userName);

        var viewModel = new MyReservationsViewModel
        {
            Reservations = reservations,
            CurrentUserName = userName
        };

        return View(viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> Cancel(int id)
    {
        var userName = HttpContext.Session.GetString("UserName");
        if (string.IsNullOrEmpty(userName))
        {
            TempData["Error"] = "会话已超时，请重新选择账号登录";
            return RedirectToAction("Index", "Home");
        }

        var result = await _reservationService.CancelAsync(id, userName);

        if (!result.Success)
            TempData["Error"] = result.ErrorMessage;

        return RedirectToAction("My");
    }
}
