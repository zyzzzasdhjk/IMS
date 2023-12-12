namespace IMS.Models.Task;

public class CreateTaskRequestModel
{
    public int CommandUid; // 指令发出者的uid
    public int Uid;
    public int Tid;
    public string Title = "";
    public string Content = "";
}