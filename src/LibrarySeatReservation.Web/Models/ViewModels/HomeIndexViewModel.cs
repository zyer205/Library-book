namespace LibrarySeatReservation.Web.Models.ViewModels;

public class HomeIndexViewModel
{
    public bool IsLoggedIn { get; set; }
    public List<UserCard> Users { get; set; } = new();
    public int TotalSeats { get; set; }
    public int AvailableSeats { get; set; }
    public int TodayReservations { get; set; }
    public string CurrentUserName { get; set; } = "";
}

public class UserCard
{
    public string Name { get; set; } = "";
    public string Role { get; set; } = "";
    public string AvatarColor { get; set; } = "";
}
