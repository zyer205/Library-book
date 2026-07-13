using Microsoft.AspNetCore.Mvc;

namespace LibrarySeatReservation.Web.Controllers;

public class AccountController : Controller
{
    [HttpPost]
    public IActionResult Switch(string? userName, string? returnUrl = null)
    {
        if (string.IsNullOrEmpty(userName))
            return RedirectToAction("Index", "Home");

        HttpContext.Session.SetString("UserName", userName);

        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            return Redirect(returnUrl);

        return RedirectToAction("Index", "Seats");
    }

    [HttpPost]
    public IActionResult Logout()
    {
        HttpContext.Session.Remove("UserName");
        return RedirectToAction("Index", "Home");
    }
}
