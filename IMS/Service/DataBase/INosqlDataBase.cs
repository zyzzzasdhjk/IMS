using MongoDB.Bson;
using MongoDB.Driver;

namespace IMS.Service.DataBase;

public interface INosqlDataBase
{
    /// <summary>
    /// 获取一个用户数据库的连接
    /// </summary>
    /// <returns></returns>
    public IMongoCollection<BsonDocument> GetUserCollectionBase();

    public IMongoCollection<MongoDataBase.UserRegisterConfirmCode> GetUserConfirmCodeCollection();
    
    /// <summary>
    /// 向集合中插入用户的验证码
    /// </summary>
    /// <param name="uid"></param>
    /// <param name="checkCode"></param>
    /// <returns></returns>
    public bool AddUserConfirmCode(int uid, int checkCode);

    /// <summary>
    /// 判断用户的验证码是否正确
    /// </summary>
    /// <param name="uid"></param>
    /// <param name="checkCode"></param>
    /// <returns></returns>
    public bool ValidateUserConfirmCode(int uid, int checkCode);

    /// <summary>
    /// 判断集合中是否存在该用户的验证码
    /// </summary>
    /// <param name="uid"></param>
    /// <returns></returns>
    public bool ExistUserConfirmCode(int uid);
}