namespace IMS.Models.User;

public class TeamMemberItemModel
{
    public string Name { get; set; }
    public int Uid { get; set; }
    public string Role { get; set; }

    public TeamMemberItemModel (int uid,string name,string role)
    {
        Name = name;
        Uid = uid;
        Role = role;
    }
}