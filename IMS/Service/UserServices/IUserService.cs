﻿using IMS.Models;

namespace IMS.Service.UserServices;

public interface IUserService
{
    /// <summary>
    ///  插入用户信息
    /// </summary>
    /// <param name="username"></param>
    /// <param name="password"></param>
    /// <param name="email"></param>
    /// <returns></returns>
    public ResponseModel RegisterUser(string username, string password, string email);

    /// <summary>
    /// 用户登录
    /// </summary>
    /// <param name="username"></param>
    /// <param name="password"></param>
    /// <returns></returns>
    public ResponseModel LoginUser(string username, string password);

    /// <summary>
    /// 验证用户邮箱
    /// </summary>
    /// <param name="uid"></param>
    /// <param name="checkCode"></param>
    /// <returns></returns>
    public ResponseModel ConfirmUser(int uid, int checkCode);

    /// <summary>
    /// 判断用户是否已经获得了授权
    /// </summary>
    /// <param name="uid"></param>
    /// <param name="code"></param>
    /// <returns></returns>
    public bool IsAuthorization(int uid, string code);

    /// <summary>
    /// 重新发送验证邮件
    /// </summary>
    /// <param name="uid"></param>
    /// <returns></returns>
    public ResponseModel ResendEmail(int uid);

    /// <summary>
    /// 根据uid来修改用户的预留邮箱
    /// </summary>
    /// <param name="uid"></param>
    /// <param name="email"></param>
    /// <returns></returns>
    public ResponseModel ResetEmail(string uid, string email);
    // public ResponseModel ResetEmail(string username, string email);

    /// <summary>
    /// 修改用户密码,这一不需要密码，是发送验证码
    /// </summary>
    /// <param name="uid"></param>
    /// <returns></returns>
    public ResponseModel ResetPassword(int uid);

    /// <summary>
    /// 修改用户密码验证
    /// </summary>
    /// <param name="uid"></param>
    /// <param name="newPwd"></param>
    /// <param name="checkCode"></param>
    /// <returns></returns>
    public ResponseModel ResetPasswordConfirm(int uid, string newPwd, string checkCode);

    /// <summary>
    /// 获取用户信息
    /// </summary>
    /// <param name="uid"></param>
    /// <returns></returns>
    public ResponseModel GetUserInfo(int uid);
    
    // 修改用户信息
    /// <summary>
    /// 修改用户个人信息
    /// </summary>
    /// <param name="uid"></param>
    /// <param name="name"></param>
    /// <param name="description"></param>
    /// <param name="gender"></param>
    /// <param name="birthday"></param>
    /// <returns></returns>
    public ResponseModel UpdateUserInfo(int uid, string name,string description,string gender,DateTime birthday);
}