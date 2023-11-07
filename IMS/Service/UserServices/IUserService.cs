using IMS.Models;
using IMS.Models.User;

namespace IMS.Service.UserServices;

public interface IUserService
{
    /// <summary>
    /// 插入用户信息
    /// </summary>
    /// <param name="username"></param>
    /// <param name="password"></param>
    /// <returns></returns>
    public UserRegisterReturnModel RegisterUser(string username, string password, string email);

    /// <summary>
    /// 用户登录
    /// </summary>
    /// <param name="username"></param>
    /// <param name="password"></param>
    /// <returns></returns>
    public UserLoginReturnModel LoginUser(string username, string password);

    /// <summary>
    /// 删除用户
    /// </summary>
    /// <param name="username"></param>
    /// <param name="password"></param>
    /// <returns></returns>
    public bool DeleteUser(string username, string password);

    /// <summary>
    /// 验证用户邮箱
    /// </summary>
    /// <param name="uid"></param>
    /// <param name="checkCode"></param>
    /// <returns></returns>
    public UserConfirmReturnModel ConfirmUser(int uid ,int checkCode);

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