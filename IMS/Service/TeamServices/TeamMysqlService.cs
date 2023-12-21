using IMS.Models;
using IMS.Models.Team;
using IMS.Service.DataBase;
using IMS.Service.FileService;
using MySql.Data.MySqlClient;

namespace IMS.Service.TeamServices;

public class TeamMysqlService : ITeamSqlService
{
    private readonly IRelationalDataBase _r;
    private readonly Dictionary<string, int> _userRoles = new();
    /*public readonly string[] TeamInfoColumns = { "name", "description", "joinCode", "peopleMaxNum" };*/

    public TeamMysqlService(IRelationalDataBase r)
    {
        _r = r;
        // 用户身份权限初始化
        _userRoles.Add("Creator", 3);
        _userRoles.Add("Admin", 2);
        _userRoles.Add("Member", 1);
    }

    public UserCreateTeamResponseStatus UserCreateTeam(int uid, string name, string description, string joinCode)
    {
        if (name == "" || name.Length > 20) return UserCreateTeamResponseStatus.NameIllegality; // 名字不能为空

        if (joinCode.Length > 0 && joinCode.Length != 9) return UserCreateTeamResponseStatus.JoinCodeIllegality;

        joinCode = joinCode.ToUpper(); // 全大写
        if (description == "") description = "团队的介绍暂时没有填写"; // 介绍为空时替换为默认介绍

        // 调用存储过程的方式比较特殊
        
            /*sqlCommand.Connection = _m.GetConnection();
            const string sql = "UserCreateTeam";
            sqlCommand.CommandText = sql;
            sqlCommand.CommandType = CommandType.StoredProcedure;
            sqlCommand.Parameters.AddWithValue("@n", name).Direction = ParameterDirection.Input;
            sqlCommand.Parameters.AddWithValue("@d", description).Direction = ParameterDirection.Input;
            sqlCommand.Parameters.AddWithValue("@j", joinCode).Direction = ParameterDirection.Input;
            sqlCommand.Parameters.AddWithValue("@u", uid).Direction = ParameterDirection.Input;
            sqlCommand.Parameters.Add("@msg", MySqlDbType.Int32).Direction = ParameterDirection.Output;
            // 执行存储过程
            sqlCommand.ExecuteNonQuery();*/
            // 从参数的Value属性中获取返回值
            var returnCode = _r.ExecuteProducerWithParameters(
                "UserCreateTeam",
                new Dictionary<string, object?>
                {
                    { "@n", name },
                    { "@d", description },
                    { "@j", joinCode },
                    { "@u", uid },
                }
            );
            if (returnCode?.ToString() == "repeat") return UserCreateTeamResponseStatus.JoinCodeRepeat;
            return UserCreateTeamResponseStatus.Success;
        
    }


    public ResponseModel JoinTeam(int uid, string joinCode)
    {
        try
        {
            // 先判断这个加入码的团队是否存在
            var result = _r.ExecuteScalarWithParameters(
                "select tid from web.TeamInfo  where joinCode = @joinCode",
                new Dictionary<string, object?>
                {
                    { "@joinCode", joinCode }
                }
            );
            if (result == null) return new ResponseModel(StatusModel.NonExist, "团队不存在"); // 不存在验证码当然是不存在这个团队咯

            int tid = Convert.ToInt32(result);
            
            
            // 更新数据库中用户的信息
            var row = _r.ExecuteNonQueryWithParameters(
                "insert into web.TeamMember(tid, uid, role) value (@tid, @uid, 'Member')", 
                new Dictionary<string, object?>
                {
                    { "@uid", uid },
                    { "@tid", tid }
                }
            );
            if (row != 1) return new ResponseModel(StatusModel.Unknown, "未知错误"); // 未知错误
            return new ResponseModel(StatusModel.Success, "加入团队成功");
        }
        catch (MySqlException e)
        {
            return new ResponseModel(StatusModel.Unknown, e.Message); // 返回错误信息
        }
    }

    public ResponseModel JoinTeam(int uid, int tid)
    {
        try
        {
            // 判断对应的团队是否存在
            var result = _r.ExecuteScalarWithParameters(
                "select tid from web.TeamInfo where tid = @tid",
                new Dictionary<string, object?>
                {
                    { "@tid", tid }
                }
            );
            if (result is null) return new ResponseModel(StatusModel.NonExist, "团队不存在");

            // 更新数据库中用户的信息
            var i = _r.ExecuteNonQueryWithParameters(
                "insert into web.TeamMember(tid, uid, role) value (@tid, @uid, 'Member')",
                new Dictionary<string, object?>
                {
                    { "@tid", tid },
                    { "@uid", uid }
                }
            );
            if (i != 1) return new ResponseModel(StatusModel.Unknown, "未知错误");// 未知错误
            return new ResponseModel(StatusModel.Success, "加入团队成功");
        }
        catch (Exception e)
        {
            return new ResponseModel(StatusModel.Unknown, e.Message); // 返回错误信息
        }
    }

    public UserAppointResponseStatus UserAppoint(int commandUid, int uid, int tid, string role)
    {
        string[] roles = { "Member", "Admin", "Deleted" };
        if (!roles.Contains(role)) return UserAppointResponseStatus.AuthorizationLimit;
        
        // 判断命令的发出者权限是否足够
        string? commandRole, commandedRole; // 
        commandRole = _r.ExecuteScalarWithParameters(
            "select role from web.TeamMember where tid = @tid and uid = @uid",
            new Dictionary<string, object?>
            {
                { "@tid", tid },
                { "@uid", commandUid }
            }
        )?.ToString();


        // 判断用户是否存在
        commandedRole = _r.ExecuteScalarWithParameters(
            "select role from web.TeamMember where tid = @tid and uid = @uid",
            new Dictionary<string, object?>
            {
                { "@tid", tid },
                { "@uid", uid }
            }
        )?.ToString();

        if (commandRole is null || commandedRole is null) return UserAppointResponseStatus.UserNonExist;

        // 判断是否有权限进行更改
        // 如果是本人进行的操作的，且为退出的话，授权
        if (_userRoles[commandRole] <= _userRoles[commandedRole]) // 如果二者的身份相当，则无法进行更改
            return UserAppointResponseStatus.AuthorizationLimit;

        // 判断用户id是否存在,并且获取用户的身份
        /*var connection = _m.GetConnection();
        sql = "select role from web.TeamMember where tid = @tid and uid = @uid";
        using (var sqlCommand = new MySqlCommand(sql, connection))
        {
            sqlCommand.Parameters.AddWithValue("@tid", tid);
            sqlCommand.Parameters.AddWithValue("@uid", uid);
            var result = sqlCommand.ExecuteScalar();
            if (result == null) return UserAppointResponseStatus.UserNonExist;
        }*/
        var r = _r.ExecuteNonQueryWithParameters(
            "update web.TeamMember set role = @role where tid = @tid and uid = @uid",
            new Dictionary<string, object?>
            {
                { "@tid", tid },
                { "@uid", uid },
                { "@role", role }
            }
        );
        if (r == 1) return UserAppointResponseStatus.Success;

        return UserAppointResponseStatus.UnKnown;
    }

    public ResponseModel DeleteTeam(int uid, int tid)
    {
        try
        {
            // 需要权限鉴定，使用存储过程来完成
            /*const string sql = "UserDeleteTeam";
            using var sqlCommand = new MySqlCommand(sql, _m.GetConnection());
            sqlCommand.CommandText = sql;
            sqlCommand.CommandType = CommandType.StoredProcedure;
            sqlCommand.Parameters.AddWithValue("@u", uid).Direction = ParameterDirection.Input;
            sqlCommand.Parameters.AddWithValue("@tid", tid).Direction = ParameterDirection.Input;
            sqlCommand.Parameters.Add("@msg", MySqlDbType.Int32).Direction = ParameterDirection.Output;*/
            // 执行存储过程
            /*sqlCommand.ExecuteNonQuery();
            // 从参数的Value属性中获取返回值
            var returnCode = (int)sqlCommand.Parameters["@msg"].Value;*/
            var r = _r.ExecuteProducerWithParameters(
                "UserDeleteTeam",
                new Dictionary<string, object?>
                {
                    { "@u", uid },
                    { "@tid", tid }
                }
            )?.ToString();
            return r == "ok"
                ? new ResponseModel(StatusModel.Success, "ok")
                : new ResponseModel(StatusModel.AuthorizationError, "权限不足");
        }
        catch (Exception e)
        {
            return new ResponseModel(StatusModel.Unknown, e.Message);
        }
    }

    public ResponseModel ExitTeam(int uid, int tid)
    {
        try
        {
            var r = _r.ExecuteNonQueryWithParameters(
                "update web.TeamMember set role = 'Deleted' where tid = @tid and uid = @uid",
                new Dictionary<string, object?>
                {
                    { "@tid", tid },
                    { "@uid", uid }
                }
            );
            return r == 1 
                ?new ResponseModel(StatusModel.Success, "ok")
                : new ResponseModel(StatusModel.Unknown, "未知错误");
        }
        catch (Exception e)
        {
            return new ResponseModel(StatusModel.Unknown, e.Message);
        }
    }


    /// <summary>
    /// </summary>
    /// <param name="tid"></param>
    /// <returns></returns>
    public ResponseModel GetTeamInfo(int tid)
    {
        try
        {
            using var result = _r.ExecuteReaderWithParameters(
                "select name,description,created_at,(select count(*) from web.TeamMember " +
                "where tid = @tid and role != 'Deleted' ) from web.TeamInfo " +
                "where tid = @tid",
                new Dictionary<string, object?>
                {
                    { "@tid", tid }
                }
            );
            // 读取信息
            if (result.HasRows)
            {
                result.Read();
                return new ResponseModel(StatusModel.Success, "ok", new TeamInfoModel(
                    result.GetString(0), result.GetString(1),
                    result.GetInt32(3),
                    result.GetDateTime(2).ToString("yyyy-MM-dd")
                ));
            }

            result.Close();
            return new ResponseModel(StatusModel.NonExist, "不存在该团队");
        }
        catch (Exception e)
        {
            return new ResponseModel(StatusModel.Unknown, e.Message);
        }
    }

    public ResponseModel GetUserTeams(int uid)
    {
        try
        {
            var result = _r.ExecuteReaderWithParameters(
                "select tid,name,description,PeopleNumber from web.UserTeamsView where uid = @uid",
                new Dictionary<string, object?>
                {
                    { "@uid", uid }
                }
            );
            if (!result.HasRows) // 如果没有查询到值，就返回错误
            {
                result.Close();
                return new ResponseModel(StatusModel.NonExist,"查询的团队不存在");
            }
            var list = new List<TeamItemModel>();
            while (result.Read())
                // 遍历查询结果表
                list.Add(new TeamItemModel(result.GetInt32(0), result.GetString(1), result.GetString(2),
                    result.GetInt32(3)));

            result.Close();

            return new ResponseModel(StatusModel.Success,"ok",list);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return new ResponseModel(StatusModel.Unknown, e.Message);
        }
    }

    public ResponseModel GetTeamMembers(int uid, int tid)
    {
        try
        {
            var result = _r.ExecuteReaderWithParameters(
                "select uid,name,role from web.TeamMemberView where tid = @tid and role != 'Deleted'",
                new Dictionary<string, object?>
                {
                    { "@tid", tid }
                }
            );
            
            var list = new List<TeamMemberItemModel>();
            // var flag = false; // 判断用户是否在这个团队中
            if (!result.HasRows) //如果结果列表为空，说明团队不存在
                return new ResponseModel(StatusModel.NonExist, "访问异常");

            while (result.Read())
                /*if (!flag && result.GetInt32(0) == uid) // 判断用户是否在这个团队中
                {
                    flag = true;
                }*/
                list.Add(new TeamMemberItemModel(result.GetInt32(0),
                    result.GetString(1), result.GetString(2)));

            result.Close();
            /*if (!flag)
            {
                return new ResponseModel(StatusModel.AuthorizationError, "用户不在这个团队中");
            }*/

            return new ResponseModel(StatusModel.Success, "ok", list);
        }
        catch (Exception e)
        {
            return new ResponseModel(StatusModel.Unknown, e.Message);
        }
    }

    public ResponseModel GetUploadRight(int tid)
    {
        try
        {
            // 先获取权限
            var right = ObjectStorageService.GetUploadRight($"TeamFiles/{tid}/*");
            return new ResponseModel(StatusModel.Success, "ok", right);
        }
        catch (Exception e)
        {
            return new ResponseModel(StatusModel.Unknown, e.Message);
        }
    }

    public ResponseModel UploadSuccess(int uid, int tid, string filename, string filepath)
    {
        try
        {
            var r = _r.ExecuteNonQueryWithParameters(
                "Insert Into web.FileInfo(fileName, filePath, Uploader) " +
                "VALUE (@fileName, @filePath, @uploader);" +
                "INSERT INTO web.TeamFiles(tid, fileId) VALUE (@tid, LAST_INSERT_ID());",
                new Dictionary<string, object?>
                {
                    { "@fileName", filename },
                    { "@filePath", filepath },
                    { "@uploader", uid },
                    { "@tid", tid }
                }
            );
            return r == 2
                ? new ResponseModel(StatusModel.Success,"ok")
                : new ResponseModel(StatusModel.Unknown, "后端接口异常");
        }
        catch (Exception e)
        {
            return new ResponseModel(StatusModel.Unknown, e.Message);
        }
    }

    public ResponseModel GetDownloadRight(int tid)
    {
        try
        {
            // 先获取权限
            var right = ObjectStorageService.GetDownloadRight($"TeamFiles/{tid}/*");
            return new ResponseModel(StatusModel.Success, "ok", right);
        }
        catch (Exception e)
        {
            return new ResponseModel(StatusModel.Unknown, e.Message);
        }
    }
}