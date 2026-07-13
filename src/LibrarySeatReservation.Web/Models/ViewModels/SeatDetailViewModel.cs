namespace LibrarySeatReservation.Web.Models.ViewModels;

public class SeatDetailViewModel
{
    public int Id { get; set; }
    public string Floor { get; set; } = "";
    public string Area { get; set; } = "";
    public string SeatNumber { get; set; } = "";
    public bool IsEnabled { get; set; }
    public List<TimeSlotVM> TimeSlots { get; set; } = new();
    public string CurrentUserName { get; set; } = "";
}

public class TimeSlotVM
{
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string Label { get; set; } = "";
    public bool IsOccupied { get; set; }
}
