using System.Data;
using IMS_API;
using MySql.Data.MySqlClient;

namespace IMS.Service.DataBase;

public class MysqlDataBase : IRelationalDataBase
{
    private readonly MySqlConnection _connection;

    public MysqlDataBase()
    {
        /*计划使用task方法来优化*/
        //创建一个 MySqlConnection 对象，并传入连接字符串。连接字符串包含了数据库的地址、用户名、密码等信息

        /*MySqlConnection connection = new MySqlConnection(
            "Database=web;Data Source=101.43.94.40;port=3306;" 
            + "User Id=CSuper;;SslMode=none;Password=76dfiu*T&,f+-&*UF;");*/

        var connection = new MySqlConnection(
            Common.MysqlConnectString
        );
        _connection = connection;
        _connection.Open();
    }

    public MySqlConnection GetConnection()
    {
        if (_connection.State == ConnectionState.Broken)
        {
            _connection.Close(); //与数据源连接中断，可先关闭，然后重新开启
            _connection.Open();
        }
        else if (_connection.State == ConnectionState.Closed)
        {
            _connection.Open();
        }

        return _connection;
    }

    
    public int ExecuteNonQueryWithParameters(string sql, Dictionary<string, object> d)
    {
        using var command = GetConnection().CreateCommand();
        command.CommandText = sql;
        foreach (var para in d)
        {
            command.Parameters.AddWithValue(para.Key, para.Value);
        }
        return command.ExecuteNonQuery();
    }

    public object? ExecuteScalarWithParameters(string sql, Dictionary<string, object> d)
    {
        using var command = GetConnection().CreateCommand();
        command.CommandText = sql;
        foreach (var para in d)
        {
            command.Parameters.AddWithValue(para.Key, para.Value);
        }
        return command.ExecuteScalar();
    }
    
    public object? ExecuteProducerWithParameters(string sql, Dictionary<string, object> d)
    {
        using var command = GetConnection().CreateCommand();
        command.CommandText = sql;
        command.CommandType = CommandType.StoredProcedure;
        foreach (var para in d)
        {
            command.Parameters.AddWithValue(para.Key, para.Value).Direction = ParameterDirection.Input;
        }
        // 要求所有的存储过程必须实现一个输出的msg
        command.Parameters.Add("@msg", MySqlDbType.Int32).Direction = ParameterDirection.Output;
        return command.ExecuteScalar();
    }
}