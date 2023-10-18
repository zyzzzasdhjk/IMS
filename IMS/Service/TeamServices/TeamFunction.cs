using System.Text.RegularExpressions;

namespace IMS.Service.TeamServices;

public class TeamFunction
{
    /// <summary>
    /// 生成团队id，9位，由大写字母和数字组成
    /// </summary>
    /// <returns></returns>
    public static string GenerateJoinCode()
    {
        return Guid.NewGuid().ToString("N").Substring(8, 9).ToUpper();
    }

    /// <summary>
    /// 校验用户所给的JoinCode是否是有效的
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    public static bool CheckJoinCode(string s)
    {
        if (s.Length == 9)
        {
            Regex regex = new Regex(@"^(?=.*[A-Z])(?=.*\d)(?!.*[a-z]).{9}$");
            return regex.IsMatch(s);
        }
        return false;
    }
}