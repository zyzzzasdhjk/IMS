namespace IMS.Models.Task;

public class AssignTaskMembersRequestModel
{
    public int CommandUid { get; set; } // 指令发出者的uid
    public int[] Uids { get; set; } = new []{0};
    public int Tid { get; set; }
}