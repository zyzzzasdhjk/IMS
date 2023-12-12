namespace IMS.Models.Task;

public class TaskInfoModel
{
    public int TaskId { get; set; }
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public string Status { get; set; } = "";
    public DateTime CreateTime { get; set; }
    public DateTime EndTime { get; set; }
    public string Master { get; set; } = ""; // 任务主管人员的名字
    public int MasterUid { get; set; } // 任务主管人员的uid
    // public List<MemberInfoModel> Members { get; set; } = new List<MemberInfoModel>();

    public TaskInfoModel(
        int taskId, string name, 
        string description, string status, 
        DateTime createTime,DateTime endTime,
        string master,int masterUid)
    {
        TaskId = taskId;
        Name = name;
        Description = description;
        Status = status;
        CreateTime = createTime;
        EndTime = endTime;
        Master = master;
        MasterUid = masterUid;
    }
}