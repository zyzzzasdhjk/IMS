using IMS.Models;
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
        var ls = _u.LoginUser(user.Username, user.Password);
        return Json(ls);
    }

    /// <summary>
    /// 注册界面
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    public JsonResult Register([FromBody] UserAccount user)
    {
        if (user.Username == null || user.Password == null)
        {
            return Json(new ReturnMessageModel("账号或者密码不能为空"));
        }
        var rs = _u.RegisterUser(user.Username, user.Password, user.Email);
        return Json(rs);
    }
    
    /// <summary>
    /// 用户验证邮箱
    /// </summary>
    /// <param name="r"></param>
    /// <returns></returns>
    public JsonResult Confirm([FromBody] RegisterConfirm r)
    {
        return Json(_u.ConfirmUser(r.Uid, r.CheckCode));
    }
}