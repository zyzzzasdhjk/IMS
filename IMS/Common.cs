using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
namespace IMS;

public class EmailSetting
{
    public string Host { get; set; } = "";
    public int Port { get; set; } = 0;
    public string UserName { get; set; }= "";
    public string Password { get; set; }= "";
}

public class Common
{
    public static string? ConnectString = null;
    public static EmailSetting EmailServiceSetting { get; set; } = new EmailSetting();
    static Common ()
    {
        string jsonfile = "./WebAppSettings.json";
 
        using (StreamReader file = File.OpenText(jsonfile))
        {
            // 数据库连接字符串的生成
            using (JsonTextReader reader = new JsonTextReader(file))
            {
                JObject j = (JObject)JToken.ReadFrom(reader);
                
                // 数据库连接字符串
                JObject dataBaseObject = (JObject)j["DataBase"];
                ConnectString = String.Format("Database={0};Data Source={1};port={2};User Id={3};;SslMode=none;Password={4};",dataBaseObject["database"], dataBaseObject["address"], dataBaseObject["port"], dataBaseObject["username"], dataBaseObject["password"]);
                /*ConnectString = j["DataBase"].ToString();*/
                
                // 邮箱服务器连接字符串
                JObject emailSettingObject = (JObject)j["EmailSetting"];
                EmailServiceSetting.Host = emailSettingObject["host"].ToString();
                EmailServiceSetting.Port = Convert.ToInt32(emailSettingObject["port"]);
                EmailServiceSetting.UserName = emailSettingObject["usernmae"].ToString();
                EmailServiceSetting.Password = emailSettingObject["password"].ToString();
            }
        }
    }
}