using IMS.Models;
using IMS.Models.Task;
using IMS.Service.DataBase;

namespace IMS.Service.TaskService;

public class TaskMysqlService : ITaskSqlService
{
    private readonly IRelationalDataBase _r;

    public TaskMysqlService(IRelationalDataBase r)
    {
        _r = r;
    }

    public ResponseModel CreateTask(int tid, int uid, string title, string content, DateTime? endTime)
    {
        try
        {
            var result = _r.ExecuteProducerWithParameters("CreateTask",
                new Dictionary<string, object?>
                {
                    { "@ti", tid },
                    { "@ui", uid },
                    { "@n", title },
                    { "@d", content },
                    { "@t", endTime }
                })?.ToString() ?? "null";
            // 从参数的Value属性中获取返回值
            if (result == "ok") return new ResponseModel(StatusModel.Success, "ok");

            return new ResponseModel(StatusModel.ParameterInvalid, result);
        }
        catch (Exception e)
        {
            return new ResponseModel(StatusModel.Unknown, e.Message);
        }
    }


    public ResponseModel AssignTask(int tid, int uid)
    {
        try
        {
            var r = _r.ExecuteNonQueryWithParameters(
                "INSERT INTO TaskMembers (taskId, uid, role) VALUES (@tid, @uid, 'Member')",
                new Dictionary<string, object?>
                {
                    { "@tid", tid },
                    { "@uid", uid }
                }
            );
            return r == 1
                ? new ResponseModel(StatusModel.Success, "分配任务成功")
                : new ResponseModel(StatusModel.Unknown, "分配任务失败");
        }

        catch (Exception e)
        {
            return new ResponseModel(StatusModel.Unknown, e.Message);
        }
    }

    public ResponseModel AssignTask(int tid, int[] uidList)
    {
        using var connection = _r.GetConnection();
        using var transaction = connection.BeginTransaction();
        using var sqlCommand = connection.CreateCommand();
        sqlCommand.Transaction = transaction;
        try
        {
            foreach (var uid in uidList)
            {
                const string sql = "INSERT INTO TaskMembers (taskId, uid, role) VALUES (@tid, @uid, 'Member')";
                sqlCommand.CommandText = sql;
                sqlCommand.Parameters.AddWithValue("@tid", tid);
                sqlCommand.Parameters.AddWithValue("@uid", uid);
                sqlCommand.ExecuteNonQuery();
                sqlCommand.Parameters.Clear(); // 清楚参数
            }

            transaction.Commit();
            return new ResponseModel(StatusModel.Success, "ok");
        }
        catch (Exception e)
        {
            transaction.Rollback();
            return new ResponseModel(StatusModel.Unknown, e.Message);
        }
    }

    public ResponseModel GetTaskMembers(int tid)
    {
        try
        {
            var result = new List<TaskMemberInfoModel>();
            using var reader = _r.ExecuteReaderWithParameters(
                "SELECT uid, name, role, created_at FROM web.TaskMembersView WHERE taskId = @tid",
                new Dictionary<string, object?>
                {
                    { "@tid", tid }
                }
            );
            while (reader.Read())
                result.Add(new TaskMemberInfoModel
                {
                    Uid = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    Role = reader.GetString(2),
                    CreatedAt = reader.GetDateTime(3)
                }
            );
            reader.Close();
            return new ResponseModel(StatusModel.Success, "ok", result);
        }
        catch (Exception e)
        {
            return new ResponseModel(StatusModel.Unknown, e.Message);
        }
    }

    public ResponseModel CreateSubTask(int tid, int uid, string title, string content, DateTime? endTime)
    {
        try
        {
            var result = _r.ExecuteProducerWithParameters("CreateSubTask",
                new Dictionary<string, object?>
                {
                    { "@ti", tid },
                    { "@ui", uid },
                    { "@n", title },
                    { "@d", content },
                    { "@t", endTime }
                })?.ToString() ?? "null";
            // 从参数的Value属性中获取返回值
            if (result == "ok") return new ResponseModel(StatusModel.Success, "ok");

            return new ResponseModel(StatusModel.ParameterInvalid, result);
        }
        catch (Exception e)
        {
            return new ResponseModel(StatusModel.Unknown, e.Message);
        }
    }

    // 查询队伍的任务
    public ResponseModel GetTeamTasks(int tid)
    {
        try
        {
            using var reader = _r.ExecuteReaderWithParameters(
                "select taskId,name,status,MasterId,MasterName from web.TeamTasksView where Tid = @tid",
                new Dictionary<string, object?>
                {
                    { "@tid", tid }
                }
            );
            var result = new List<TaskInfoModel>();
            while (reader.Read())
                result.Add(new TaskInfoModel
                {
                    TaskId = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    Status = reader.GetString(2),
                    MasterUid = reader.GetInt32(3),
                    Master = reader.GetString(4)
                });
            return new ResponseModel(StatusModel.Success, "ok", result);
        }
        catch (Exception e)
        {
            return new ResponseModel(StatusModel.Unknown, e.Message);
        }
    }

    public ResponseModel GetTaskSubtasks(int tid)
    {
        try
        {
            using var reader = _r.ExecuteReaderWithParameters(
                "select subtaskId,name,status,MasterId,MasterName from web.TaskSubtasksView where taskId = @tid",
                new Dictionary<string, object?>
                {
                    { "@tid", tid }
                }
            );
            var result = new List<TaskInfoModel>();
            while (reader.Read())
                result.Add(new TaskInfoModel
                {
                    TaskId = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    Status = reader.GetString(2),
                    MasterUid = reader.GetInt32(3),
                    Master = reader.GetString(4)
                });
            return new ResponseModel(StatusModel.Success, "ok", result);
        }
        catch (Exception e)
        {
            return new ResponseModel(StatusModel.Unknown, e.Message);
        }
    }

    public ResponseModel SubmitTaskFile(int tid, int uid, List<string> path)
    {
        throw new NotImplementedException();
    }

    public ResponseModel SubmitResult(int tid, int uid)
    {
        throw new NotImplementedException();
    }

    // 查询任务的基本信息
    public ResponseModel GetTaskInfo(int tid)
    {
        try
        {
            var reader = _r.ExecuteReaderWithParameters(
                "SELECT taskId, name, description, status, created_at, end_at, MasterName ,masterId " +
                "FROM TaskInfoView WHERE taskId = @tid",
                new Dictionary<string, object?>
                {
                    { "@tid", tid }
                });
            if (!reader.HasRows) return new ResponseModel(StatusModel.NonExist, "任务不存在");
            // 从reader中获取信息
            reader.Read();
            var taskInfo = new TaskInfoModel
            (
                reader.GetInt32(0),
                reader.GetString(1),
                reader.GetString(2),
                reader.GetString(3),
                reader.GetDateTime(4),
                reader.GetDateTime(5),
                reader.GetString(6),
                reader.GetInt32(7)
            );
            reader.Close();
            return new ResponseModel(StatusModel.Success, "ok", taskInfo);
        }
        catch (Exception e)
        {
            return new ResponseModel(StatusModel.Unknown, e.Message);
        }
    }

    public ResponseModel UpdateTask(int tid, string name, string description, string status, DateTime? endTime)
    {
        try
        {
            var r = _r.ExecuteNonQueryWithParameters(
                "UPDATE TaskInfo SET name = @name, description = @description, status = @status, end_at = @end_at WHERE taskId = @tid",
                new Dictionary<string, object?>
                {
                    { "@tid", tid },
                    { "@name", name },
                    { "@description", description },
                    { "@status", status },
                    { "@end_at", endTime }
                }
            );

            return r == 1
                ? new ResponseModel(StatusModel.Success, "更新任务成功")
                : new ResponseModel(StatusModel.NonExist, "更新任务失败");
        }
        catch (Exception e)
        {
            return new ResponseModel(StatusModel.Unknown, e.Message);
        }
    }
}