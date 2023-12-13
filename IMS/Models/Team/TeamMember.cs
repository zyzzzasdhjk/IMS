namespace IMS.Models.Team;

public class TeamMember
{
    public string Name { get; set; } = "";
    public string Role { get; set; } = "";
    public DateTime JoinTime { get; set; } = new();
}