using IMS.Models;

namespace IMS.Service.UserServices;

public interface IUserService
{
    /// <summary>
    /// 插入用户信息
    /// </summary>
    /// <param name="username"></param>
    /// <param name="password"></param>
    /// <returns></returns>
    public RegisterStatus RegisterUser(string username, string password, string email);

    /// <summary>
    /// 用户登录
    /// </summary>
    /// <param name="username"></param>
    /// <param name="password"></param>
    /// <returns></returns>
    public LoginStatus LoginUser(string username, string password);

    /// <summary>
    /// 删除用户
    /// </summary>
    /// <param name="username"></param>
    /// <param name="password"></param>
    /// <returns></returns>
    public bool DeleteUser(string username, string password);

    /// <summary>
    /// 校验用户的验证码
    /// </summary>
    /// <param name="checkCode"></param>
    /// <returns></returns>
    public ReturnMessageModel ConfirmUser(string username ,int checkCode);

    /// <summary>
    /// 重新发送验证邮件
    /// </summary>
    /// <param name="username"></param>
    /// <param name="email"></param>
    /// <returns></returns>
    public ReturnMessageModel ResendEmail(string username, string email);
    
    /// <summary>
    /// 修改用户密码
    /// </summary>
    /// <param name="uid"></param>
    /// <param name="password"></param>
    /// <returns></returns>
    public ReturnMessageModel ResetPassword(string uid, string password);
}