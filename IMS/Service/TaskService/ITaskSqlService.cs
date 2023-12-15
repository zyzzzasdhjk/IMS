﻿using IMS.Models;

namespace IMS.Service.TaskService;

public interface ITaskSqlService
{
    /// <summary>
    /// 创建一个任务
    /// </summary>
    /// <param name="uid"></param>
    /// <param name="tid">创建任务所在的队伍</param>
    /// <param name="title">任务的名字</param>
    /// <param name="content">任务的内容</param>
    /// <param name="endTime">任务结束时间</param>
    /// <returns></returns>
    public ResponseModel CreateTask(int tid,  int uid, string title, string content,DateTime? endTime);

    // 获取任务的信息
    /// <summary>
    /// 获取任务的信息
    /// </summary>
    /// <param name="tid"></param>
    /// <returns></returns>
    public ResponseModel GetTaskInfo(int tid);
    
    /// <summary>
    ///     添加多个用户到任务
    /// </summary>
    /// <param name="tid"></param>
    /// <param name="uidList">包含多个用户id的数组</param>
    /// <returns></returns>
    public ResponseModel AssignTask(int tid, int[] uidList);

    /// <summary>
    ///     添加一个用户id到任务
    /// </summary>
    /// <param name="tid">任务id</param>
    /// <param name="uid"></param>
    /// <returns></returns>
    public ResponseModel AssignTask(int tid, int uid);

    // 获取任务的全部成员信息
    /// <summary>
    /// 获取任务的全部成员信息
    /// </summary>
    /// <param name="tid"></param>
    /// <returns></returns>
    public ResponseModel GetTaskMembers(int tid);
    
    // 为任务创建子任务
    /// <summary>
    /// 为任务创建子任务
    /// </summary>
    /// <param name="tid">父任务的编号</param>
    /// <param name="uid"></param>
    /// <param name="title"></param>
    /// <param name="content"></param>
    /// <param name="endTime"></param>
    /// <returns></returns>
    public ResponseModel CreateSubTask(int tid,  int uid, string title, string content,DateTime? endTime);

    public ResponseModel GetTeamTasks(int tid);
    
    /// <summary>
    ///     删除掉任务中的一个用户
    /// </summary>
    /// <param name="tid"></param>
    /// <param name="uid"></param>
    /// <returns></returns>
    public ResponseModel DeleteATaskMember(int tid, int uid);

    /// <summary>
    ///     提交任务的结果
    /// </summary>
    /// <param name="tid">任务id</param>
    /// <param name="uid">用户id</param>
    /// <returns></returns>
    public ResponseModel SubmitResult(int tid, int uid);

    /// <summary>
    ///     上传任务中所需的文件
    /// </summary>
    /// <param name="tid"></param>
    /// <param name="uid"></param>
    /// <param name="path">mongodb数据库中对应的文件id</param>
    /// <returns></returns>
    public ResponseModel SubmitTaskFile(int tid, int uid, List<string> path);
}