using MySql.Data.MySqlClient;

namespace IMS.Service.DataBase;

public class DataBaseFunction
{
    public static List<Dictionary<string, object>> GetJsonResult(MySqlDataReader result)
    {
        var list = new List<Dictionary<string, object>>();
        while (result.Read())
        {
            var dict = new Dictionary<string, object>();
            for (var i = 0; i < result.FieldCount; i++) dict.Add(result.GetName(i), result.GetValue(i));
            list.Add(dict);
        }

        return list;
    }


    /// <summary>
    ///     解析错误1062出现的列名
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    public static string Error1062Parse(string s)
    {
        return s.Split('\'')[^1].Split('.')[1]; // 获取字段名[]
    }
}