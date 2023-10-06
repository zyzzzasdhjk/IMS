using IMS.Service.DataBase;

namespace Test;
public class Tests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void Test1()
    {
        IRelationalDataBase m = new MysqlDataBase();
        m.Init();
        Assert.Pass();
    }
}