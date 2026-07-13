namespace LibrarySeatReservation.Web.Models.ViewModels;

public class SeatsIndexViewModel
{
    public List<SeatDisplayVM> Seats { get; set; } = new();
    public string CurrentFloor { get; set; } = "";
    public List<string> Floors { get; set; } = new();
    public bool IsGuest { get; set; }
    public string CurrentUserName { get; set; } = "";
}

public class SeatDisplayVM
{
    public int Id { get; set; }
    public string Floor { get; set; } = "";
    public string Area { get; set; } = "";
    public string SeatNumber { get; set; } = "";
    public bool IsEnabled { get; set; }
    public bool IsOccupied { get; set; }
}
