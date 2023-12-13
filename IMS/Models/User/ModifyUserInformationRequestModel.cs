namespace IMS.Models.User;

public class ModifyUserInformationRequestModel
{
    public int Uid { get; set; }
    public string Name { get; set; } = "";
    public string Gender { get; set; } = "";
    public DateTime Birthday { get; set; } = new DateTime();
    public string Description { get; set; } = "";
}