using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace IMS_API;

public class EmailSetting
{
    public int Port;
    public string Host { get; set; } = "";
    public string UserName { get; set; } = "";
    public string Password { get; set; } = "";
}

public class ObjectStorageSetting
{
    public string Appid { get; set; } = "";
    public string Bucket { get; set; } = "";
    public string Region { get; set; } = "";
    public string SecretId { get; set; } = "";
    public string SecretKey { get; set; } = "";
}

public class Common
{
    public static readonly string MysqlConnectString;
    public static readonly string MongoDbConnectString;
    public static readonly EmailSetting EmailServiceSetting = new();
    public static readonly ObjectStorageSetting ObjectStorageSetting = new();
    public static readonly bool NeedAuth; // 判断是否要进行用户的身份认证，默认为开启


    static Common()
    {
        var jsonfile = "./WebAppSettings.json";

        using (var file = File.OpenText(jsonfile))
        {
            // 数据库连接字符串的生成
            using (var reader = new JsonTextReader(file))
            {
                var j = (JObject)JToken.ReadFrom(reader);

                /*if (j.Property("Mysql") is null || j.Property("MongoDB") == null || j.Property("EmailSetting") == null)
                {
                    throw new Exception("配置文件不完整");
                }*/

                NeedAuth = (j["NeedAuth"] ?? bool.TrueString).ToObject<bool>();

                // Mysql数据库连接字符串
                var mysqlSetting = (JObject)(j["Mysql"] ?? new JObject());
                MysqlConnectString =
                    $"Database={mysqlSetting["database"]};" +
                    $"Data Source={mysqlSetting["address"]};" +
                    $"port={mysqlSetting["port"]};" +
                    $"User Id={mysqlSetting["username"]};" +
                    $"SslMode=none;Password={mysqlSetting["password"]};" +
                    $"Pooling=true;ConnectionTimeout=60;MaxPoolSize=200;MinPoolSize=10;";
                /*ConnectString = j["DataBase"].ToString();*/

                // MongoDB数据库连接字符串
                var mongoDbSetting = (JObject)(j["MongoDB"] ?? new JObject());
                MongoDbConnectString =
                    $"mongodb://{mongoDbSetting["username"]}:{mongoDbSetting["password"]}" +
                    $"@{mongoDbSetting["address"]}:{mongoDbSetting["port"]}/";
                // MongoDbConnectString = $"mongodb://{mongoDbSetting["address"]}:{mongoDbSetting["port"]}/";
                /*ConnectString = j["DataBase"].ToString();*/

                // 邮箱服务器连接字符串
                var emailSettingObject = (JObject)(j["EmailSetting"] ?? new JObject());
                EmailServiceSetting.Host = emailSettingObject["host"]?.ToString() ?? "";
                EmailServiceSetting.Port = Convert.ToInt32(emailSettingObject["port"]);
                EmailServiceSetting.UserName = emailSettingObject["username"]?.ToString() ?? "";
                EmailServiceSetting.Password = emailSettingObject["password"]?.ToString() ?? "";

                // 对象存储所需的数据
                var objectStorageSettingObject = (JObject)(j["OOS"] ?? new JObject());
                ObjectStorageSetting.Appid =
                    objectStorageSettingObject["appid"]?.ToString() ?? "";
                ObjectStorageSetting.Bucket =
                    objectStorageSettingObject["bucket"]?.ToString() ?? "";
                ObjectStorageSetting.Region =
                    objectStorageSettingObject["region"]?.ToString() ?? "";
                ObjectStorageSetting.SecretId =
                    objectStorageSettingObject["secretId"]?.ToString() ?? "";
                ObjectStorageSetting.SecretKey =
                    objectStorageSettingObject["secretKey"]?.ToString() ?? "";
            }
        }
    }
}