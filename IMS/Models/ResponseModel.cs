namespace IMS.Models;

public class ResponseModel
{
    public StatusModel Status { get; set; }
    public object Data { get; set; } = "";

    public ResponseModel(StatusModel s, object d)
    {
        Status = s;
        Data = d;
    }

    public ResponseModel(StatusModel s)
    {
        Status = s;
    }
}