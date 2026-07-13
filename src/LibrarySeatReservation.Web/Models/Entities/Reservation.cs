namespace LibrarySeatReservation.Web.Models.Entities;

public class Reservation
{
    public int Id { get; set; }
    public int SeatId { get; set; }
    public string UserName { get; set; } = "";
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string Status { get; set; } = "待使用";
    public DateTime CreatedAt { get; set; }
    public Seat Seat { get; set; } = null!;
}
