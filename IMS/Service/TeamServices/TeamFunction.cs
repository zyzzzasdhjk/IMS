namespace IMS.Service.TeamServices;

public class TeamFunction
{
    /// <summary>
    /// 生成团队id，9位，由大写字母和数字组成
    /// </summary>
    /// <returns></returns>
    public static string GenerateTeamId()
    {
        return Guid.NewGuid().ToString("N").Substring(8, 9).ToUpper();
    }
    
    
    
}