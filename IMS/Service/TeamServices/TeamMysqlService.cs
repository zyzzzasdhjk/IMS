using System.Data;
using IMS.Models;
using IMS.Models.Team;
using IMS.Models.User;
using IMS.Service.DataBase;
using MySql.Data.MySqlClient;

namespace IMS.Service.TeamServices;

public class TeamMysqlService : ITeamSqlService
{
    private readonly IRelationalDataBase _m;
    public readonly string[] TeamInfoColumns = new string[] { "name", "description", "joinCode", "peopleMaxNum" };
    private readonly Dictionary<string, int> _userRoles = new Dictionary<string, int>();

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
        if (name == "" || name.Length > 20)
        {
            return UserCreateTeamResponseStatus.NameIllegality; // 名字不能为空
        }

        if (joinCode.Length > 0 && joinCode.Length != 9)
        {
            return UserCreateTeamResponseStatus.JoinCodeIllegality;
        }

        joinCode = joinCode.ToUpper(); // 全大写
        if (description == "")
        {
            description = "团队的介绍暂时没有填写"; // 介绍为空时替换为默认介绍
        }

        // 调用存储过程的方式比较特殊
        using (var sqlCommand = new MySqlCommand())
        {
            sqlCommand.Connection = _m.GetConnection();
            const string sql = "UserCreateTeam";
            sqlCommand.CommandText = sql;
            sqlCommand.CommandType = CommandType.StoredProcedure;
            sqlCommand.Parameters.AddWithValue("@n", name).Direction = ParameterDirection.Input;
            sqlCommand.Parameters.AddWithValue("@d", description).Direction = ParameterDirection.Input;
            sqlCommand.Parameters.AddWithValue("@j", joinCode).Direction = ParameterDirection.Input;
            sqlCommand.Parameters.AddWithValue("@u", uid).Direction = ParameterDirection.Input;
            sqlCommand.Parameters.Add("@msg", MySqlDbType.Int32).Direction = ParameterDirection.Output;
            // 执行存储过程
            sqlCommand.ExecuteNonQuery();
            // 从参数的Value属性中获取返回值
            var returnCode = (int)sqlCommand.Parameters["@msg"].Value;
            if (returnCode == 1)
            {
                return UserCreateTeamResponseStatus.JoinCodeRepeat;
            }

            return UserCreateTeamResponseStatus.Success;
        }
    }


    public JoinTeamResponseModel JoinTeam(int uid, string joinCode)
    {
        try
        {
            // 先判断这个加入码的团队是否存在
            string sql = "select tid from web.TeamInfo  where joinCode = @joinCode";
            int tid;
            using (MySqlCommand sqlCommand = new MySqlCommand(sql, _m.GetConnection()))
            {
                sqlCommand.Parameters.AddWithValue("@joinCode", joinCode);
                var result = sqlCommand.ExecuteScalar();
                if (result == null)
                {
                    return new JoinTeamResponseModel(CommonStatus.NonExist); // 不存在验证码当然是不存在这个团队咯
                }

                tid = Convert.ToInt32(result);
            }

            // 更新数据库中用户的信息
            sql = "insert into web.TeamMember(tid, uid, role) value (@tid, @uid, 'Member')";
            using (MySqlCommand sqlCommand = new MySqlCommand(sql, _m.GetConnection()))
            {
                sqlCommand.Parameters.AddWithValue("@uid", uid);
                sqlCommand.Parameters.AddWithValue("@tid", tid);
                if (sqlCommand.ExecuteNonQuery() != 1)
                {
                    return new JoinTeamResponseModel(CommonStatus.Unknown); // 未知错误
                }
            }

            return new JoinTeamResponseModel(CommonStatus.Success);
        }
        catch (MySqlException e)
        {
            return new JoinTeamResponseModel(e.Number + " " + e.Message); // 返回错误信息
        }
    }

    public JoinTeamResponseModel JoinTeam(int uid, int tid)
    {
        try
        {
            // 判断对应的团队是否存在
            string sql = "select tid from web.TeamInfo where tid = @tid";
            using (MySqlCommand sqlCommand = new MySqlCommand(sql, _m.GetConnection()))
            {
                sqlCommand.Parameters.AddWithValue("@tid", tid);
                var result = sqlCommand.ExecuteScalar();
                if (result == null)
                {
                    return new JoinTeamResponseModel(CommonStatus.NonExist);
                }
            }

            // 更新数据库中用户的信息
            sql = "insert into web.TeamMember(tid, uid, role) value (@tid, @uid, 'Member')";
            using (MySqlCommand sqlCommand = new MySqlCommand(sql, _m.GetConnection()))
            {
                sqlCommand.Parameters.AddWithValue("@uid", uid);
                sqlCommand.Parameters.AddWithValue("@tid", tid);
                if (sqlCommand.ExecuteNonQuery() != 1)
                {
                    return new JoinTeamResponseModel(CommonStatus.Unknown); // 未知错误
                }
            }

            return new JoinTeamResponseModel(CommonStatus.Success);
        }
        catch (Exception e)
        {
            return new JoinTeamResponseModel(e.Message); // 返回错误信息
        }
    }

    public UserAppointResponseStatus UserAppoint(int commandUid, int uid, int tid, string role)
    {
        string[] roles = { "Member", "Admin", "Deleted" };
        if (!roles.Contains(role))
        {
            return UserAppointResponseStatus.AuthorizationLimit;
        }

        // 判断命令的发出者权限是否足够
        string sql = "select role from web.TeamMember where tid = @tid and uid = @uid";
        string? commandRole, commandedRole; // 
        using (MySqlCommand sqlCommand = new MySqlCommand(sql, _m.GetConnection()))
        {
            sqlCommand.Parameters.AddWithValue("@tid", tid);
            sqlCommand.Parameters.AddWithValue("@uid", commandUid);
            var result = sqlCommand.ExecuteScalar();
            commandRole = result.ToString();
        }

        // 判断用户是否存在
        sql = "select role from web.TeamMember where uid = @uid";
        using (MySqlCommand sqlCommand = new MySqlCommand(sql, _m.GetConnection()))
        {
            sqlCommand.Parameters.AddWithValue("@uid", uid);
            var result = sqlCommand.ExecuteScalar();
            commandedRole = result.ToString();
        }

        if (commandRole is null || commandedRole is null)
        {
            return UserAppointResponseStatus.UserNonExist;
        }

        // 判断是否有权限进行更改
        // 如果是本人进行的操作的，且为退出的话，授权
        if (_userRoles[commandRole] <= _userRoles[commandedRole]) // 如果二者的身份相当，则无法进行更改
        {
            return UserAppointResponseStatus.AuthorizationLimit;
        }

        // 判断用户id是否存在,并且获取用户的算法
        var connection = _m.GetConnection();
        sql = "select role from web.TeamMember where tid = @tid and uid = @uid";
        using (MySqlCommand sqlCommand = new MySqlCommand(sql, connection))
        {
            sqlCommand.Parameters.AddWithValue("@tid", tid);
            sqlCommand.Parameters.AddWithValue("@uid", uid);
            var result = sqlCommand.ExecuteScalar();
            if (result == null)
            {
                return UserAppointResponseStatus.UserNonExist;
            }
        }

        sql = "update web.TeamMember set role = @role where tid = @tid and uid = @uid";
        using (MySqlCommand sqlCommand = new MySqlCommand(sql, connection))
        {
            sqlCommand.Parameters.AddWithValue("@tid", tid);
            sqlCommand.Parameters.AddWithValue("@uid", uid);
            sqlCommand.Parameters.AddWithValue("@role", role);
            var result = sqlCommand.ExecuteNonQuery();
            if (result == 1)
            {
                return UserAppointResponseStatus.Success;
            }
        }

        return UserAppointResponseStatus.UnKnown;
    }

    public ResponseModel DeleteTeam(int uid, int tid)
    {
        try
        {
            // 需要权限鉴定，使用存储过程来完成
            const string sql = "UserDeleteTeam";
            using var sqlCommand = new MySqlCommand(sql, _m.GetConnection());
            sqlCommand.CommandText = sql;
            sqlCommand.CommandType = CommandType.StoredProcedure;
            sqlCommand.Parameters.AddWithValue("@u", uid).Direction = ParameterDirection.Input;
            sqlCommand.Parameters.AddWithValue("@tid", tid).Direction = ParameterDirection.Input;
            sqlCommand.Parameters.Add("@msg", MySqlDbType.Int32).Direction = ParameterDirection.Output;
            // 执行存储过程
            sqlCommand.ExecuteNonQuery();
            // 从参数的Value属性中获取返回值
            var returnCode = (int)sqlCommand.Parameters["@msg"].Value;
            return returnCode == 1
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
            const string sql = "update web.TeamMember set role = 'Deleted' where tid = @tid and uid = @uid";
            using (MySqlCommand sqlCommand = new MySqlCommand(sql, _m.GetConnection()))
            {
                sqlCommand.Parameters.AddWithValue("@tid", tid);
                sqlCommand.Parameters.AddWithValue("@uid", uid);
                var result = sqlCommand.ExecuteNonQuery();
                if (result == 1)
                {
                    return new ResponseModel(StatusModel.Success, "ok");
                }
            }

            return new ResponseModel(StatusModel.Unknown, "1");
        }
        catch (Exception e)
        {
            return new ResponseModel(StatusModel.Unknown, e.Message);
        }
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="tid"></param>
    /// <returns></returns>
    public ResponseModel GetTeamInfo(int tid)
    {
        try
        {
            const string sql = "select name,description,created_at,(select count(*) from web.TeamMember where tid = @tid and role != 'Deleted' ) from web.TeamInfo where tid = @tid";
            using MySqlCommand sqlCommand = new MySqlCommand(sql, _m.GetConnection());
            sqlCommand.Parameters.AddWithValue("@tid", tid);
            var result = sqlCommand.ExecuteReader();
            if (result.HasRows)
            {
                result.Read();
                return new ResponseModel(StatusModel.Success, new TeamInfoModel(
                    result.GetString(0),result.GetString(1),
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

    /// <summary>
    /// 检测加入码是否已经存在
    /// </summary>
    /// <param name="joinCode"></param>
    /// <returns>true为存在</returns>
    private bool ExistJoinCode(string joinCode)
    {
        var connection = _m.GetConnection();
        string checkSql = "select * from web.TeamInfo where joinCode = @joinCode";
        using (MySqlCommand sqlCommand = new MySqlCommand(checkSql, connection))
        {
            sqlCommand.Parameters.AddWithValue("@joinCode", joinCode);
            var result = sqlCommand.ExecuteReader();
            if (result.HasRows)
            {
                return true;
            }

            return false;
        }
    }

    public ReturnMessageModel CreateTeam(TeamInfoModel t)
    {
        var connection = _m.GetConnection();
        if (t.Name == null) // 名字不允许为空
        {
            return new ReturnMessageModel("名字不允许为空");
        }

        string joinCode;

        if (t.JoinCOde != null)
        {
            if (!TeamFunction.CheckJoinCode(t.JoinCOde))
            {
                return new ReturnMessageModel("加入码格式不正确");
            }

            if (ExistJoinCode(t.JoinCOde))
            {
                return new ReturnMessageModel("加入码已存在");
            }

            joinCode = t.JoinCOde;
        }
        else
        {
            // 如果用户没有给定生成码，自动生成加入码
            joinCode = TeamFunction.GenerateJoinCode();
            while (ExistJoinCode(joinCode)) // 但生成的加入码已经存在时，重新生成
            {
                joinCode = TeamFunction.GenerateJoinCode();
            }
        }

        const string sql =
            "insert into web.TeamInfo (name, description,JoinCode) values (@name, @description,@joinCode)";
        using (MySqlCommand sqlCommand = new MySqlCommand(sql, connection))
        {
            sqlCommand.Parameters.AddWithValue("@name", t.Name);
            sqlCommand.Parameters.AddWithValue("@description", t.Description);
            sqlCommand.Parameters.AddWithValue("@joinCode", joinCode);
            var result = sqlCommand.ExecuteNonQuery();
            return result >= 1 ? new ReturnMessageModel() : new ReturnMessageModel(false);
        }
    }

    public ReturnMessageModel UpdateTeam(TeamInfoModel t)
    {
        bool nameFlag = t.Name != null, descriptionFlag = t.Description != null;
        if (!nameFlag && !descriptionFlag)
        {
            return new ReturnMessageModel("未更新信息");
        }

        string sql = String.Format("Update web.TeamInfo set {0} {1} {2} where tid = @tid",
            nameFlag ? "name = @name" : "", nameFlag & descriptionFlag ? "," : "",
            descriptionFlag ? "description = @description" : "");
        var connection = _m.GetConnection();
        using (MySqlCommand sqlCommand = new MySqlCommand(sql, connection))
        {
            sqlCommand.Parameters.AddWithValue("@tid", t.Tid);
            if (nameFlag)
            {
                sqlCommand.Parameters.AddWithValue("@name", t.Name);
            }

            if (descriptionFlag)
            {
                sqlCommand.Parameters.AddWithValue("@description", t.Description);
            }

            var result = sqlCommand.ExecuteNonQuery();
            return result >= 1 ? new ReturnMessageModel() : new ReturnMessageModel(false);
        }
    }

    public GetUserTeamsReturnModel GetUserTeams(int uid)
    {
        if (!_m.IsUserStatusNormal(uid))
        {
            return new GetUserTeamsReturnModel(GetUserTeamsReturnStatus.Error);
        }

        var connection = _m.GetConnection();
        const string sql = "select tid,name,description,PeopleNumber from web.UserTeamsView where uid = @uid";
        using (MySqlCommand sqlCommand = new MySqlCommand(sql, connection))
        {
            sqlCommand.Parameters.AddWithValue("@uid", uid);
            var result = sqlCommand.ExecuteReader();
            if (!result.HasRows) // 如果没有查询到值，就返回错误
            {
                result.Close();
                return new GetUserTeamsReturnModel(GetUserTeamsReturnStatus.Error);
            }

            try
            {
                var list = new List<TeamItemModel>();
                while (result.Read())
                {
                    // 遍历查询结果表
                    list.Add(new TeamItemModel(result.GetInt32(0), result.GetString(1), result.GetString(2),
                        result.GetInt32(3)));
                }

                result.Close();

                return new GetUserTeamsReturnModel(list);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return new GetUserTeamsReturnModel(GetUserTeamsReturnStatus.Error);
            }
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
            if (result == 1)
            {
                return new ResponseModel(StatusModel.Success, "ok");
            }

            return new ResponseModel(StatusModel.Unknown);
        }
        catch (Exception e)
        {
            return new ResponseModel(StatusModel.Unknown, e.Message);
        }
    }

    public ResponseModel GetTeamMembers(int uid, int tid)
    {
        try
        {
            const string sql = "select uid,name,role from web.TeamMemberView where tid = @tid and role != 'Deleted'";
            using var connection = _m.GetConnection();
            using var sqlCommand = new MySqlCommand(sql, connection);
            sqlCommand.Parameters.AddWithValue("@tid", tid);
            var result = sqlCommand.ExecuteReader();
            var list = new List<TeamMemberItemModel>();
            var flag = false; // 判断用户是否在这个团队中
            if (!result.HasRows) //如果结果列表为空，说明团队不存在
            {
                return new ResponseModel(StatusModel.NonExist, "访问异常");
            }

            while (result.Read())
            {
                if (!flag && result.GetInt32(0) == uid) // 判断用户是否在这个团队中
                {
                    flag = true;
                }

                list.Add(new TeamMemberItemModel(result.GetInt32(0),
                    result.GetString(1), result.GetString(2)));
            }

            result.Close();
            if (!flag)
            {
                return new ResponseModel(StatusModel.AuthorizationError, "用户不在这个团队中");
            }

            return new ResponseModel(StatusModel.Success, list);
        }
        catch (Exception e)
        {
            return new ResponseModel(StatusModel.Unknown, e.Message);
        }
    }
}