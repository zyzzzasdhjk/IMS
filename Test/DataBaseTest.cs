using IMS.Service.DataBase;
using IMS.Service.UserServices;

namespace Test;

public class DataBaseTest
{
    private IRelationalDataBase _m;
    private IUserService _u;
    
    [SetUp]
    public void Setup()
    {
        _m = new MysqlDataBase();
        _u = new UserService(_m);
    }

    [Test]
    public void TestUserInsert()
    { 
        /*Console.WriteLine(_u.RegisterUser("admin", "123456"));*/
    }
}