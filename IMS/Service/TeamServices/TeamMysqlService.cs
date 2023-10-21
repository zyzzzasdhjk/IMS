using System.Collections;
using System.Text.Json;
using System.Text.Json.Nodes;
using IMS.Models;
using IMS.Models.Team;
using IMS.Service.DataBase;
using MySql.Data.MySqlClient;

namespace IMS.Service.TeamServices;

public class TeamMysqlService : ITeamSqlService
{
    private readonly IRelationalDataBase _m;
    public readonly string[] TeamInfoColumns = new string[]{"name","description","joinCode","peopleMaxNum"};
    
    public TeamMysqlService(IRelationalDataBase m)
    {
        _m = m;
    }

    public List<Dictionary<string, object>> GetAllMembers(int tid)
    {
        var connection = _m.GetConnection();
        string sql = "select name, role, created_at from web.TeamMemberView where tid = @tid & role!='Deleted'";
        using (MySqlCommand sqlCommand = new MySqlCommand(sql,connection))
        {
            sqlCommand.Parameters.AddWithValue("@tid", tid);
            var result = sqlCommand.ExecuteReader();
            if (result.HasRows)
            {
                return DataBaseFunction.GetJsonResult(result);
            }
            else
            {
                return new List<Dictionary<string, object>>();

            }
        }
    }
    
    public bool AddMember(int uid,int tid)
    {
        var connection = _m.GetConnection();
        string sql = "insert into web.TeamMember (tid, uid) values (@tid, @uid)";
        using (MySqlCommand sqlCommand = new MySqlCommand(sql,connection))
        {
            sqlCommand.Parameters.AddWithValue("@tid", tid);
            sqlCommand.Parameters.AddWithValue("@uid", uid);
            var result = sqlCommand.ExecuteNonQuery();
            return result >= 1 ? true : false;
        }
    }

    public bool DeleteMember(int uid,int tid)
    {
        var connection = _m.GetConnection();
        string sql = "delete from web.TeamMember where tid = @tid and uid = @uid";
        using (MySqlCommand sqlCommand = new MySqlCommand(sql,connection))
        {
            sqlCommand.Parameters.AddWithValue("@tid", tid);
            sqlCommand.Parameters.AddWithValue("@uid", uid);
            var result = sqlCommand.ExecuteNonQuery();
            return result >= 1 ? true : false;
        }
    }
    

    public bool DeleteAdministrator(int uid,int tid)
    {
        var connection = _m.GetConnection();
        string sql = "update web.TeamMember set role = 'Member' where tid = @tid and uid = @uid";
        using (MySqlCommand sqlCommand = new MySqlCommand(sql,connection))
        {
            sqlCommand.Parameters.AddWithValue("@tid", tid);
            sqlCommand.Parameters.AddWithValue("@uid", uid);
            var result = sqlCommand.ExecuteNonQuery();
            return result >= 1 ? true : false;
        }
    }

    public bool AddAdministrator(int uid,int tid)
    {
        var connection = _m.GetConnection();
        string sql = "update web.TeamMember set role = 'Admin' where tid = @tid and uid = @uid";
        using (MySqlCommand sqlCommand = new MySqlCommand(sql,connection))
        {
            sqlCommand.Parameters.AddWithValue("@tid", tid);
            sqlCommand.Parameters.AddWithValue("@uid", uid);
            var result = sqlCommand.ExecuteNonQuery();
            return result >= 1 ? true : false;
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
        using (MySqlCommand sqlCommand = new MySqlCommand(checkSql,connection))
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
    
    public ReturnMessageModel CreateTeam(TeamInfo t)
    {
        var connection = _m.GetConnection();
        if (t.Name == null) // 名字不允许为空
        {
            return new ReturnMessageModel("名字不允许为空");
        }

        string joinCode;
        
        if (t.JoinCOde!=null)
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
        const string sql = "insert into web.TeamInfo (name, description,JoinCode) values (@name, @description,@joinCode)";
        using (MySqlCommand sqlCommand = new MySqlCommand(sql,connection))
        {
            sqlCommand.Parameters.AddWithValue("@name", t.Name);
            sqlCommand.Parameters.AddWithValue("@description", t.Description);
            sqlCommand.Parameters.AddWithValue("@joinCode", joinCode);
            var result = sqlCommand.ExecuteNonQuery();
            return result >= 1 ? new ReturnMessageModel() : new ReturnMessageModel(false);
        }
        
    }

    public ReturnMessageModel UpdateTeam(TeamInfo t)
    {
        bool nameFlag = t.Name != null,descriptionFlag = t.Description != null;
        if (!nameFlag && !descriptionFlag)
        {
            return new ReturnMessageModel("未更新信息");
        }
        string sql = String.Format("Update web.TeamInfo set {0} {1} {2} where tid = @tid",nameFlag?"name = @name":"",nameFlag&descriptionFlag?",":"",descriptionFlag?"description = @description":"");
        var connection = _m.GetConnection();
        using (MySqlCommand sqlCommand = new MySqlCommand(sql,connection))
        {
            sqlCommand.Parameters.AddWithValue("@tid", t.tid);
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

    public ReturnMessageModel UpdateTeamPeopleMaxNum(int tid,int newNum)
    {
        string sql = "update web.TeamInfo set peopleMaxNum = @newNum where tid = @tid";
        var connection = _m.GetConnection();
        using (MySqlCommand sqlCommand = new MySqlCommand(sql,connection))
        {
            sqlCommand.Parameters.AddWithValue("@tid", tid);
            sqlCommand.Parameters.AddWithValue("@newNum", newNum);
            var result = sqlCommand.ExecuteNonQuery();
            return result >= 1 ? new ReturnMessageModel() : new ReturnMessageModel(false);
        }
    }
    
    public ReturnMessageModel DeleteTeam(int tid)
    {
        string sql = "delete from web.TeamInfo where tid = @tid";
        var connection = _m.GetConnection();
        using (MySqlCommand sqlCommand = new MySqlCommand(sql,connection))
        {
            sqlCommand.Parameters.AddWithValue("@tid", tid);
            var result = sqlCommand.ExecuteNonQuery();
            return result >= 1 ? new ReturnMessageModel() : new ReturnMessageModel(false);
        }
    }

    public ReturnMessageModel BanTeam(int tid, int time)
    {
        throw new NotImplementedException();
    }
}