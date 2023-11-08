namespace IMS.Models;

public enum AuthorizationReturnStatus
{
    Success = 0, // 成功
    NonAuthorization = 1, // 未检测到登录信息
}

public class AuthorizationReturnModel
{
    AuthorizationReturnStatus Code { get; set; } = AuthorizationReturnStatus.Success;
    public object? Message { get; set; }

    AuthorizationReturnModel(AuthorizationReturnStatus s, object? m)
    {
        Code = s;
        Message = m;
    }
}