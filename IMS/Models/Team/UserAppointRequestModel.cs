namespace IMS.Models.Team;

public class UserAppointRequestModel
{
    public int CommandUid { get; set; }
    public int Tid { get; set; }
    public int Uid { get; set; }
    public string Role { get; set; } = "";
}