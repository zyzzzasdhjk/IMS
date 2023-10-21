﻿using IMS.Service.DataBase;
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
        Console.WriteLine(_u.LoginUser("123", "123"));
    }
}