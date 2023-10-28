using System.Text.RegularExpressions;
using IMS.Models;
using IMS.Service.DataBase;
using Microsoft.AspNetCore.Mvc.Razor;
using MySql.Data.MySqlClient;
using Newtonsoft.Json.Linq;

namespace IMS.Service.UserServices;

public class UserService : IUserService
{
    private IRelationalDataBase _d;

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

    
    public RegisterStatus RegisterUser(string username, string password)
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
            
            Console.WriteLine(password.Length);
            sql = "insert into web.User(username, password) value(@username,@password)";
            using (MySqlCommand sqlCommand = new MySqlCommand(sql,_d.GetConnection()))
            {
                sqlCommand.Parameters.AddWithValue("@username", username);
                sqlCommand.Parameters.AddWithValue("@password", password);
                sqlCommand.ExecuteNonQuery();
            }

            return RegisterStatus.Success;
        }
        catch (Exception e)
        {
            return RegisterStatus.PasswordInvalid;
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