using IMS.Models;

namespace IMS.Service.TaskService;

public interface ITaskSqlService
{
    /// <summary>
    /// 创建一个任务
    /// </summary>
    /// <returns></returns>
    public ReturnMessageModel CreateTask();
}