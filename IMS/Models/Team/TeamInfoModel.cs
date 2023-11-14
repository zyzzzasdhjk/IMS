namespace IMS.Models.Team;

/// <summary>
/// 团队信息模板
/// </summary>
public class TeamInfoModel
{
    public int Tid { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? JoinCOde { get; set; } // 加入码，可以自定义
    
    public string CreateTime { get; set; } // 团队创建时间
    
    public int Number { get; set; } // 团队人数
    

    public TeamInfoModel(string name,string description,int number,string createTime)
    {
        Name = name;
        Description = description;
        Number = number;
        CreateTime = createTime;
    }
}