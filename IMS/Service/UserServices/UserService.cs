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
    private readonly IRelationalDataBase _d;
    private readonly INosqlDataBase _m; // 非关系型数据库

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

    public ResponseModel RegisterUser(string username, string password, string email)
    {
        /*判断用户名字是否过长或者为空*/
        /*if (username.Length > 20 || username.Length <= 0)
        {
            return new UserRegisterReturnModel(UserRegisterReturnStatus.UserNameIllegality);
        }

        if (email.Length > 30 || email.Length <= 0)
        {
            return new UserRegisterReturnModel(UserRegisterReturnStatus.EmailIllegality);
        }

        /*判断密码是否合格#1#
        if (!IsPasswordValid(password))
        {
            return new UserRegisterReturnModel(UserRegisterReturnStatus.PassWordIllegality);
        }*/

        /*判断密码合格后对密码进行SHA加密*/
        password = PasswordHasher.HashPassword(password);

        /*查询是否存在相同的账号或者是邮箱，允许邮箱重复*/
        const string sql = "select username,email from web.User " +
                           "where username = @username";
        using (var sqlCommand = new MySqlCommand(sql, _d.GetConnection()))
        {
            sqlCommand.Parameters.AddWithValue("@username", username);
            var result = sqlCommand.ExecuteReader();
            if (result.HasRows) // 当存在数据的时候，说明有邮箱或者是用户名重复了
            {
                result.Read();
                var resultUsername = result.GetValue(0).ToString();
                result.Close();
                if (resultUsername == username)
                {
                    return new ResponseModel(
                        StatusModel.Repeat,"用户名重复");
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
            return new ResponseModel(StatusModel.ParameterInvalid,"邮件格式非法");
        }

        return new ResponseModel(StatusModel.Success, "ok",uid);
    }

    public ResponseModel ResendEmail(int uid)
    {
        try
        {
            // 判断用户的状态是不是未验证
            const string sql = "select email from web.User where uid = @uid and status = 'UnConfirmed'";
            using var sqlCommand = new MySqlCommand(sql, _d.GetConnection());
            sqlCommand.Parameters.AddWithValue("@uid", uid);
            var email = sqlCommand.ExecuteScalar()?.ToString();
            
            if (email is null)
            {
                return new ResponseModel(StatusModel.NonExist, "用户不存在或者是用户账号已经验证成功");
            }
        
            // 如果上面的验证都通过了,生成一个随机的校验码
            var r = new Random();
            var checkCode = r.Next(100000000, 999999999); // 生成一个随机的校验码
            if (EmailService.SendEmail(email, "激活邮件",
                    $"你的验证码为{checkCode}，有效时间为30分钟，请尽快激活。"))
            {
                _m.AddUserConfirmCode(uid, checkCode);
            }
            else
            {
                return new ResponseModel(StatusModel.Unknown, "邮件服务异常");
            }

            return new ResponseModel(StatusModel.Success, "ok");
        }
        catch (Exception e)
        {
            return new ResponseModel(StatusModel.Unknown, "后端接口异常:" + e.Message);
        }
    }

    public ResponseModel ResetEmail(string username, string email)
    {
        try
        {
            const string sql = "update web.User set email = @email where username = @username";
            using var sqlCommand = new MySqlCommand(sql, _d.GetConnection());
            sqlCommand.Parameters.AddWithValue("@email", email);
            sqlCommand.Parameters.AddWithValue("@username", username);
            var row = sqlCommand.ExecuteNonQuery();
            return row == 1
                ? new ResponseModel(StatusModel.Success, "ok")
                : new ResponseModel(StatusModel.NonExist, "用户不存在");
        }
        catch (MySqlException e)
        {
            if (e.Number == 1062 && DataBaseFunction.Error1062Parse(e.Message) == "email")
            {
                return new ResponseModel(StatusModel.Repeat,"该邮箱已被使用");
            }
            return new ResponseModel(StatusModel.Unknown, e.Number + "  " + e.Message);
        }
        catch (Exception e)
        {
            return new ResponseModel(StatusModel.Unknown, e.Message);
        }
    }

    public ResponseModel ConfirmUser(int uid, int checkCode)
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
                    return new ResponseModel(StatusModel.Repeat, "用户已经验证过了");
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

            return new ResponseModel(StatusModel.Success);
        }

        return new ResponseModel(StatusModel.CheckCodeError, "验证码错误");
    }

    public ResponseModel LoginUser(string username, string password)
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
                return new ResponseModel(StatusModel.NonExist,"不存在该用户");
            }

            result.Read();
            var uid = Convert.ToInt32(result.GetValue(0)); // 用户ID
            var userPassword = result.GetValue(1).ToString(); // 用户密码
            var userStatus = result.GetValue(2).ToString(); // 用户状态
            result.Close(); // 及时关闭，防止出现未关闭错误

            switch (userStatus)
            {
                case "Banned":
                    return new ResponseModel(StatusModel.Banned,"账号已经被封禁");
                case "UnConfirmed":
                    return new ResponseModel(StatusModel.Unconfirmed, "用户账号未验证",
                    JObject.FromObject(new {Uid = uid}));
            }

            if (userPassword == null) //密码为空，什么数据库错误或者是账号数据错误
            {
                return new ResponseModel(StatusModel.Unknown,"用户数据异常");
            }

            if (!PasswordHasher.CheckPassword(password, userPassword))
            {
                return new ResponseModel(StatusModel.ParameterError,"账号或者密码错误");
            }

            /*生成用户认证码，用于后面的认证*/
            var authenticationCode = INosqlDataBase.GenerateUserAuthenticationCode(uid, password);
            _m.AddUserAuthenticationCode(uid, authenticationCode);
            return new ResponseModel(StatusModel.Success, "登录成功", 
                new {Uid = uid, AuthenticationCode = authenticationCode}
            );
        }
    }
    
    
    /// <summary>
    /// 修改密码验证，这一步不需要密码
    /// </summary>
    /// <param name="uid"></param>
    /// <returns></returns>
    public ResponseModel ResetPassword(int uid)
    {
        try
        {
            const string sql = "select email from web.User where uid = @uid and status = 'Normal'";
            using var sqlCommand = new MySqlCommand(sql, _d.GetConnection());
            sqlCommand.Parameters.AddWithValue("@uid", uid);
            var result = sqlCommand.ExecuteScalar();
            if (result is DBNull || result is null)
            {
                return new ResponseModel(StatusModel.NonExist, "用户不存在");
            }
            var email = result.ToString() ?? "";
        
            // 修改密码需要向邮箱发送验证邮件
            var r = new Random();
            var checkCode = r.Next(100000000, 999999999); // 生成一个随机的校验码
            if (EmailService.SendEmail(email, "激活邮件",
                    $"你的验证码为{checkCode}，有效时间为30分钟，请尽快完成验证。"))
            {
                _m.AddUserConfirmCode(uid, checkCode);
            }
            else
            {
                return new ResponseModel(StatusModel.Unknown, "邮件服务异常");
            }

            return new ResponseModel(StatusModel.Success, "ok");
        }
        catch (Exception e)
        {
            return new ResponseModel(StatusModel.Unknown, e.Message);
        }
    }

    /// <summary>
    /// 用户修改密码验证
    /// </summary>
    /// <param name="uid"></param>
    /// <param name="newPwd"></param>
    /// <param name="checkCode"></param>
    /// <returns></returns>
    public ResponseModel ResetPasswordConfirm(int uid, string newPwd,string checkCode)
    {
        try
        {
            // 修改密码需要向邮箱发送验证邮件
            if (!_m.ValidateUserConfirmCode(uid, Convert.ToInt32(checkCode))) // 验证验证码
            {
                return new ResponseModel(StatusModel.CheckCodeError,"验证码错误");
            }
            const string sql = "update web.user set password = @password where uid = @uid";
            using var sqlCommand = new MySqlCommand(sql, _d.GetConnection());
            sqlCommand.Parameters.AddWithValue("@uid", uid);
            sqlCommand.Parameters.AddWithValue("@password", PasswordHasher.HashPassword(newPwd));
            var changeRow = sqlCommand.ExecuteNonQuery();
            if (changeRow == 1)
            {
                return new ResponseModel(StatusModel.Success,"修改密码成功");
            }
            
            return new ResponseModel(StatusModel.NonExist, "该用户信息异常");
        }
        catch (Exception e)
        {
            return new ResponseModel(StatusModel.Unknown, e.Message);
        }
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

    
    public ResponseModel GetUserInfo(int uid)
    {
        const string sql = "select name, gender, birthday, description," +
                           "(select created_at from web.user where uid = @uid) " +
                           "from web.userInfo where uid = @uid";
        using var sqlCommand = new MySqlCommand(sql, _d.GetConnection());
        sqlCommand.Parameters.AddWithValue("@uid", uid);
        var result = sqlCommand.ExecuteReader();
        if (!result.HasRows)
        {
            result.Close();
            return new ResponseModel(StatusModel.NonExist, "不存在该用户");
        }

        // 读取用户信息
        result.Read();
        var name = result.GetString(0);
        var gender = result.GetString(1);
        // 需要判定null，不然会报错
        // ** is Null. This method or property cannot be called on Null values.
        var birthday = result.IsDBNull(2) 
            ? "" 
            : result.GetDateTime(2).ToString("yyyy-MM-dd");
        var description = result.IsDBNull(3) 
            ? "" 
            : result.GetString(3);
        var createdAt = result.IsDBNull(4) 
            ? "" 
            : result.GetDateTime(4).ToString("yyyy-MM-dd");
        result.Close();

        return new ResponseModel(StatusModel.Success, "获取用户信息成功",
            new
            {
                Name = name, Gender = gender, Birthday = birthday,
                Description = description, CreatedAt = createdAt
            });
    }

    public bool DeleteUser(string username, string password)
    {
        return true;
    }
}