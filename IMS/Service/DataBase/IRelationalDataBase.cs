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
    ///     判断用户的状态是否正常
    /// </summary>
    /// <param name="uid"></param>
    /// <returns></returns>
    public bool IsUserStatusNormal(int uid);
}