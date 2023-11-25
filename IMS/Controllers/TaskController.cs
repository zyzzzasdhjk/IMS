using Microsoft.AspNetCore.Mvc;

namespace IMS.Controllers;

public class TaskController : Controller
{
    public class SubmitTaskFile
    {
        public int Tid { get; set; }
        public int Uid { get; set; }
        public List<string> Path { get; set; } = new List<string>();
    }
    
    public JsonResult Index([FromBody] SubmitTaskFile s)
    {
        return Json("get");
    }
}