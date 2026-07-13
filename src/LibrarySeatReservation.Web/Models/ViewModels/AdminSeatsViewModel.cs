using LibrarySeatReservation.Web.Models.Entities;

namespace LibrarySeatReservation.Web.Models.ViewModels;

public class AdminSeatsViewModel
{
    public List<Seat> Seats { get; set; } = new();
}
