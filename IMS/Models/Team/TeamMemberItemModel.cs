namespace IMS.Models.Team;

public class TeamMemberItemModel
{
    public TeamMemberItemModel(int uid, string name, string role)
    {
        Name = name;
        Uid = uid;
        Role = role;
    }

    public string Name { get; set; }
    public int Uid { get; set; }
    public string Role { get; set; }
}