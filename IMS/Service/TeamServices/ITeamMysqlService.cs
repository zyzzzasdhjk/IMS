using System.Text.Json;
using System.Text.Json.Nodes;
using IMS.Service.DataBase;
using MySql.Data.MySqlClient;

namespace IMS.Service.TeamServices;

public class ITeamMysqlService : ITeamSqlService
{
    private MySqlConnection _connection;
    
    public ITeamMysqlService(IRelationalDataBase m)
    {
        _connection = m.GetConnection();
    }

    public string GetAllMembers(int tid)
    {
        
        string sql = "select name, role, created_at from web.TeamMemberView where tid = @tid & role!='Deleted'";
        using (MySqlCommand sqlCommand = new MySqlCommand(sql,_connection))
        {
            sqlCommand.Parameters.AddWithValue("@tid", tid);
            var result = sqlCommand.ExecuteReader();
            if (result.HasRows)
            {
                var list = new List<Dictionary<string, object>>();
                while (result.Read())
                {
                    var dict = new Dictionary<string, object>();
                    for (var i = 0; i < result.FieldCount; i++)
                    {
                        dict.Add(result.GetName(i), result.GetValue(i));
                    }
                    list.Add(dict);
                }
                return JsonSerializer.Serialize(list);;
            }
            else
            {
                return "";

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