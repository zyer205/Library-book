using LibrarySeatReservation.Web.Models.Entities;

namespace LibrarySeatReservation.Web.Models.ViewModels;

public class MyReservationsViewModel
{
    public List<Reservation> Reservations { get; set; } = new();
    public string CurrentUserName { get; set; } = "";
}
