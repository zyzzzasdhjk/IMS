using System.Text.Json;
using IMS.Service.DataBase;
using MySql.Data.MySqlClient;

namespace IMS.Service.TeamServices;

public class TeamMysqlService : ITeamSqlService
{
    private MySqlConnection _connection;
    
    public TeamMysqlService(IRelationalDataBase m)
    {
        _connection = m.GetConnection();
    }

    public List<Dictionary<string, object>> GetAllMembers(int tid)
    {
        
        string sql = "select name, role, created_at from web.TeamMemberView where tid = @tid & role!='Deleted'";
        using (MySqlCommand sqlCommand = new MySqlCommand(sql,_connection))
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

    public bool GetMember()
    {
        throw new NotImplementedException();
    }

    public bool AddMember()
    {
        throw new NotImplementedException();
    }

    public bool DeleteMember()
    {
        throw new NotImplementedException();
    }
    

    public bool DeleteAdministrator()
    {
        throw new NotImplementedException();
    }
    
    public bool GetAllAdministrators()
    {
        throw new NotImplementedException();
    }

    public bool AddAdministrator()
    {
        throw new NotImplementedException();
    }
}