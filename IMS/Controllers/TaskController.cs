using IMS_API;
using IMS.Models;
using IMS.Models.Task;
using IMS.Service.TaskService;
using IMS.Service.UserServices;
using Microsoft.AspNetCore.Mvc;

namespace IMS.Controllers;

public class TaskController : Controller
{
    private readonly ITaskSqlService _t;
    private readonly IUserService _u;

    public TaskController(ITaskSqlService t, IUserService u)
    {
        _t = t;
        _u = u;
    }


    public JsonResult Index([FromBody] SubmitTaskFile s)
    {
        return Json("get");
    }

    /// <summary>
    ///     根据用户传入的任务id，创建一个新的任务
    /// </summary>
    /// <param name="c"></param>
    /// <param name="authorization"></param>
    /// <returns></returns>
    public JsonResult CreateTask(CreateTaskRequestModel c, [FromHeader] string authorization)
    {
        try
        {
            if (!Common.NeedAuth || _u.IsAuthorization(c.CommandUid, authorization))
                return Json(_t.CreateTask(c.Uid, c.Tid, c.Title, c.Content));
            return Json(new ResponseModel(StatusModel.AuthorizationError, "禁止未知用户执行次操作"));
        }
        catch (Exception e)
        {
            return Json(new ResponseModel(StatusModel.Unknown, e.Message));
        }
    }

    // 查询团队信息
    /// <summary>
    /// 查询团队信息
    /// </summary>
    /// <param name="tid"></param>
    /// <param name="authorization"></param>
    /// <returns></returns>
    public JsonResult GetTaskInfo([FromBody] TaskIdRequestModel t, [FromHeader] string authorization)
    {
        try
        {
            return Json(_t.GetTaskInfo(t.TaskId));
        }
        catch (Exception e)
        {
            return Json(new ResponseModel(StatusModel.Unknown, e.Message));
        }
    }

    /// <summary>
    ///     为一个任务指派一个成员
    /// </summary>
    /// <param name="a"></param>
    /// <param name="authorization"></param>
    /// <returns></returns>
    public JsonResult AssignTaskMember(AssignTaskMemberRequestModel a, [FromHeader] string authorization)
    {
        try
        {
            if (!Common.NeedAuth || _u.IsAuthorization(a.CommandUid, authorization))
                return Json(_t.AssignTask(a.Tid, a.Uid));
            return Json(new ResponseModel(StatusModel.AuthorizationError, "禁止未知用户执行次操作"));
        }
        catch (Exception e)
        {
            return Json(new ResponseModel(StatusModel.Unknown, e.Message));
        }
    }

    // 为一个任务指派多个成员
    /// <summary>
    ///     为一个任务指派多个成员
    /// </summary>
    /// <param name="a"></param>
    /// <param name="authorization"></param>
    /// <returns></returns>
    public JsonResult AssignTaskMembers(AssignTaskMembersRequestModel a, [FromHeader] string authorization)
    {
        try
        {
            if (!Common.NeedAuth || _u.IsAuthorization(a.CommandUid, authorization))
                return Json(_t.AssignTask(a.Tid, a.Uid));

            return Json(new ResponseModel(StatusModel.AuthorizationError, "禁止未知用户执行次操作"));
        }
        catch (Exception e)
        {
            return Json(new ResponseModel(StatusModel.Unknown, e.Message));
        }
    }

    public class SubmitTaskFile
    {
        public int Tid { get; set; }
        public int Uid { get; set; }
        public List<string> Path { get; set; } = new();
    }

    // 查询团队的基本信息
}