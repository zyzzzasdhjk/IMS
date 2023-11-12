namespace IMS.Models.User;

public class UserRegisterConfirmRequestModel
{
    public int Uid { get; set; } = 0;
    public int CheckCode { get; set; } = 0;
}