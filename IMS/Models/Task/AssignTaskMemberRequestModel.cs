namespace IMS.Models.Task;

public class AssignTaskMemberRequestModel
{
    public int CommandUid { get; set; } // 指令发出者的uid
    public int Uid { get; set; }
    public int Tid { get; set; }
}