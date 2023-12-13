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

    public ResponseModel CreateTask(int uid, int tid, string title, string content)
    {
        const string sql = "INSERT INTO task (uid, tid, title, content) VALUES (@uid, @tid, @title, @content)";
        using var sqlCommand = _r.GetConnection().CreateCommand();
        sqlCommand.CommandText = sql;
        sqlCommand.Parameters.AddWithValue("@uid", uid);
        sqlCommand.Parameters.AddWithValue("@tid", tid);
        sqlCommand.Parameters.AddWithValue("@title", title);
        sqlCommand.Parameters.AddWithValue("@content", content);
        try
        {
            return sqlCommand.ExecuteNonQuery() == 1
                ? new ResponseModel(StatusModel.Success, "创建任务成功")
                : new ResponseModel(StatusModel.Unknown, "创建任务失败");
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

    public ResponseModel AssignTask(int tid, List<int> uidList)
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
    public ResponseModel GetTeamInfo(int tid)
    {
        var sql = "SELECT taskId, name, description, status, created_at, end_at, name ,masterId " +
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