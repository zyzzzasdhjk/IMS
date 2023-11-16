using COSSTS;
using Newtonsoft.Json.Linq;

namespace IMS.Service.FileService;

/// <summary>
/// 对象存储服务
/// </summary>
public class ObjectStorageService
{
    private static readonly string Bucket = Common.ObjectStorageSetting.Bucket;
    private static readonly string Region = Common.ObjectStorageSetting.Region;
    private static readonly string SecretId = Common.ObjectStorageSetting.SecretId;
    private static readonly string SecretKey = Common.ObjectStorageSetting.SecretKey;

    /// <summary>
    /// 获取上传的权限
    /// </summary>
    /// <param name="path">文件要存放的路径</param>
    /// <returns></returns>
    public static object GetUploadRight(string path)
    {
        string
            allowPrefix = path + "/*"; // 这里改成允许的路径前缀，可以根据自己网站的用户登录态判断允许上传的具体路径，例子： a.jpg 或者 a/* 或者 * (使用通配符*存在重大安全风险, 请谨慎评估使用)
        string[] allowActions =
        {
            // 允许的操作范围，这里以上传操作为例
            "name/cos:PutObject",
            "name/cos:PostObject",
            "name/cos:InitiateMultipartUpload",
            "name/cos:ListMultipartUploads",
            "name/cos:ListParts",
            "name/cos:UploadPart",
            "name/cos:CompleteMultipartUpload"
        };
        
        Dictionary<string, object> values = new Dictionary<string, object>();
        values.Add("bucket", Bucket);
        values.Add("region", Region);
        values.Add("allowPrefix", allowPrefix);
        // 也可以通过 allowPrefixes 指定路径前缀的集合
        // values.Add("allowPrefixes", new string[] {
        //     "path/to/dir1/*",
        //     "path/to/dir2/*",
        // });
        values.Add("allowActions", allowActions);
        values.Add("durationSeconds", 1800);

        values.Add("secretId", SecretId);
        values.Add("secretKey", SecretKey);

        // 设置域名
        // values.Add("Domain", "sts.tencentcloudapi.com");

        Dictionary<string, object> credential = STSClient.genCredential(values); //返回值说明见README.m
        var credentials = (JObject)credential["Credentials"];

        return new
        {
            tmpSecretId = credentials["TmpSecretId"]?.ToString(),
            tmpSecretKey = credentials["TmpSecretKey"]?.ToString(),
            sessionToken = credentials["Token"]?.ToString(),
            expiredTime = credential["ExpiredTime"],
            expiration = credential["Expiration"],
            startTime = credential["StartTime"],
        };
    }
}