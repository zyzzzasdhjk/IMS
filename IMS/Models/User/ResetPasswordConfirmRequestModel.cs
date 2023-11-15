namespace IMS.Models.User;

public class ResetPasswordConfirmRequestModel
{
    public int Uid { get; set; }
    public string CheckCode { get; set; } = "";
    public string Password { get; set; } = "";
}