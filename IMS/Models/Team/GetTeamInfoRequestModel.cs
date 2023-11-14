namespace IMS.Models.Team;

public class GetTeamInfoRequestModel
{
    public int Uid { get; set; }
    public int Tid { get; set; }
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
}