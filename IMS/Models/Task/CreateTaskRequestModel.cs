namespace IMS.Models.Task;

public class CreateTaskRequestModel
{
    public int CommandUid { get; set; } // 指令发出者的uid
    public string Content { get; set; } = "";
    public int Tid { get; set; }
    public string Title { get; set; } = "";
    public int Uid { get; set; }
    public DateTime? EndTime { get; set; } = null;
}