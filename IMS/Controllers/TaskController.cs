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

    /// <summary>
    /// 检查参数是否符合要求
    /// </summary>
    /// <param name="t"></param>
    /// <param name="paras"></param>
    /// <returns></returns>
    private bool ParametricTest(TaskRequestModel t, List<string> paras)
    {
        foreach (var p in paras)
        {
            if (t.GetType().GetProperty(p) == null) return false;
        }
        return true;
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
    public JsonResult CreateTask([FromBody]CreateTaskRequestModel c, [FromHeader] string authorization)
    {
        try
        {
            if (!Common.NeedAuth || _u.IsAuthorization(c.CommandUid, authorization))
                return Json(_t.CreateTask(c.Uid,c.Tid,  c.Title, c.Content,c.EndTime));
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
    /// <param name="t"></param>
    /// <param name="authorization"></param>
    /// <returns></returns>
    public JsonResult GetTaskInfo([FromBody] TaskIdRequestModel t, [FromHeader] string authorization)
    {
        try
        {
            if (t.TaskId == -1)
            {
                return Json(new ResponseModel(StatusModel.ParameterInvalid,"参数错误"));
            }
            return Json(_t.GetTaskInfo(t.TaskId));
        }
        catch (Exception e)
        {
            return Json(new ResponseModel(StatusModel.Unknown, e.Message));
        }
    }

    /// <summary>
    /// 为一个任务指派一个成员
    /// </summary>
    /// <param name="a"></param>
    /// <param name="authorization"></param>
    /// <returns></returns>
    public JsonResult AssignTaskMember([FromBody]AssignTaskMemberRequestModel a, [FromHeader] string authorization)
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
    /// 为一个任务指派多个成员
    /// </summary>
    /// <param name="a"></param>
    /// <param name="authorization"></param>
    /// <returns></returns>
    public JsonResult AssignTaskMembers([FromBody]AssignTaskMembersRequestModel a, [FromHeader] string authorization)
    {
        try
        {
            if (!Common.NeedAuth || _u.IsAuthorization(a.CommandUid, authorization))
                return Json(_t.AssignTask(a.Tid, a.Uids));

            return Json(new ResponseModel(StatusModel.AuthorizationError, "禁止未知用户执行次操作"));
        }
        catch (Exception e)
        {
            return Json(new ResponseModel(StatusModel.Unknown, e.Message));
        }
    }
    
    // 获取任务成员信息
    /// <summary>
    /// 获取任务成员信息
    /// </summary>
    /// <param name="t"></param>
    /// <param name="authorization"></param>
    /// <returns></returns>
    public JsonResult GetTaskMembers([FromBody] TaskIdRequestModel t, [FromHeader] string authorization)
    {
        try
        {
            if (t.TaskId == -1)
            {
                return Json(new ResponseModel(StatusModel.ParameterInvalid, "参数错误"));
            }

            if (!Common.NeedAuth || _u.IsAuthorization(t.TaskId, authorization))
                return Json(_t.GetTaskMembers(t.TaskId));
            return Json(new ResponseModel(StatusModel.AuthorizationError, "禁止未知用户执行次操作"));
        }
        catch (Exception e)
        {
            return Json(new ResponseModel(StatusModel.Unknown, e.Message));
        }
    }

    // 创建任务的子任务
    /// <summary>
    /// 创建任务的子任务
    /// </summary>
    /// <param name="c">和创建团队任务是通用的，只不过这个tid是父任务的id</param>
    /// <param name="authorization"></param>
    /// <returns></returns>
    public JsonResult CreateSubTask([FromBody] CreateTaskRequestModel c, [FromHeader] string authorization)
    {
        try
        {
            if (!Common.NeedAuth || _u.IsAuthorization(c.CommandUid, authorization))
                return Json(_t.CreateSubTask(c.Tid, c.Uid, c.Title, c.Content, c.EndTime));
            return Json(new ResponseModel(StatusModel.AuthorizationError, "禁止未知用户执行次操作"));
        }
        catch (Exception e)
        {
            return Json(new ResponseModel(StatusModel.Unknown, e.Message));
        }
    }
    
    // 获取队伍所属的所有任务
    /// <summary>
    /// 获取队伍所属的所有任务
    /// </summary>
    /// <param name="t"></param>
    /// <param name="authorization"></param>
    /// <returns></returns>
    public JsonResult GetTeamTasks([FromBody] TaskRequestModel t, [FromHeader] string authorization)
    {
        try
        {
            if (!Common.NeedAuth || _u.IsAuthorization(t.Uid, authorization))
                return Json(_t.GetTeamTasks(t.Tid));
            return Json(new ResponseModel(StatusModel.AuthorizationError, "禁止未知用户执行次操作"));
        }
        catch (Exception e)
        {
            return Json(new ResponseModel(StatusModel.Unknown, e.Message));
        }
    }
    
    // 获取任务的全部子任务
    /// <summary>
    /// 获取任务的全部子任务
    /// </summary>
    /// <param name="t"></param>
    /// <param name="authorization"></param>
    /// <returns></returns>
    public JsonResult GetTaskSubtasks([FromBody] TaskRequestModel t, [FromHeader] string authorization)
    {
        try
        {
            if (!Common.NeedAuth || _u.IsAuthorization(t.Uid, authorization))
                return Json(_t.GetTaskSubtasks(t.Tid));
            return Json(new ResponseModel(StatusModel.AuthorizationError, "禁止未知用户执行次操作"));
        }
        catch (Exception e)
        {
            return Json(new ResponseModel(StatusModel.Unknown, e.Message));
        }
    }
    
    // 修改任务的信息
    /// <summary>
    /// 修改任务的信息
    /// </summary>
    /// <param name="t"></param>
    /// <param name="authorization"></param>
    /// <returns></returns>
    public JsonResult UpdateTask([FromBody] TaskRequestModel t, [FromHeader] string authorization)
    {
        try
        {
            if (!Common.NeedAuth || _u.IsAuthorization(t.Uid, authorization))
                return Json(_t.UpdateTask(t.Tid, t.Title, t.Description,t.Status, t.EndTime));
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