namespace IMS.Models.Task;

public class AssignTaskMembersRequestModel
{
    public int CommandUid { get; set; } // 指令发出者的uid
    public List<int> Uid { get; set; } = new List<int>();
    public int Tid { get; set; }
}