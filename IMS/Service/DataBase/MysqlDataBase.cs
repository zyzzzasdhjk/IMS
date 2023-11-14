using System.Data;
using MySql.Data.MySqlClient;

namespace IMS.Service.DataBase;

public class MysqlDataBase : IRelationalDataBase
{
    private MySqlConnection _connection;

    public MysqlDataBase()
    {
        /*计划使用task方法来优化*/
        //创建一个 MySqlConnection 对象，并传入连接字符串。连接字符串包含了数据库的地址、用户名、密码等信息
        
        /*MySqlConnection connection = new MySqlConnection(
            "Database=web;Data Source=101.43.94.40;port=3306;" 
            + "User Id=CSuper;;SslMode=none;Password=76dfiu*T&,f+-&*UF;");*/

        MySqlConnection connection = new MySqlConnection(
            Common.MysqlConnectString
        );
        _connection = connection;
        _connection.Open();
    }

    public MySqlConnection GetConnection()
    {
        /*if (_connection.State == ConnectionState.Broken)
        {
            _connection.Close();//与数据源连接中断，可先关闭，然后重新开启
            _connection.Open();
        }
        else if (_connection.State == ConnectionState.Closed)
        {
            _connection.Open();
        }*/
        return _connection;
    }

    public bool IsUserStatusNormal(int uid)
    {
        var c = GetConnection();
        const string sql = "select count(*) from web.User " +
                           "where uid = @uid and  status = 'Normal'";
        using (var sqlCommand = new MySqlCommand(sql, c))
        {
            sqlCommand.Parameters.AddWithValue("@uid", uid);
            var result = sqlCommand.ExecuteScalar();
            return Convert.ToInt32(result) > 0;
        }
    }
}