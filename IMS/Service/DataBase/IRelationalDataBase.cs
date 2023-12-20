using System.Data;
using MySql.Data.MySqlClient;

namespace IMS.Service.DataBase;

public interface IRelationalDataBase
{
    /// <summary>
    ///     从数据库获取一个可用的链接
    /// </summary>
    /// <returns></returns>
    public MySqlConnection GetConnection();

    /// <summary>
    /// 执行需要参数的非查询语句，返回受影响的列数
    /// </summary>
    /// <param name="sql"></param>
    /// <param name="d"></param>
    /// <returns></returns>
    public int ExecuteNonQueryWithParameters(string sql, Dictionary<string, object?> d);
    
    /// <summary>
    /// 执行需要参数的单格查询语句，返回查询结果
    /// </summary>
    /// <param name="sql"></param>
    /// <param name="d"></param>
    /// <returns></returns>
    public object? ExecuteScalarWithParameters(string sql, Dictionary<string, object> d);

    /// <summary>
    /// 执行需要参数的存储过程，返回存储过程的信息
    /// </summary>
    /// <param name="sql"></param>
    /// <param name="d"></param>
    /// <returns></returns>
    public object? ExecuteProducerWithParameters(string sql, Dictionary<string, object?> d);

    /// <summary>
    /// 执行需要参数的select语句
    /// </summary>
    /// <param name="sql"></param>
    /// <param name="d"></param>
    /// <returns></returns>
    public IDataReader ExecuteReaderWithParameters(string sql, Dictionary<string, object?> d);
}