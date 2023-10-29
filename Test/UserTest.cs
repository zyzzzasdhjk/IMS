using IMS.Service.DataBase;
using IMS.Service.UserServices;

namespace Test;

public class UserTest
{
    private IRelationalDataBase _r;
    private IUserService _u;
    
    [SetUp]
    public void Setup()
    {
        _r = new MysqlDataBase();
        _u = new UserService(_r);
    }

    [Test]
    public void LoginTest()
    {
        Console.WriteLine(_u.LoginUser("123456", "123"));
    }

    [Test]
    public void RegisterTest()
    {
        Console.WriteLine(_u.RegisterUser("zy","123456abc","1"));
    }

    [Test]
    public void EmailTest()
    {
        EmailService e = new EmailService();
        EmailService.SendEmail("2075221336@qq.com", "这是测试邮件", "邮件内容");
    }
}