using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
namespace IMS;

public class Common
{
    public static string? ConnectString = null;
    static Common ()
    {
        string jsonfile = "./WebAppSettings.json";
 
        using (StreamReader file = File.OpenText(jsonfile))
        {
            using (JsonTextReader reader = new JsonTextReader(file))
            {
                JObject j = (JObject)JToken.ReadFrom(reader);
                
                // 数据库连接字符串
                JObject DataBaseObject = (JObject)j["DataBase"];
                ConnectString = String.Format("Database={0};Data Source={1};port={2};User Id={3};;SslMode=none;Password={4};",DataBaseObject["database"], DataBaseObject["address"], DataBaseObject["port"], DataBaseObject["username"], DataBaseObject["password"]);
                /*ConnectString = j["DataBase"].ToString();*/
            }
        }
    }
}