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
    public RegisterStatus RegisterUser(string username, string password);

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
    /// 修改用户密码
    /// </summary>
    /// <param name="uid"></param>
    /// <param name="password"></param>
    /// <returns></returns>
    public ReturnMessageModel ResetPassword(string uid, string password);
}