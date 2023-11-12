using System.Text.RegularExpressions;
using IMS.Models;
using IMS.Models.User;
using IMS.Service.DataBase;
using MySql.Data.MySqlClient;
using Newtonsoft.Json.Linq;

namespace IMS.Service.UserServices;

class CheckCode
{
    public string Username { get; set; } = "";
    public DateTime Time { get; set; }
}

public class UserService : IUserService
{
    private IRelationalDataBase _d;
    private INosqlDataBase _m; // 非关系型数据库

    private Dictionary<int, CheckCode> _registerCheckCodes =
        new Dictionary<int, CheckCode>(); // 存储用户注册时候的令牌

    public UserService(IRelationalDataBase d1, INosqlDataBase d2)
    {
        _d = d1;
        _m = d2;
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
        using (MySqlCommand sqlCommand = new MySqlCommand(sql, _d.GetConnection()))
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

    public bool IsAuthorization(int uid, string code)
    {
        if (code == "AuthorizationTest")
        {
            return true;
        } // 测试用，上线时要删除
        var u = _m.ValidateUserAuthenticationCode(code);
        if (u != -1 && u == uid)
        {
            return true;
        }

        return false;
    }
    
    public UserRegisterReturnModel RegisterUser(string username, string password, string email)
    {
        /*判断用户名字是否过长或者为空*/
        if (username.Length > 20 || username.Length <= 0)
        {
            return new UserRegisterReturnModel(UserRegisterReturnStatus.UserNameIllegality);
        }

        if (email.Length > 30 || email.Length <= 0)
        {
            return new UserRegisterReturnModel(UserRegisterReturnStatus.EmailIllegality);
        }

        /*判断密码是否合格*/
        if (!IsPasswordValid(password))
        {
            return new UserRegisterReturnModel(UserRegisterReturnStatus.PassWordIllegality);
        }

        /*判断密码合格后对密码进行SHA加密*/
        password = PasswordHasher.HashPassword(password);

        /*查询是否存在相同的账号或者是邮箱*/
        const string sql = "select username,email from web.User " +
                           "where username = @username or email = @email";
        using (var sqlCommand = new MySqlCommand(sql, _d.GetConnection()))
        {
            sqlCommand.Parameters.AddWithValue("@username", username);
            sqlCommand.Parameters.AddWithValue("@email", email);
            var result = sqlCommand.ExecuteReader();
            if (result.HasRows) // 当存在数据的时候，说明有邮箱或者是用户名重复了
            {
                result.Read();
                var resultUsername = result.GetValue(0).ToString();
                var resultEmail = result.GetValue(1).ToString();
                result.Close();
                if (resultUsername == username)
                {
                    return new UserRegisterReturnModel(
                        UserRegisterReturnStatus.UserNameRepeat);
                }

                if (resultEmail == email)
                {
                    return new
                        UserRegisterReturnModel(
                            UserRegisterReturnStatus.EmailRepeat);
                }
            }

            result.Close();
        }

        int uid; // 插入数据后用户的id
        /*开始向数据库插入用户的信息*/
        const string sql1 = "insert into web.User(username, password, status, email) " +
                            "value (@username,@password,'UnConfirmed',@email)";
        using (var sqlCommand = new MySqlCommand(sql1, _d.GetConnection()))
        {
            sqlCommand.Parameters.AddWithValue("@username", username);
            sqlCommand.Parameters.AddWithValue("@password", password);
            sqlCommand.Parameters.AddWithValue("@email", email);
            sqlCommand.ExecuteNonQuery();
            uid = Convert.ToInt32(sqlCommand.LastInsertedId);
        }

        // 如果上面的验证都通过了,生成一个随机的校验码
        var r = new Random();
        var checkCode = r.Next(100000000, 999999999); // 生成一个随机的校验码
        if (EmailService.SendEmail(email, "激活邮件",
                $"你的验证码为 {checkCode} ，有效时间为30分钟，请尽快激活。"))
        {
            _m.AddUserConfirmCode(uid, checkCode);
        }
        else
        {
            return new UserRegisterReturnModel(UserRegisterReturnStatus.EmailError);
        }
        
        return new UserRegisterReturnModel(uid);
    }

    public ReturnMessageModel ResendEmail(string username, string email)
    {
        // 如果上面的验证都通过了,生成一个随机的校验码
        var r = new Random();
        var checkCode = r.Next(100000000, 999999999); // 生成一个随机的校验码
        if (EmailService.SendEmail(email, "激活邮件",
                String.Format("你的验证码为{0}，有效时间为30分钟，请尽快激活。", checkCode)))
        {
            _registerCheckCodes.Add(checkCode,
                new CheckCode() { Username = username, Time = DateTime.Now });
        }
        else
        {
            return new ReturnMessageModel("邮件发送错误");
        }

        return new ReturnMessageModel();
    }

    public UserConfirmReturnModel ConfirmUser(int uid, int checkCode)
    {
        /*实现校验用户是否是未验证状态*/
        const string sql1 = "select status from web.User where uid = @uid";
        using (var sqlCommand = new MySqlCommand(sql1, _d.GetConnection()))
        {
            sqlCommand.Parameters.AddWithValue("@uid", uid);
            var result = sqlCommand.ExecuteScalar();
            if (result != null)
            {
                if (Convert.ToString(result) != "UnConfirmed")
                {
                    return new UserConfirmReturnModel(UserConfirmReturnStatus.Completed);
                }
            }
        }

        if (_m.ValidateUserConfirmCode(uid, checkCode))
        {
            // 验证成功，此时修改用户数据库内的用户状态
            const string sql2 = "update web.User set status = 'Normal' where uid = @uid";
            using (var sqlCommand2 = new MySqlCommand(sql2, _d.GetConnection()))
            {
                sqlCommand2.Parameters.AddWithValue("@uid", uid);
                sqlCommand2.ExecuteNonQuery();
            }

            return new UserConfirmReturnModel(UserConfirmReturnStatus.Success);
        }
        return new UserConfirmReturnModel(UserConfirmReturnStatus.Error);
    }

    public UserLoginReturnModel LoginUser(string username, string password)
    {
        /*根据账号进行查询*/
        const string sql = "select uid,password,status from web.User where username = @username";
        using (var sqlCommand = new MySqlCommand(sql, _d.GetConnection()))
        {
            sqlCommand.Parameters.AddWithValue("@username", username);
            var result = sqlCommand.ExecuteReader();
            if (!result.HasRows)
            {
                result.Close();
                // 如果查询结果位null，说明不存在这个用户
                return new UserLoginReturnModel(UserLoginReturnStatus.UsernameOrPasswordError);
            }

            result.Read();
            var uid = Convert.ToInt32(result.GetValue(0));  // 用户ID
            var userPassword = result.GetValue(1).ToString(); // 用户密码
            var userStatus = result.GetValue(2).ToString(); // 用户状态
            result.Close(); // 及时关闭，防止出现未关闭错误

            switch (userStatus)
            {
                case "Banned":
                    return new UserLoginReturnModel(UserLoginReturnStatus.UserBanned);
                case "UnConfirmed":
                    return new UserLoginReturnModel(UserLoginReturnStatus.UserUnconfirmed);
            }

            if (userPassword == null) //密码为空，什么数据库错误或者是账号数据错误
            {
                return new UserLoginReturnModel(UserLoginReturnStatus.UserDataError);
            }

            if (!PasswordHasher.CheckPassword(password, userPassword))
            {
                return new UserLoginReturnModel(UserLoginReturnStatus.UsernameOrPasswordError);
            }
            
            /*生成用户认证码，用于后面的认证*/
            var authenticationCode = INosqlDataBase.GenerateUserAuthenticationCode(uid,password);
            _m.AddUserAuthenticationCode(uid, authenticationCode);
            return new UserLoginReturnModel(uid,authenticationCode);
        }
    }

    /// <summary>
    /// 用户修改密码验证
    /// </summary>
    /// <param name="uid"></param>
    /// <param name="newPwd"></param>
    /// <returns></returns>
    public ReturnMessageModel ResetPwdConfirm(int uid, string newPwd)
    {
        return new ReturnMessageModel();
    }

    public ReturnMessageModel ResetUserEmail(string username, string email)
    {
        string sql = "update web.user set email = @email where username = @username";
        using (MySqlCommand sqlCommand = new MySqlCommand(sql, _d.GetConnection()))
        {
            sqlCommand.Parameters.AddWithValue("@username", username);
            sqlCommand.Parameters.AddWithValue("@email", email);
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


    public bool DeleteUser(string username, string password)
    {
        return true;
    }

    public ReturnMessageModel ResetPassword(string uid, string password)
    {
        throw new NotImplementedException();
    }
}