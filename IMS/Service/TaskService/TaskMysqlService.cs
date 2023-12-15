using System.Data;
using IMS.Models;
using IMS.Models.Task;
using IMS.Service.DataBase;
using MySql.Data.MySqlClient;

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
            using var sqlCommand = new MySqlCommand();
            sqlCommand.Connection = _r.GetConnection();
            const string sql = "CreateTask";
            sqlCommand.CommandText = sql;
            sqlCommand.CommandType = CommandType.StoredProcedure;
            sqlCommand.Parameters.AddWithValue("@ti", tid).Direction = ParameterDirection.Input;
            sqlCommand.Parameters.AddWithValue("@ui", uid).Direction = ParameterDirection.Input;
            sqlCommand.Parameters.AddWithValue("@n", title).Direction = ParameterDirection.Input;
            sqlCommand.Parameters.AddWithValue("@d", content).Direction = ParameterDirection.Input;
            sqlCommand.Parameters.AddWithValue("@t", endTime).Direction = ParameterDirection.Input;
            // 执行存储过程
            sqlCommand.Parameters.Add("@msg", MySqlDbType.Text).Direction = ParameterDirection.Output;
            // 执行存储过程
            var result = sqlCommand.ExecuteScalar().ToString() ?? "null";
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
        const string sql = "INSERT INTO TaskMembers (taskId, uid, role) VALUES (@tid, @uid, 'Member')";
        using var sqlCommand = _r.GetConnection().CreateCommand();
        sqlCommand.CommandText = sql;
        sqlCommand.Parameters.AddWithValue("@tid", tid);
        sqlCommand.Parameters.AddWithValue("@uid", uid);
        try
        {
            return sqlCommand.ExecuteNonQuery() == 1
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
        var sql = "SELECT uid, name, role, created_at FROM web.TaskMembersView WHERE taskId = @tid";
        using var sqlCommand = _r.GetConnection().CreateCommand();
        sqlCommand.CommandText = sql;
        sqlCommand.Parameters.AddWithValue("@tid", tid);
        try
        {
            var result = new List<TaskMemberInfoModel>();
            using var reader = sqlCommand.ExecuteReader();
            while (reader.Read())
                result.Add(new TaskMemberInfoModel
                {
                    Uid = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    Role = reader.GetString(2),
                    CreatedAt = reader.GetDateTime(3)
                });
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
            using var sqlCommand = new MySqlCommand();
            sqlCommand.Connection = _r.GetConnection();
            const string sql = "CreateSubTask";
            sqlCommand.CommandText = sql;
            sqlCommand.CommandType = CommandType.StoredProcedure;
            sqlCommand.Parameters.AddWithValue("@ti", tid).Direction = ParameterDirection.Input;
            sqlCommand.Parameters.AddWithValue("@ui", uid).Direction = ParameterDirection.Input;
            sqlCommand.Parameters.AddWithValue("@n", title).Direction = ParameterDirection.Input;
            sqlCommand.Parameters.AddWithValue("@d", content).Direction = ParameterDirection.Input;
            sqlCommand.Parameters.AddWithValue("@t", endTime).Direction = ParameterDirection.Input;
            // 执行存储过程
            sqlCommand.Parameters.Add("@msg", MySqlDbType.Text).Direction = ParameterDirection.Output;
            // 执行存储过程
            var result = sqlCommand.ExecuteScalar().ToString() ?? "null";
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
            const string sql = "select taskId,name,status,MasterId,MasterName from web.TeamTasksView where Tid = @tid";
            using var sqlCommand = _r.GetConnection().CreateCommand();
            sqlCommand.CommandText = sql;
            sqlCommand.Parameters.AddWithValue("@tid", tid);
            using var reader = sqlCommand.ExecuteReader();
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
            return new ResponseModel(StatusModel.Success, "ok",result);
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

    public ResponseModel DeleteATaskMember(int tid, int uid)
    {
        // 删除一个任务成员
        const string sql = "DELETE FROM TaskMembers WHERE taskId = @tid AND uid = @uid";
        using var sqlCommand = _r.GetConnection().CreateCommand();
        sqlCommand.CommandText = sql;
        sqlCommand.Parameters.AddWithValue("@tid", tid);
        sqlCommand.Parameters.AddWithValue("@uid", uid);
        try
        {
            return sqlCommand.ExecuteNonQuery() == 1
                ? new ResponseModel(StatusModel.Success, "删除成员成功")
                : new ResponseModel(StatusModel.NonExist, "删除成员失败");
        }
        catch (Exception e)
        {
            return new ResponseModel(StatusModel.Unknown, e.Message);
        }
    }

    // 删除多个任务成员
    public ResponseModel DeleteATaskMember(int tid, List<int> uidList)
    {
        using var connection = _r.GetConnection();
        using var transaction = connection.BeginTransaction();
        using var sqlCommand = connection.CreateCommand();
        sqlCommand.Transaction = transaction;
        try
        {
            foreach (var uid in uidList)
            {
                const string sql = "DELETE FROM TaskMembers WHERE taskId = @tid AND uid = @uid";
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

    // 查询任务的基本信息
    public ResponseModel GetTaskInfo(int tid)
    {
        var sql = "SELECT taskId, name, description, status, created_at, end_at, MasterName ,masterId " +
                  "FROM TaskInfoView WHERE taskId = @tid";
        using var sqlCommand = _r.GetConnection().CreateCommand();
        sqlCommand.CommandText = sql;
        sqlCommand.Parameters.AddWithValue("@tid", tid);
        try
        {
            var reader = sqlCommand.ExecuteReader();
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
}