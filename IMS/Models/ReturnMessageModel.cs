namespace IMS.Models;

public class ReturnMessageModel
{
    public bool Status { get; set; } = true;
    public string? Message { get; set; }

    public ReturnMessageModel()
    {
        Status = true;
        Message = null;
    }

    public ReturnMessageModel(string msg)
    {
        Status = false;
        Message = msg;
    }

    public ReturnMessageModel(bool s)
    {
        if (!s)
        {
            Status = false;
            Message = "未知错误";
        }
        Status = true;
        Message = null;
    }

    public ReturnMessageModel(bool s, string message)
    {
        Status = s;
        Message = message;
    }
    
    public override string ToString()
    {
        return String.Format("{0} {1}", Status, Message);
    }
}