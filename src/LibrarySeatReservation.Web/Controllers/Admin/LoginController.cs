using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LibrarySeatReservation.Web.Models.ViewModels;

namespace LibrarySeatReservation.Web.Controllers.Admin;

[Route("Admin/Login")]
public class LoginController : Controller
{
    private readonly Models.AppDbContext _context;

    public LoginController(Models.AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult Index()
    {
        if (HttpContext.Session.GetInt32("IsAdmin") == 1)
            return Redirect("/Admin/Reservations");

        return View("~/Views/Admin/Login.cshtml", new LoginViewModel());
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
            return View("~/Views/Admin/Login.cshtml", model);

        var admin = await _context.Admins
            .FirstOrDefaultAsync(a => a.Username == model.Username
                                   && a.Password == model.Password);

        if (admin == null)
        {
            ModelState.AddModelError("", "用户名或密码错误");
            return View("~/Views/Admin/Login.cshtml", model);
        }

        HttpContext.Session.SetInt32("IsAdmin", 1);
        return Redirect("/Admin/Reservations");
    }

    [HttpPost("Logout")]
    public IActionResult Logout()
    {
        HttpContext.Session.Remove("IsAdmin");
        return RedirectToAction("Index");
    }
}
