namespace IMS.Models;

public class ResponseModel
{
    public StatusModel Status { get; set; }
    public string Message { get; set; } = "";
    public object Data { get; set; } = "";

    public ResponseModel(StatusModel s, string str)
    {
        Status = s;
        Message = str;
    }
    
    public ResponseModel(StatusModel s, string str,object d)
    {
        Status = s;
        Message = str;
        Data = d;
    }

    public ResponseModel(StatusModel s)
    {
        Status = s;
    }
}