using IMS.Service.TeamServices;

namespace IMS.Controllers;
using Microsoft.AspNetCore.Mvc;

public class TeamController : Controller
{
    private ITeamSqlService _t;

    public TeamController(ITeamSqlService t)
    {
        _t = t;
    }
    
    public JsonResult Index()
    {
        var j = new Dictionary<string, string>();
        j.Add("msg","team");
        return Json(j);
    }

    // 获得用户所有的团队
    public JsonResult GetUserTeams(Object user)
    {
        return Json(_t.GetAllMembers(Convert.ToInt32(user)));
    }

    public bool AddTeamMember(Object user, Object team)
    {
        return _t.AddMember(Convert.ToInt32(user), Convert.ToInt32(team));
    }
}