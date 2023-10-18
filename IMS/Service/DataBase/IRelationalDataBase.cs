using MySql.Data.MySqlClient;

namespace IMS.Service.DataBase;

public interface IRelationalDataBase
{
    /// <summary>
    /// 从数据库获取一个可用的链接
    /// </summary>
    /// <returns></returns>
    public MySqlConnection GetConnection();
}