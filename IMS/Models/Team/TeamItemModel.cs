namespace IMS.Models.Team;

public class TeamItemModel
{
    public int Tid { get; set; } // 团队id
    public string Name { get; set; } = "";// 团队名字
    public string Description { get; set; } = "";// 团队描述
    public int Number { get; set; } // 团队人数
}