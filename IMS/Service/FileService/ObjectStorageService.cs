using COSSTS;
using COSXML;
using COSXML.Auth;
using COSXML.Model.Tag;
using IMS_API;
using Newtonsoft.Json.Linq;

namespace IMS.Service.FileService;

/// <summary>
/// 对象存储服务
/// </summary>
public class ObjectStorageService
{
    private static readonly string Appid = Common.ObjectStorageSetting.Appid;
    private static readonly string Bucket = Common.ObjectStorageSetting.Bucket;
    private static readonly string Region = Common.ObjectStorageSetting.Region;
    private static readonly string SecretId = Common.ObjectStorageSetting.SecretId;
    private static readonly string SecretKey = Common.ObjectStorageSetting.SecretKey;
    private static readonly CosXmlConfig Config;
    private static readonly QCloudCredentialProvider CosCredentialProvider;
    
    private static readonly string[] WritePolicy = {
        // 允许的操作范围，这里以上传操作为例
        "name/cos:PutObject",
        "name/cos:PostObject",
        "name/cos:InitiateMultipartUpload",
        "name/cos:ListMultipartUploads",
        "name/cos:ListParts",
        "name/cos:UploadPart",
        "name/cos:CompleteMultipartUpload"
    };

    private static readonly string[] ReadPolicy =
    {
        "name/cos:GetObject"
    };
    
    static ObjectStorageService()
    {
        Config = new CosXmlConfig.Builder()
            .IsHttps(true)  //设置默认 HTTPS 请求
            .SetRegion(Region)
            .Build();

       CosCredentialProvider = new DefaultQCloudCredentialProvider(SecretId, SecretKey, 15768000);
    }

    public static object GetReadRight(string path)
    {
        Dictionary<string, object> values = new Dictionary<string, object>();
        values.Add("bucket", Bucket);
        values.Add("region", Region);
        values.Add("allowPrefix", path);
        values.Add("allowActions", ReadPolicy);
        values.Add("durationSeconds", 1800);

        values.Add("secretId", SecretId);
        values.Add("secretKey", SecretKey);

        Dictionary<string, object> credential = STSClient.genCredential(values); //返回值说明见README.m
        var credentials = (JObject)credential["Credentials"];

        return new
        {
            TmpSecretId = credentials["TmpSecretId"]?.ToString(),
            TmpSecretKey = credentials["TmpSecretKey"]?.ToString(),
            SecurityToken = credentials["Token"]?.ToString(),
            ExpiredTime = credential["ExpiredTime"],
            StartTime = credential["StartTime"],
        };
    }
    
    
    /// <summary>
    /// 获取上传的权限
    /// </summary>
    /// <param name="path">允许存放的文件名字</param>
    /// <returns></returns>
    public static object GetUploadRight(string path)
    {
        string
            allowPrefix = path; // 这里改成允许的路径前缀，可以根据自己网站的用户登录态判断允许上传的具体路径，例子： a.jpg 或者 a/* 或者 * (使用通配符*存在重大安全风险, 请谨慎评估使用)
        

        Dictionary<string, object> values = new Dictionary<string, object>();
        values.Add("bucket", Bucket);
        values.Add("region", Region);
        values.Add("allowPrefix", allowPrefix);
        // 也可以通过 allowPrefixes 指定路径前缀的集合
        // values.Add("allowPrefixes", new string[] {
        //     "path/to/dir1/*",
        //     "path/to/dir2/*",
        // });
        values.Add("allowActions", WritePolicy);
        values.Add("durationSeconds", 3600);

        values.Add("secretId", SecretId);
        values.Add("secretKey", SecretKey);

        // 设置域名
        // values.Add("Domain", "sts.tencentcloudapi.com");

        Dictionary<string, object> credential = STSClient.genCredential(values); //返回值说明见README.m
        var credentials = (JObject)credential["Credentials"];

        return new
        {
            TmpSecretId = credentials["TmpSecretId"]?.ToString(),
            TmpSecretKey = credentials["TmpSecretKey"]?.ToString(),
            SecurityToken = credentials["Token"]?.ToString(),
            ExpiredTime = credential["ExpiredTime"],
            StartTime = credential["StartTime"],
        };
        
        /*string[] allowActions = new string[] {  // 允许的操作范围，这里以上传操作为例
            "name/cos:PutObject",
            "name/cos:PostObject",
            "name/cos:InitiateMultipartUpload",
            "name/cos:ListMultipartUploads",
            "name/cos:ListParts",
            "name/cos:UploadPart",
            "name/cos:CompleteMultipartUpload"
        };*/
    }

    public static string GetLongLink(string path)
    {
        PreSignatureStruct preSignatureStruct = new PreSignatureStruct();
        // APPID 获取参考 https://console.cloud.tencent.com/developer
        preSignatureStruct.appid = Appid;
        // 存储桶所在地域, COS 地域的简称请参照 https://cloud.tencent.com/document/product/436/6224
        preSignatureStruct.region = Region; 
        // 存储桶名称，此处填入格式必须为 bucketname-APPID, 其中 APPID 获取参考 https://console.cloud.tencent.com/developer
        preSignatureStruct.bucket = Bucket;
        preSignatureStruct.key = path; //对象键
        preSignatureStruct.httpMethod = "GET"; //HTTP 请求方法
        preSignatureStruct.isHttps = true; //生成 HTTPS 请求 URL
        preSignatureStruct.signDurationSecond = 15768000;  // 5个月 //请求签名时间为600s
        preSignatureStruct.headers = null;//签名中需要校验的 header
        preSignatureStruct.queryParameters = null; //签名中需要校验的 URL 中请求参数
        
        CosXml s = new CosXmlServer(Config, CosCredentialProvider);
        return (s.GenerateSignURL(preSignatureStruct));
    }
}