using System.Text.RegularExpressions;
using IMS.Models;
using IMS.Service.DataBase;
using MySql.Data.MySqlClient;
using Newtonsoft.Json.Linq;

namespace IMS.Service.UserServices;

class CheckCode
{
    public string Usename { get; set; }
    public DateTime Time { get; set; }
}

public class UserService : IUserService
{
    private IRelationalDataBase _d;
    private Dictionary<int,CheckCode> _changePwdCheckCodes= 
        new Dictionary<int,CheckCode>(); // 存储用户更改密码的令牌
    private Dictionary<int,CheckCode> _registerCheckCodes = 
        new Dictionary<int,CheckCode>(); // 存储用户注册时候的令牌

    public UserService(IRelationalDataBase database)
    {
        _d = database;
    }
    
    /// <summary>
    /// 判断密码是否符合规则
    /// 该规则为
    /// </summary>
    /// <param name="password"></param>
    /// <returns></returns>
    private static bool IsPasswordValid(string password)
    {
        return Regex.IsMatch(password, @"^(?![a-zA-Z]+$)(?!\d+$)(?![^\da-zA-Z\s]+$).{8,}$");
    }

    public ReturnMessageModel UpdateUser(JObject user)
    {
        string sql = "UPDATE USERINFO SET " +
                     "name = @name,description = @description," +
                     "gender = @gender,birthday = @birthday " +
                     "WHERE uid = @uid";
        using (MySqlCommand sqlCommand = new MySqlCommand(sql,_d.GetConnection()))
        {
            sqlCommand.Parameters.AddWithValue("@name", user["name"]);
            sqlCommand.Parameters.AddWithValue("@description", user["description"]);
            sqlCommand.Parameters.AddWithValue("@gender", user["gender"]);
            sqlCommand.Parameters.AddWithValue("@birthday", user["birthday"]);
            int num = sqlCommand.ExecuteNonQuery();
            if (num == 1)
            {
                return new ReturnMessageModel();
            }
            else
            {
                return new ReturnMessageModel("错误!");
            }
        }
    }

    public RegisterStatus RegisterUser(string username, string password ,string email)
    {
        try
        {
            /*判断用户名字是否过长或者为空*/
            if (username.Length>20 || username.Length<=0 || username == null)
            {
                return RegisterStatus.UsernameInvalid;
            }

            /*判断密码是否合格*/
            if (!IsPasswordValid(password))
            {
                return RegisterStatus.PasswordInvalid;
            }
            
            /*判断密码合格后对密码进行SHA加密*/
            password = PasswordHasher.HashPassword(password);
            
            /*查询是否存在相同的账号*/
            string sql = "select count(*) from web.User where username = @username";
            using (MySqlCommand sqlCommand = new MySqlCommand(sql,_d.GetConnection()))
            {
                sqlCommand.Parameters.AddWithValue("@username", username);
                if (Convert.ToInt32(sqlCommand.ExecuteScalar()) == 1)
                {
                    return RegisterStatus.UsernameExist;
                }
            }
            
            sql = "insert into web.User(username, password, status) value (@username,@password,'UnConfirmed')";
            using (MySqlCommand sqlCommand = new MySqlCommand(sql,_d.GetConnection()))
            {
                sqlCommand.Parameters.AddWithValue("@username", username);
                sqlCommand.Parameters.AddWithValue("@password", password);
                sqlCommand.ExecuteNonQuery();
            }

            // 如果上面的验证都通过了,生成一个随机的校验码
            var r = new Random();
            var checkCode = r.Next(100000000, 999999999); // 生成一个随机的校验码
            if (EmailService.SendEmail(email , "激活邮件",
                    String.Format("你的验证码为{0}，有效时间为30分钟，请尽快激活。",checkCode)))
            {
                _registerCheckCodes.Add(checkCode, 
                    new CheckCode() { Usename = username, Time = DateTime.Now });
            }
            else
            {
                return RegisterStatus.EmailError;
            }
            return RegisterStatus.Success;
        }
        catch (Exception e)
        {
            return RegisterStatus.PasswordInvalid;
        }
    }
    
    public ReturnMessageModel ResendEmail(string username ,string email)
    {
        // 如果上面的验证都通过了,生成一个随机的校验码
        var r = new Random();
        var checkCode = r.Next(100000000, 999999999); // 生成一个随机的校验码
        if (EmailService.SendEmail(email , "激活邮件",
                String.Format("你的验证码为{0}，有效时间为30分钟，请尽快激活。",checkCode)))
        {
            _registerCheckCodes.Add(checkCode, 
                new CheckCode() { Usename = username, Time = DateTime.Now });
        }
        else
        {
            return new ReturnMessageModel("邮件发送错误");
        }
        return new ReturnMessageModel();
    }

    public ReturnMessageModel ConfirmUser(string username ,int checkCode)
    {
        if (!_registerCheckCodes.ContainsKey(checkCode) || // 如果没有这个验证码
            username != _registerCheckCodes[checkCode].Usename) // 如果不是当前用户的校验码
        {
            return new ReturnMessageModel("验证码错误");
        }

        if (DateTime.Now - _registerCheckCodes[checkCode].Time > TimeSpan.FromMinutes(30)) // 如果超过10分钟
        {
            _registerCheckCodes.Remove(checkCode); // 过期则删除
            return new ReturnMessageModel("验证码已过期");
        }
        _registerCheckCodes.Remove(checkCode); // 验证成功删除验证码
        string sql = "update web.User set status = 'Normal' where username = @username";
        using (MySqlCommand sqlCommand = new MySqlCommand(sql,_d.GetConnection()))
        {
            sqlCommand.Parameters.AddWithValue("@username", username);
            var changeRow = sqlCommand.ExecuteNonQuery();
            if (changeRow == 1)
            {
                return new ReturnMessageModel();
            }
            else
            {
                return new ReturnMessageModel("后端错误");
            }
        }
    }

    public LoginStatus LoginUser(string username, string password)
    {
        /*根据账号进行查询*/
        string sql = "select password,status from web.User where username = @username";
        using (MySqlCommand sqlCommand = new MySqlCommand(sql,_d.GetConnection()))
        {
            sqlCommand.Parameters.AddWithValue("@username", username);
            var result = sqlCommand.ExecuteReader();
            if (!result.HasRows)
            {
                // 如果查询结果位null，说明不存在这个用户
                return LoginStatus.UserNameError;
            }
            result.Read();
            if (result.GetValue(1).ToString() != "Normal")
            {
                return LoginStatus.UserBanned;
            }
            if (!PasswordHasher.CheckPassword(password,result.GetValue(0).ToString()))
            {
                return LoginStatus.PasswordError;
            }
            result.Close();
            return LoginStatus.Success;
        }
    }

    public bool DeleteUser(string username, string password)
    {
        return true;
    }

    public ReturnMessageModel ResetPassword(string uid, string password)
    {
        throw new NotImplementedException();
    }
}