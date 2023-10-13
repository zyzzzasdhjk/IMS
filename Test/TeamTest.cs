using IMS.Service.DataBase;
using IMS.Service.TeamServices;

namespace Test;

public class TeamTest
{
    private IRelationalDataBase _m;
    private ITeamSqlService _t;
    
    [SetUp]
    public void Setup()
    {
        _m = new MysqlDataBase();
        _t = new TeamMysqlService(_m);
    }
    
    [Test]
    public void Test1()
    {
        Console.WriteLine(TeamFunction.GenerateTeamId());
    }

    [Test]
    public void Test2()
    {
        Console.WriteLine(_t.GetAllMembers(1).ToString());
    }
}