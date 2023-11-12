namespace IMS.Models.Team;

public class JoinTeamRequestModel
{
    public string JoinCode { get; set; } // 加入码
    public string Tid { get; set; } // 团队id
    // 这里需要判断用户输入的是哪一种加入方式
    public int Uid { get; set; }
}