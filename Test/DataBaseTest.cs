using IMS.Service.DataBase;
using IMS.Service.UserServices;
using MongoDB.Driver;
using MySql.Data.MySqlClient;
using ZstdSharp.Unsafe;

namespace Test;

public class DataBaseTest
{
    private IRelationalDataBase _d;
    private IUserService _u;
    private INosqlDataBase _m;
    
    [SetUp]
    public void Setup()
    {
        _d = new MysqlDataBase();
        _m = new MongoDataBase();
        _u = new UserService(_d,_m);
    }

    [Test]
    public void MongoDbTest()
    {
        /*Console.WriteLine(_m.GetUserConfirmCodeCollection().
            Find(Builders<MongoDataBase.UserRegisterConfirmCode>.Filter.Eq("CheckCode",749559721)).Any());
    */
        /*Console.WriteLine(_m.ValidateUserAuthenticationCode("asd"));*/
        _m.AddUserConfirmCode(1, 123456);
    }
    
    [Test]
    public void TestUserInsert()
    {
        try {
            string sql = "insert into web.User(username, password, status, email) " +
                         "value (@username,@password,'UnConfirmed',@email)";
            using (MySqlCommand sqlCommand = new MySqlCommand(sql, _d.GetConnection()))
            {
                sqlCommand.Parameters.AddWithValue("@username", "123");
                sqlCommand.Parameters.AddWithValue("@password", "password");
                sqlCommand.Parameters.AddWithValue("@email", "207500000000000000000000000000000000000000221336@qq.com");
                sqlCommand.ExecuteNonQuery();
            }
        }
        catch (MySqlException e)
        {
            if (e.Message.Contains("Duplicate entry"))
            {
                string duplicateEntry = e.Message.Split("key")[1];
                Console.WriteLine(duplicateEntry.Trim(new char[] {' ','\''}));
            }
            else
            {
                Console.WriteLine(e.Message == "Data too long for column 'email' at row 1");
            }
        }
    }
}