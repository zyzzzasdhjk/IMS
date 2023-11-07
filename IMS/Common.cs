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
    public static readonly string MysqlConnectString;
    public static readonly string MongoDBConnectString;
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
                
                // Mysql数据库连接字符串
                JObject mysqlSetting = (JObject)j["Mysql"];
                MysqlConnectString = String.Format("Database={0};Data Source={1};port={2};User Id={3};;SslMode=none;Password={4};",mysqlSetting["database"], mysqlSetting["address"], mysqlSetting["port"], mysqlSetting["username"], mysqlSetting["password"]);
                /*ConnectString = j["DataBase"].ToString();*/
                
                // MongoDB数据库连接字符串
                JObject mongoDBSetting = (JObject)j["MongoDB"];
                MongoDBConnectString = String.Format("mongodb://{0}:{1}", mongoDBSetting["address"], mongoDBSetting["port"]);
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