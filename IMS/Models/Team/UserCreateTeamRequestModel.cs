namespace IMS.Models.Team;

// 创建团队的请求模型
public class UserCreateTeamRequestModel
{
    public int Uid { get; set; } = -1;
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public string JoinCode { get; set; } = "";
}