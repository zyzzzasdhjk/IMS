using MySql.Data.MySqlClient;

namespace IMS.Service.DataBase;

public interface IRelationalDataBase
{
    /// <summary>
    /// 初始化
    /// </summary>
    public void Init();
    
    /// <summary>
    /// 从数据库获取一个可用的链接
    /// </summary>
    /// <returns></returns>
    public MySqlConnection GetConnection();
}