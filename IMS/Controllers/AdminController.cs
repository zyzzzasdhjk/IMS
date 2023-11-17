using IMS.Models;
using IMS.Service.DataBase;
using IMS.Service.FileService;
using Microsoft.AspNetCore.Mvc;

namespace IMS.Controllers;

public class AdminController : Controller
{
    private readonly INosqlDataBase _m;

    public AdminController(INosqlDataBase m)
    {
        _m = m;
    }
    
    public JsonResult Test()
    {
        return Json("123");
    }

    /// <summary>
    /// 获取上传静态资源的权限
    /// </summary>
    /// <returns></returns>
    public JsonResult UploadStaticResources()
    {
        var o = _m.GetTmpKey("WebsiteStaticResources");
        if (o is not null)
        {
            return Json(o);
        }

        o = ObjectStorageService.GetUploadRight("WebsiteStaticResources/*");
        _m.AddTmpKey(o, "WebsiteStaticResources");
        return Json(o);
    }

    /// <summary>
    /// 上传成功后返回5个月时效的连接
    /// </summary>
    /// <returns></returns>
    public JsonResult UploadStaticResourcesEnd(string key)
    {
        if (key == "")
        {
            return Json(new ResponseModel(StatusModel.ParameterInvalid,"Key不能为空"));
        }
        return Json(new ResponseModel(StatusModel.Success,"ok",
            new
            {
                Url = ObjectStorageService.GetLongLink("WebsiteStaticResources/"+key),
                
            }
            ));
    }
}