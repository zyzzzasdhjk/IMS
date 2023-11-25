using IMS.Models;
using IMS.Service.DataBase;

namespace IMS.Service.TaskService;

public class TaskMysqlService : ITaskSqlService
{
    private IRelationalDataBase _r;

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
        throw new NotImplementedException();
    }
}