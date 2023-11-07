using MongoDB.Bson;
using MongoDB.Driver;

namespace IMS.Service.DataBase;

public class MongoDataBase : INosqlDataBase
{
    private IMongoDatabase _d;

    public class UserRegisterConfirmCode
    {
        public int Uid { get; set; }
        public int CheckCode { get; set; }
        public DateTime CreateAt{ get; set; }

        public UserRegisterConfirmCode(int uid, int checkCode)
        {
            Uid = uid;
            CheckCode = checkCode;
            CreateAt = DateTime.Now;
        }
    }
    
    public MongoDataBase()
    {
        var m = new MongoClient(Common.MongoDBConnectString);
        _d = m.GetDatabase("WEB");
        
    }

    public IMongoCollection<BsonDocument> GetUserCollectionBase()
    {
        return _d.GetCollection<BsonDocument>("User");
    }
    
    public IMongoCollection<UserRegisterConfirmCode> GetUserConfirmCodeCollection()
    {
        return _d.GetCollection<UserRegisterConfirmCode>("UserRegisterConfirm");
    }

    /// <summary>
    /// 将用户的验证码添加进入集合中，超时时间为30分钟
    /// </summary>
    /// <param name="uid"></param>
    /// <param name="checkCode"></param>
    /// <returns></returns>
    public bool AddUserConfirmCode(int uid, int checkCode)
    {
        var confirmCode = GetUserConfirmCodeCollection();
        confirmCode.InsertOne(new UserRegisterConfirmCode(uid,checkCode));
        return true;
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
    /// 删除掉用户之前存在的验证码
    /// </summary>
    /// <param name="uid"></param>
    /// <returns></returns>
    public bool DeleteUserConfirmCode(int uid)
    {
        var confirmCode = GetUserConfirmCodeCollection();
        confirmCode.DeleteOne(x => x.Uid == uid);
        return true;
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
        var filter = Builders<UserRegisterConfirmCode>.Filter.And(
                Builders<UserRegisterConfirmCode>.Filter.Eq("Uid", uid),
                Builders<UserRegisterConfirmCode>.Filter.Eq("CheckCode",checkCode));
        return confirmCode.Find(filter).Any();
    }
}