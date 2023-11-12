namespace IMS.Models.Team;

public enum UserAppointResponseStatus 
{
    Success, // 成功
    UserNonExist, // 用户不存在
    AuthorizationLimit, // 权限不足
    UnKnown // 未知
}

public class UserAppointResponseModel
{
    public UserAppointResponseStatus Status { get; set; }
    public string Message { get; set; }

    public UserAppointResponseModel(UserAppointResponseStatus status)
    {
        switch (status)
        {
            case UserAppointResponseStatus.Success:
                Status = UserAppointResponseStatus.Success;
                Message = "任命成功";
                break;
            case UserAppointResponseStatus.UserNonExist:
                Status = UserAppointResponseStatus.UserNonExist;
                Message = "用户不存在";
                break;
            case UserAppointResponseStatus.AuthorizationLimit:
                Status = UserAppointResponseStatus.AuthorizationLimit;
                Message = "权限不足";
                break;
            default:
                Status = UserAppointResponseStatus.UnKnown;
                Message = "未知错误";
                break;
        }
    }
}