using LibrarySeatReservation.Web.Models.Entities;

namespace LibrarySeatReservation.Web.Models.ViewModels;

public class AdminReservationsViewModel
{
    public List<Reservation> Reservations { get; set; } = new();
    public string CurrentStatus { get; set; } = "";
    public List<string> StatusOptions { get; set; } = new();
}
