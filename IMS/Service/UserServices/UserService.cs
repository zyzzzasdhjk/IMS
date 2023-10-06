using System;
using System.Text.RegularExpressions;
using IMS.Service.DataBase;
using IMS.Service.DataBase;
using MySql.Data.MySqlClient;

namespace IMS.Service.UserServices;

public class UserService : IUserService
{
    private MySqlConnection _connection;

    public UserService(IRelationalDataBase database)
    {
        _connection = database.GetConnection();
    }
    
    /// <summary>
    /// 判断密码是否符合规则
    /// 该规则为
    /// </summary>
    /// <param name="password"></param>
    /// <returns></returns>
    private static bool IsPasswordValid(string password)
    {
        return Regex.IsMatch(password, @"^(?=.*\d)(?=.*[a-zA-Z])(?=.*[\W_]).{8,}$");
    }


    public RegisterStatus InsertUser(string username, string password)
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
            string sql = "select count(*) from web.user where username = @username";
            using (MySqlCommand sqlCommand = new MySqlCommand(sql,_connection))
            {
                sqlCommand.Parameters.AddWithValue("@username", username);
                if (Convert.ToInt32(sqlCommand.ExecuteScalar()) == 1)
                {
                    return RegisterStatus.UsernameExist;
                }
            }
            
            Console.WriteLine(password.Length);
            sql = "insert into web.user(username, password) value(@username,@password)";
            using (MySqlCommand sqlCommand = new MySqlCommand(sql,_connection))
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
        string sql = "select password from web.user where username = @username";
        using (MySqlCommand sqlCommand = new MySqlCommand(sql,_connection))
        {
            sqlCommand.Parameters.AddWithValue("@username", username);
            var result = Convert.ToString(sqlCommand.ExecuteScalar());
            if (result == null)
            {
                // 如果查询结果位null，说明不存在这个用户
                return LoginStatus.UserNameError;
            }
            
            if (!PasswordHasher.CheckPassword(password,result))
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
}