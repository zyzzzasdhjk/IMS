namespace IMS.Models;

public enum AuthorizationReturnStatus
{
    Success = 0, // 成功
    NonAuthorization = 1 // 未检测到登录信息
}

public class AuthorizationReturnModel
{
    public AuthorizationReturnModel(object? m)
    {
        Code = AuthorizationReturnStatus.Success;
        Message = m;
    }

    public AuthorizationReturnModel()
    {
        Code = AuthorizationReturnStatus.NonAuthorization;
        Message = "已拒绝未知用户的访问请求";
    }

    public AuthorizationReturnStatus Code { get; set; }
    public object? Message { get; set; }
}