﻿using MongoDB.Bson;
using MongoDB.Driver;

namespace IMS.Service.DataBase;

public class MongoDataBase : INosqlDataBase
{
    private readonly IMongoDatabase _d;

    /// <summary>
    /// 验证码模型
    /// </summary>
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
        var m = new MongoClient(Common.MongoDBConnectString);
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
        return _d.GetCollection<UserCode>("UserRegisterConfirm");
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
        var u = new UserCode(uid, checkCode);
        Console.WriteLine($"{u.Uid}   {u.CheckCode}");
        confirmCode.InsertOne(u);
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
            var authenticationCode = GetUserAuthenticationCodeCollection();
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
}