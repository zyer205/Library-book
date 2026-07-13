namespace LibrarySeatReservation.Web.Models.Entities;

public class Seat
{
    public int Id { get; set; }
    public string Floor { get; set; } = "";
    public string Area { get; set; } = "";
    public string SeatNumber { get; set; } = "";
    public bool IsEnabled { get; set; } = true;
    public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
}
