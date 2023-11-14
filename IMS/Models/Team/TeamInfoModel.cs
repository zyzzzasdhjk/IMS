namespace IMS.Models.Team;

/// <summary>
/// 团队信息
/// </summary>
public class TeamInfoModel
{
    public int Tid { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? JoinCOde { get; set; } // 加入码，可以自定义
}