namespace IMS.Models.Task;

public class TaskRequestModel
{
    public int CommandUid { get; set; } // 命令发出者
    public int Tid { get; set; } // 队伍id
    public int Uid { get; set; }
    public string? Title { get; set; }
    public string? Status { get; set; } // 项目状态
    public string? Description { get; set; }
    public DateTime? EndTime { get; set; }
}