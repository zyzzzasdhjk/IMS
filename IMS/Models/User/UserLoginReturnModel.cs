namespace IMS.Models.User;

public enum UserLoginReturnStatus  
{
    Success = 0, // 成功
    UsernameOrPasswordError = 1, // 账号或者密码错误
    UserUnconfirmed = 2, // 账号未激活
    UserBanned = 3, // 账号被封
    UserDataError = 4, // 账号信息异常
}

public class UserLoginReturnModel
{
    public UserLoginReturnStatus Code { get; set; } = UserLoginReturnStatus.Success;
    public string? Message { get; set; }
    public string AuthenticationCode { get; set; } = "";
    public int Uid { get; set; } = -1;

    public UserLoginReturnModel(UserLoginReturnStatus s)
    {
        switch (s)
        {
            case UserLoginReturnStatus.UsernameOrPasswordError:
                Code = UserLoginReturnStatus.UsernameOrPasswordError;
                Message = "账号或者密码错误！";
                break;
            case UserLoginReturnStatus.UserBanned:
                Code = UserLoginReturnStatus.UserBanned;
                Message = "账号被封禁！";
                break;
            case UserLoginReturnStatus.UserDataError:
                Code = UserLoginReturnStatus.UserDataError;
                Message = "账号信息异常！";
                break;
            case UserLoginReturnStatus.UserUnconfirmed:
                Code = UserLoginReturnStatus.UserUnconfirmed;
                Message = "账号未激活！";
                break;
        }
    }

    public UserLoginReturnModel(int uid,string authenticationCode)
    {
        Message = "登录成功！";
        AuthenticationCode = authenticationCode;
        Uid = uid;
    }
}