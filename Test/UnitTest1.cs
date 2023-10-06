using IMS.Service.DataBase;
using IMS.Service.UserServices;

namespace Test;
public class Tests
{
    private IRelationalDataBase _m;
    
    [SetUp]
    public void Setup()
    {
        _m = new MysqlDataBase();
    }

    [Test]
    public void Test1()
    {
        var u = new UserService(_m);
        Console.WriteLine(u.InsertUser("123456abc@", "123456abc@"));
        Assert.Pass();
    }
}