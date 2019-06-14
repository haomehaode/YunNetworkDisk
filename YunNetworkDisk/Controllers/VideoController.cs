using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using YunNetworkDisk.Models;

namespace YunNetworkDisk.Controllers
{
    public class VideoController : Controller
    {
        // GET: Video
        public ActionResult Index( string search)
        {
            if (search == null)//其他主页面
            {
                ViewBag.DateFiles = Maincontrol.TravelVideo();
                return View();
            }
            else               //其他搜索界面
            {
                ViewBag.DateFiles = Maincontrol.GetFindFiledate(search);
                return View();
            }
        }
    }
}