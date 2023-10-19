using IMS.Models.Team;
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
        Console.WriteLine(TeamFunction.GenerateJoinCode());
    }

    [Test]
    public void Test2()
    {
        Console.WriteLine(_t.GetAllMembers(1).ToString());
    }

    [Test]
    public void Test3()
    {
        var s = "123456789";
        Console.WriteLine(TeamFunction.CheckJoinCode(s));
    }

    [Test]
    public void Test4()
    {
        var t = new TeamInfo();
        t.tid = 1;
        t.Description = "65+656+265";
        Console.WriteLine(_t.UpdateTeam(t));
    }
}