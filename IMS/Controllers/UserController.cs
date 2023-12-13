using IMS.Models;
using IMS.Models.User;
using IMS.Service.FileService;
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

    public JsonResult Test()
    {
        return Json("123");
    }

    public JsonResult Login([FromBody] UserAccount user)
    {
        if (user.Username == null || user.Password == null)
            return Json(new ResponseModel(StatusModel.ParameterInvalid, "账号或者密码不能为空"));

        var ls = _u.LoginUser(user.Username, user.Password);
        return Json(ls);
    }

    /// <summary>
    ///     注册界面
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    public JsonResult Register([FromBody] UserAccount user)
    {
        if (user.Username == null || user.Password == null)
            return Json(new ResponseModel(StatusModel.ParameterInvalid, "账号或者密码不能为空"));

        var rs = _u.RegisterUser(user.Username, user.Password, user.Email ?? "");
        return Json(rs);
    }

    /// <summary>
    ///     用户验证邮箱
    /// </summary>
    /// <param name="r"></param>
    /// <returns></returns>
    public JsonResult Confirm([FromBody] UserRegisterConfirmRequestModel r)
    {
        return Json(_u.ConfirmUser(r.Uid, r.CheckCode));
    }

    // 重发验证邮件
    /// <summary>
    ///     重发验证邮件
    /// </summary>
    /// <param name="r"></param>
    /// <returns></returns>
    public JsonResult ResendEmail([FromBody] UidRequestModel r)
    {
        return Json(_u.ResendEmail(r.Uid));
    }

    // 用户重设验证邮箱
    /// <summary>
    ///     用户重设验证邮箱
    /// </summary>
    /// <param name="r"></param>
    /// <returns></returns>
    public JsonResult ResetEmail([FromBody] RestEmailRequestModel r)
    {
        if (r.Email is null || r.Username is null)
            return Json(new ResponseModel(StatusModel.ParameterInvalid, "用户名或者邮箱不能为空"));

        return Json(_u.ResetEmail(r.Username, r.Email));
    }

    /// <summary>
    ///     获取用户信息
    /// </summary>
    /// <param name="r"></param>
    /// <returns></returns>
    public JsonResult GetUserInfo([FromBody] UidRequestModel r)
    {
        return Json(_u.GetUserInfo(r.Uid));
    }

    public JsonResult ResetPassword([FromBody] UidRequestModel u, [FromHeader] string authorization)
    {
        if (_u.IsAuthorization(u.Uid, authorization))
            return Json(new AuthorizationReturnModel(
                _u.ResetPassword(u.Uid)
            ));

        return Json(new AuthorizationReturnModel());
    }

    public JsonResult ResetPasswordConfirm([FromBody] ResetPasswordConfirmRequestModel u,
        [FromHeader] string authorization)
    {
        if (_u.IsAuthorization(u.Uid, authorization))
            return Json(
                _u.ResetPasswordConfirm(u.Uid, u.Password, u.CheckCode)
            );

        return Json(new ResponseModel(StatusModel.AuthorizationError, "拒绝访问"));
    }

    /// <summary>
    ///     获取OOS的校验码
    /// </summary>
    /// <returns></returns>
    public JsonResult Cos([FromBody] UidRequestModel u, [FromHeader] string authorization)
    {
        if (_u.IsAuthorization(u.Uid, authorization))
            return Json(ObjectStorageService.GetUploadRight($"UserImage/{u.Uid}.*"));
        return Json(new ResponseModel(StatusModel.AuthorizationError, "拒绝访问"));
    }

    public JsonResult CosTest()
    {
        return Json(ObjectStorageService.GetUploadRight("*"));
    }

    public JsonResult CosDownloadTest()
    {
        return Json(ObjectStorageService.GetReadRight("*"));
    }
}