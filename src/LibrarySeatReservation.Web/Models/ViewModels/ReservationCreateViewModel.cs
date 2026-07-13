namespace LibrarySeatReservation.Web.Models.ViewModels;

public class ReservationCreateViewModel
{
    public int SeatId { get; set; }
    public string SeatNumber { get; set; } = "";
    public string Floor { get; set; } = "";
    public string Area { get; set; } = "";
    public DateTime SelectedDate { get; set; }
    public TimeSpan StartTimeSpan { get; set; }
    public TimeSpan EndTimeSpan { get; set; }
    public List<TimeSlotVM> AvailableTimeSlots { get; set; } = new();
    public string UserName { get; set; } = "";
}
