using System.ComponentModel.DataAnnotations;

namespace LibrarySeatReservation.Web.Models.ViewModels;

public class SeatCreateViewModel
{
    [Required(ErrorMessage = "请填写楼层")]
    public string Floor { get; set; } = "";

    [Required(ErrorMessage = "请填写区域")]
    public string Area { get; set; } = "";

    [Required(ErrorMessage = "请填写座位编号")]
    public string SeatNumber { get; set; } = "";
}

public class SeatEditViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "请填写楼层")]
    public string Floor { get; set; } = "";

    [Required(ErrorMessage = "请填写区域")]
    public string Area { get; set; } = "";

    [Required(ErrorMessage = "请填写座位编号")]
    public string SeatNumber { get; set; } = "";
}
