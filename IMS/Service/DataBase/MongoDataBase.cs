using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace IMS.Service.DataBase;

public class MongoDataBase : INosqlDataBase
{
    private readonly IMongoDatabase _d;

    /// <summary>
    /// 验证码模型
    /// </summary>
    [BsonIgnoreExtraElements]
    private class UserCode
    {
        public int Uid;
        public string CheckCode;
        public DateTime CreateAt; // readonly 只能在初始化的时候被修改
        /*设置readonly时会导致插入的空的文档*/

        public UserCode(int uid, int code)
        {
            Uid = uid;
            CheckCode = Convert.ToString(code);
            CreateAt = DateTime.Now;
        }
        
        public UserCode(int uid, string code)
        {
            Uid = uid;
            CheckCode = code;
            CreateAt = DateTime.Now;
        }
    }

    public MongoDataBase()
    {
        var m = new MongoClient(Common.MongoDbConnectString);
        _d = m.GetDatabase("WEB");
        
    }

    public IMongoCollection<BsonDocument> GetUserCollectionBase()
    {
        return _d.GetCollection<BsonDocument>("User");
    }
    
    /// <summary>
    /// 获取注册验证数据库
    /// </summary>
    /// <returns></returns>
    private IMongoCollection<UserCode> GetUserConfirmCodeCollection()
    {
        return _d.GetCollection<UserCode>("UserRegisterConfirm");
    }
    
    /// <summary>
    /// 获取认证数据库
    /// </summary>
    /// <returns></returns>
    private IMongoCollection<UserCode> GetUserAuthenticationCodeCollection()
    {
        return _d.GetCollection<UserCode>("UserAuthentication");
    }

    /// <summary>
    /// 将code添加到对应的集合中，添加时会删除掉之前存在的code
    /// </summary>
    /// <param name="collectionName"></param>
    /// <param name="uid"></param>
    /// <param name="code"></param>
    /// <returns></returns>
    private bool AddCodeToCollection(string collectionName, int uid, string code)
    {
        try
        {
            var collection = _d.GetCollection<UserCode>(collectionName);
            collection.DeleteMany(new BsonDocument("Uid", uid));
            collection.InsertOne(new UserCode(uid, code));
            return true;
        }
        catch (Exception e)
        {
            return false;
        }
    }
    
    /// <summary>
    /// 将用户的验证码添加进入集合中，超时时间为30分钟
    /// </summary>
    /// <param name="uid"></param>
    /// <param name="checkCode"></param>
    /// <returns></returns>
    public bool AddUserConfirmCode(int uid, int checkCode)
    {
        return AddCodeToCollection("UserRegisterConfirm", uid, Convert.ToString(checkCode));
    }
    
    /// <summary>
    /// 判断集合中是否存在该用户的验证码
    /// </summary>
    /// <param name="uid"></param>
    /// <returns></returns>
    public bool ExistUserConfirmCode(int uid)
    {
        var confirmCode = GetUserConfirmCodeCollection();
        return confirmCode.Find(new BsonDocument("Uid", uid)).Any();
    }

    /// <summary>
    /// 判断用户的验证码是否正确
    /// </summary>
    /// <param name="uid"></param>
    /// <param name="checkCode"></param>
    /// <returns></returns>
    public bool ValidateUserConfirmCode(int uid, int checkCode)
    {
        var confirmCode = GetUserConfirmCodeCollection();
        var filter = Builders<UserCode>.Filter.And(
                Builders<UserCode>.Filter.Eq("Uid", uid),
                Builders<UserCode>.Filter.Eq("CheckCode", 
                    Convert.ToString(checkCode)));
        return confirmCode.Find(filter).Any();
    }

    /// <summary>
    /// 向集合中添加用户的认证码
    /// </summary>
    /// <param name="uid"></param>
    /// <param name="code"></param>
    /// <returns></returns>
    public bool AddUserAuthenticationCode(int uid, string code)
    {
        try
        {
            /*如果用户登录，应该要先删除掉集合内以及存在的认证码，以更新持续时间*/
            var authenticationCode = GetUserAuthenticationCodeCollection();
            /*找到并且删除掉原有的认证码*/
            authenticationCode.FindOneAndDelete(
                Builders<UserCode>.Filter.Eq("Uid", uid));
            authenticationCode.InsertOne(new UserCode(uid,code));
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
        }
    }

    /// <summary>
    /// 判断用户认证码是否存在，如果存在的话，返回用户的uid，不然则返回-1
    /// </summary>
    /// <param name="code"></param>
    /// <returns></returns>
    public int ValidateUserAuthenticationCode(string code)
    {
        try
        {
            var authenticationCode = GetUserAuthenticationCodeCollection();
            var result = authenticationCode.Find(Builders<UserCode>.Filter.
                Eq("CheckCode", code)).FirstOrDefault();
            if (result == null)
            {
                return -1;
            }
            return result.Uid;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return -2;
        }
    }

    public bool AddUserEmailCheckCode(int uid)
    {
        throw new NotImplementedException();
    }
}