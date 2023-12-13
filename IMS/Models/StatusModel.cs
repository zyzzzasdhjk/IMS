namespace IMS.Models;

public enum StatusModel
{
    Success, //  成功
    ParameterInvalid = 400, // 从前端接收到参数格式错误
    ParameterError = 401, // 参数出错
    CheckCodeError = 402, // 验证码错误
    AuthorizationError = 501, // 未授权
    NonExist = 502, // 不存在
    Banned = 503, // 被封禁了
    Repeat = 504, // 重复
    Unknown = 999, // 未知错误
    Unconfirmed = 700 // 未验证账号
}