using IMS.Models;
using IMS.Models.Team;
using IMS.Service.TeamServices;
using IMS.Service.UserServices;

namespace IMS.Controllers;
using Microsoft.AspNetCore.Mvc;

public class TeamController : Controller
{
    private readonly ITeamSqlService _t;
    private readonly IUserService _u;

    public TeamController(ITeamSqlService t,IUserService u)
    {
        _t = t;
        _u = u;
    }
    
    public JsonResult Index()
    {
        var j = new Dictionary<string, string>();
        j.Add("msg","team");
        return Json(j);
    }

    // 获得用户所有的团队
    public JsonResult GetUserTeams([FromBody] UidRequestModel u, [FromHeader] string authorization)
    {
        if (_u.IsAuthorization(u.Uid,authorization))
        {
            return Json(new AuthorizationReturnModel(_t.GetUserTeams(u.Uid)));
        }

        return Json(new AuthorizationReturnModel());
    }

    public JsonResult UserCreateTeam([FromBody] UserCreateTeamRequestModel u, [FromHeader] string authorization)
    {
        if (_u.IsAuthorization(u.Uid,authorization))
        {
            return Json(new AuthorizationReturnModel(
                new UserCreateTeamResponseModel(
                    _t.UserCreateTeam(u.Uid,u.Name,u.Description,u.JoinCode)
                    )));
        }
        return Json(new AuthorizationReturnModel());
    }
    
    // 任命一个用户
    /// <summary>
    /// 更改用户的身份或者是删除用户
    /// </summary>
    /// <param name="u"></param>
    /// <param name="authorization"></param>
    /// <returns></returns>
    public JsonResult UserAppoint([FromBody] UserAppointRequestModel u, [FromHeader] string authorization)
    {
        if (_u.IsAuthorization(u.Uid,authorization))
        {
            return Json(new AuthorizationReturnModel(
                new UserAppointResponseModel(
                    _t.UserAppoint(u.CommandUid,u.Uid,u.Tid,u.Role)
                )));
        }
        return Json(new AuthorizationReturnModel());
    }
    
    
}