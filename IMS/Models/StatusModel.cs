namespace IMS.Models;

public enum StatusModel
{
    Success, //  成功
    PostError = 400, // 从前端接收到null引发的错误
    AuthorizationError = 501, // 未授权
    NonExist = 502, // 不存在
    Unknown = 999, // 未知错误
}