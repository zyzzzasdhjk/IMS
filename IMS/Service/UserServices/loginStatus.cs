namespace IMS.Service.UserServices;

public enum LoginStatus
{
    UserNameError, // 账号不存在
    PasswordError, // 密码错误
    UserBanned, // 账号被封
    Success, // 成功
    
}