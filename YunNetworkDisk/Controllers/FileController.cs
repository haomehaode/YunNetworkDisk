using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using YunNetworkDisk.Models;

namespace YunNetworkDisk.Controllers
{
    public class FileController : Controller
    {

        // GET: File
        /// <summary>
        /// 主页和搜索
        /// </summary>
        /// <param name="search">关键词</param>
        /// <returns>页面</returns>
        public ActionResult Index(string search)
        {
            if (search == null)//主页面
            {
                ViewBag.DateFiles = Maincontrol.GetFile();
                return View();
            }
            else               //搜索界面
            {
                ViewBag.DateFiles = Maincontrol.GetFindFiledate(search);
                return View();
            }
        }
    }
}