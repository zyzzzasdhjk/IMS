﻿namespace IMS.Service.UserServices;

public enum RegisterStatus
{
    UsernameInvalid, //  用户名过长
    UsernameExist, // 用户已存在
    PasswordInvalid, // 密码不合格
    EmailError, // 邮箱错误
    Success // 成功
}