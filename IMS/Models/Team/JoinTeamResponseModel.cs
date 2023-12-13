namespace IMS.Models.Team;

public class JoinTeamResponseModel
{
    public JoinTeamResponseModel(CommonStatus s)
    {
        switch (s)
        {
            case CommonStatus.Success:
                Status = s;
                Message = "加入成功";
                break;
            case CommonStatus.Error:
                Status = s;
                Message = "加入失败";
                break;
            case CommonStatus.NonExist:
                Status = s;
                Message = "不存在该他团队";
                break;
            default:
                Status = CommonStatus.Error;
                Message = "加入失败";
                break;
        }
    }

    public JoinTeamResponseModel(string msg)
    {
        Status = CommonStatus.Unknown;
        Message = msg;
    }

    public CommonStatus Status { get; set; }
    public string Message { get; set; }
}