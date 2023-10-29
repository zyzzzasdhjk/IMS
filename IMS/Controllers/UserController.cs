﻿using IMS.Models;
using IMS.Models.User;
using IMS.Service.UserServices;
using Microsoft.AspNetCore.Mvc;

namespace IMS.Controllers;

public class UserController : Controller
{
    private readonly IUserService _u;
    
    public UserController(IUserService u)
    {
        _u = u;
    }
    
    public JsonResult Index()
    {
        var j = new Dictionary<string, string>();
        j.Add("msg","error");
        return Json(j);
    }

    public JsonResult Login([FromBody] UserAccount user)
    {
        if (user.Username == null || user.Password == null)
        {
            return Json(new ReturnMessageModel("账号或者密码不能为空"));
        }
        LoginStatus ls = _u.LoginUser(user.Username, user.Password);
        if (ls == LoginStatus.Success)
        {
            return Json(new ReturnMessageModel());
        }
        else if (ls == LoginStatus.PasswordError)
        {
            return Json(new ReturnMessageModel("密码错误"));
        }
        else if (ls == LoginStatus.UserBanned)
        {
            return Json(new ReturnMessageModel("账号已被封禁"));
        }
        else
        {
            return Json(new ReturnMessageModel("账号不存在"));
        }
    }

    public JsonResult Register([FromBody] UserAccount user)
    {
        if (user.Username == null || user.Password == null)
        {
            return Json(new ReturnMessageModel("账号或者密码不能为空"));
        }
        RegisterStatus rs = _u.RegisterUser(user.Username, user.Password, user.Email);
        if (rs == RegisterStatus.Success)
        {
            return Json(new ReturnMessageModel(true,"请输入你获取的验证码"));
        }
        else if (rs == RegisterStatus.UsernameInvalid)
        {
            return Json(new ReturnMessageModel("用户名过长"));
        }
        else if (rs == RegisterStatus.UsernameExist)
        {
            return Json(new ReturnMessageModel("用户名已存在"));
        }
        else if(rs == RegisterStatus.EmailError)
        {
            return Json(new ReturnMessageModel("邮箱地址错误"));
        }
        else if (rs == RegisterStatus.PasswordInvalid)
        {
            return Json(new ReturnMessageModel("密码不符合要求"));
        }
        else
        {
            return Json(new ReturnMessageModel("未知错误"));
        }
    }
}