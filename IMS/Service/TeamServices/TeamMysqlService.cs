using IMS.Models;
using IMS.Models.Team;
using IMS.Service.DataBase;
using MySql.Data.MySqlClient;

namespace IMS.Service.TeamServices;

public class TeamMysqlService : ITeamSqlService
{
    private readonly IRelationalDataBase _m;
    private readonly Dictionary<string, int> _userRoles = new();
    /*public readonly string[] TeamInfoColumns = { "name", "description", "joinCode", "peopleMaxNum" };*/

    public TeamMysqlService(IRelationalDataBase m)
    {
        _m = m;
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
            var returnCode = _m.ExecuteProducerWithParameters(
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
            var result = _m.ExecuteScalarWithParameters(
                "select tid from web.TeamInfo  where joinCode = @joinCode",
                new Dictionary<string, object?>
                {
                    { "@joinCode", joinCode }
                }
            );
            if (result == null) return new ResponseModel(StatusModel.NonExist, "团队不存在"); // 不存在验证码当然是不存在这个团队咯

            int tid = Convert.ToInt32(result);
            
            
            // 更新数据库中用户的信息
            var row = _m.ExecuteNonQueryWithParameters(
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
            var result = _m.ExecuteScalarWithParameters(
                "select tid from web.TeamInfo where tid = @tid",
                new Dictionary<string, object?>
                {
                    { "@tid", tid }
                }
            );
            if (result is null) return new ResponseModel(StatusModel.NonExist, "团队不存在");

            // 更新数据库中用户的信息
            var i = _m.ExecuteNonQueryWithParameters(
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
        commandRole = _m.ExecuteScalarWithParameters(
            "select role from web.TeamMember where tid = @tid and uid = @uid",
            new Dictionary<string, object?>
            {
                { "@tid", tid },
                { "@uid", commandUid }
            }
        )?.ToString();


        // 判断用户是否存在
        commandedRole = _m.ExecuteScalarWithParameters(
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
        var r = _m.ExecuteNonQueryWithParameters(
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
            var r = _m.ExecuteProducerWithParameters(
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
            var r = _m.ExecuteNonQueryWithParameters(
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
            using var result = _m.ExecuteReaderWithParameters(
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
            var result = _m.ExecuteReaderWithParameters(
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
            var result = _m.ExecuteReaderWithParameters(
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

    /*/// <summary>
    /// 检测加入码是否已经存在
    /// </summary>
    /// <param name="joinCode"></param>
    /// <returns>true为存在</returns>
    private bool ExistJoinCode(string joinCode)
    {
        var connection = _m.GetConnection();
        var checkSql = "select * from web.TeamInfo where joinCode = @joinCode";
        using (var sqlCommand = new MySqlCommand(checkSql, connection))
        {
            sqlCommand.Parameters.AddWithValue("@joinCode", joinCode);
            var result = sqlCommand.ExecuteReader();
            if (result.HasRows) return true;

            return false;
        }
    }*/

    /*public ResponseModel CreateTeam(TeamInfoModel t)
    {
        var connection = _m.GetConnection();
        if (t.Name == null) // 名字不允许为空
            return new ResponseModel(StatusModel.ParameterInvalid, "名字不允许为空");

        string joinCode;

        if (t.JoinCOde != null)
        {
            if (!TeamFunction.CheckJoinCode(t.JoinCOde))
                return new ResponseModel(StatusModel.ParameterInvalid, "加入码格式不正确");

            if (ExistJoinCode(t.JoinCOde)) return new ResponseModel(StatusModel.ParameterInvalid, "加入码已存在");

            joinCode = t.JoinCOde;
        }
        else
        {
            // 如果用户没有给定生成码，自动生成加入码
            joinCode = TeamFunction.GenerateJoinCode();
            while (ExistJoinCode(joinCode)) // 但生成的加入码已经存在时，重新生成
                joinCode = TeamFunction.GenerateJoinCode();
        }

        const string sql =
            "insert into web.TeamInfo (name, description,JoinCode) values (@name, @description,@joinCode)";
        using (var sqlCommand = new MySqlCommand(sql, connection))
        {
            sqlCommand.Parameters.AddWithValue("@name", t.Name);
            sqlCommand.Parameters.AddWithValue("@description", t.Description);
            sqlCommand.Parameters.AddWithValue("@joinCode", joinCode);
            var result = sqlCommand.ExecuteNonQuery();
            return result >= 1
                ? new ResponseModel(StatusModel.Success, "ok")
                : new ResponseModel(StatusModel.Unknown, "修改失败");
        }
    }

    public ResponseModel UpdateTeam(TeamInfoModel t)
    {
        bool nameFlag = t.Name != null, descriptionFlag = t.Description != null;
        if (!nameFlag && !descriptionFlag) return new ResponseModel(StatusModel.ParameterInvalid, "未更新信息");

        var sql = string.Format("Update web.TeamInfo set {0} {1} {2} where tid = @tid",
            nameFlag ? "name = @name" : "", nameFlag & descriptionFlag ? "," : "",
            descriptionFlag ? "description = @description" : "");
        var connection = _m.GetConnection();
        using (var sqlCommand = new MySqlCommand(sql, connection))
        {
            sqlCommand.Parameters.AddWithValue("@tid", t.Tid);
            if (nameFlag) sqlCommand.Parameters.AddWithValue("@name", t.Name);

            if (descriptionFlag) sqlCommand.Parameters.AddWithValue("@description", t.Description);

            var result = sqlCommand.ExecuteNonQuery();
            return result >= 1
                ? new ResponseModel(StatusModel.Success, "ok")
                : new ResponseModel(StatusModel.Unknown, "修改失败");
        }
    }

    public ResponseModel UpdateTeamInfo(TeamInfoModel t)
    {
        try
        {
            const string sql = "update web.TeamInfo set name = @name, description = @description where tid = @tid";
            using var connection = _m.GetConnection();
            using var sqlCommand = new MySqlCommand(sql, connection);
            sqlCommand.Parameters.AddWithValue("@tid", t.Tid);
            sqlCommand.Parameters.AddWithValue("@name", t.Name);
            sqlCommand.Parameters.AddWithValue("@description", t.Description);
            var result = sqlCommand.ExecuteNonQuery();
            if (result == 1) return new ResponseModel(StatusModel.Success, "ok");

            return new ResponseModel(StatusModel.Unknown);
        }
        catch (Exception e)
        {
            return new ResponseModel(StatusModel.Unknown, e.Message);
        }
    }*/
}