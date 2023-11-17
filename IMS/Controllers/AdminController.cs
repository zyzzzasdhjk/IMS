using IMS.Service.FileService;
using Microsoft.AspNetCore.Mvc;

namespace IMS.Controllers;

public class AdminController : Controller
{
    /// <summary>
    /// 获取上传静态资源的权限
    /// </summary>
    /// <returns></returns>
    public JsonResult UploadStaticResources()
    {
        return Json(ObjectStorageService.GetUploadRight("WebsiteStaticResources/*"));
    }

    /// <summary>
    /// 上传成功后返回3个月时效的连接
    /// </summary>
    /// <returns></returns>
    public JsonResult UploadStaticResourcesEnd()
    {
        return Json(ObjectStorageService.GetLongLink("UserImage/47.jpg"));
    }
}