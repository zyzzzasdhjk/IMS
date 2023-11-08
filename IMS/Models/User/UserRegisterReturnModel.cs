namespace IMS.Models.User;

public enum UserRegisterReturnStatus
{
    Success = 0, // 成功
    UserNameRepeat = 1, // 用户名重复
    UserNameIllegality = 2, // 用户名不合格
    PassWordIllegality = 3, // 密码不合格
    EmailRepeat = 4, // 邮箱重复
    EmailIllegality = 5, // 邮箱不合格
    EmailError = 6, // 邮箱系统错误
}

public class UserRegisterReturnModel
{
    public UserRegisterReturnStatus Code { get; set; } = UserRegisterReturnStatus.Success;
    public string? Message { get; set; }
    public int Uid { get; set; }

    public UserRegisterReturnModel(UserRegisterReturnStatus s)
    {
        switch (s)
        {
            case UserRegisterReturnStatus.UserNameRepeat:
                Code = UserRegisterReturnStatus.UserNameRepeat;
                Message = "用户名重复！";
                break;
            case UserRegisterReturnStatus.UserNameIllegality:
                Code = UserRegisterReturnStatus.UserNameIllegality;
                Message = "用户名不合格！";
                break;
            case UserRegisterReturnStatus.PassWordIllegality:
                Code = UserRegisterReturnStatus.PassWordIllegality;
                Message = "密码不合格！";
                break;
            case UserRegisterReturnStatus.EmailRepeat:
                Code = UserRegisterReturnStatus.EmailRepeat;
                Message = "邮箱重复！";
                break;
            case UserRegisterReturnStatus.EmailIllegality:
                Code = UserRegisterReturnStatus.EmailIllegality;
                Message = "邮箱不合格！";
                break;
            case UserRegisterReturnStatus.EmailError:
                Code = UserRegisterReturnStatus.EmailError;
                Message = "邮箱系统错误！";
                break;
            default:
                Code = UserRegisterReturnStatus.Success;
                Message = "注册成功！";
                break;
        }
    }

    public UserRegisterReturnModel(int uid)
    {
        Uid = uid;
        Message = "注册成功！";
    }
}