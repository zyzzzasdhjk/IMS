using System.Security.Cryptography;
using System.Text;
using MongoDB.Bson;
using MongoDB.Driver;

namespace IMS.Service.DataBase;

public interface INosqlDataBase
{
    /// <summary>
    ///     获取一个用户数据库的连接
    /// </summary>
    /// <returns></returns>
    public IMongoCollection<BsonDocument> GetUserCollectionBase();

    /// <summary>
    ///     向集合中插入用户的邮箱验证码，过期时间30min
    /// </summary>
    /// <param name="uid"></param>
    /// <param name="checkCode"></param>
    /// <returns></returns>
    public bool AddUserConfirmCode(int uid, int checkCode);

    /// <summary>
    ///     判断用户的验证码是否正确
    /// </summary>
    /// <param name="uid"></param>
    /// <param name="checkCode"></param>
    /// <returns></returns>
    public bool ValidateUserConfirmCode(int uid, int checkCode);

    /// <summary>
    ///     判断集合中是否存在该用户的验证码
    /// </summary>
    /// <param name="uid"></param>
    /// <returns></returns>
    public bool ExistUserConfirmCode(int uid);

    /// <summary>
    ///     向集合中添加用户的认证码
    /// </summary>
    /// <param name="uid"></param>
    /// <param name="code"></param>
    /// <returns></returns>
    public bool AddUserAuthenticationCode(int uid, string code);

    /// <summary>
    ///     判断用户认证码是否存在，如果存在的话，返回用户的uid，不然则返回-1
    /// </summary>
    /// <param name="code"></param>
    /// <returns></returns>
    public int ValidateUserAuthenticationCode(string code);

    /// <summary>
    ///     向集合中添加用户的邮箱验证码
    /// </summary>
    /// <param name="uid"></param>
    /// <returns></returns>
    public bool AddUserEmailCheckCode(int uid);

    /// <summary>
    ///     将获取的上传密钥暂时存起来
    /// </summary>
    /// <param name="s"></param>
    /// <param name="name">key的名字</param>
    /// <returns></returns>
    public void AddTmpKey(object s, string name);

    /// <summary>
    ///     获取暂存的key,没有的话
    /// </summary>
    /// <param name="name">key的名字</param>
    /// <returns></returns>
    public object? GetTmpKey(string name);

    /// <summary>
    ///     根据uid，密码和当前时间生成校验码
    /// </summary>
    /// <param name="uid"></param>
    /// <param name="password"></param>
    /// <returns></returns>
    public static string GenerateUserAuthenticationCode(int uid, string password)
    {
        // 获取当前时间的UTC格式
        var currentTime = DateTime.UtcNow;
        // 把当前时间转换为字符串
        var timeString = currentTime.ToString("yyyyMMddHHmmss");
        // 把uid，密码和时间字符串拼接起来
        var input = uid + password + timeString;
        // 创建一个SHA256实例
        var sha256 = SHA256.Create();
        // 计算输入的哈希值
        var hashValue = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
        // 把哈希值转换为十六进制字符串
        var authenticationCode = BitConverter.ToString(hashValue).Replace("-", "");
        // 返回校验码
        return authenticationCode;
    }
}