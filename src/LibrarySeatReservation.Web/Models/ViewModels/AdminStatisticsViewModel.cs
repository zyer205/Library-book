namespace LibrarySeatReservation.Web.Models.ViewModels;

public class AdminStatisticsViewModel
{
    public int TodayReservations { get; set; }
    public int ActiveReservations { get; set; }
    public double SeatUsageRate { get; set; }
    public int TotalReservations { get; set; }
}
