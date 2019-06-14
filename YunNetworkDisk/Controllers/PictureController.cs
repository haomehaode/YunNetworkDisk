using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using YunNetworkDisk.Models;

namespace YunNetworkDisk.Controllers
{
    public class PictureController : Controller
    {
        // GET: Picture
        public ActionResult Index(string search)
        {
            if (search == null)//其他主页面
            {
                ViewBag.DateFiles = Maincontrol.TravelPicture(); 
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