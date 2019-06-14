using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using YunNetworkDisk.Models;
using static YunNetworkDisk.Models.Filetransfer;

namespace YunNetworkDisk.Controllers
{
    public class OtherController : Controller
    {
        // GET: Other
        public ActionResult Index(string  search)
        {
            if (search == null)//其他主页面
            {
                ViewBag.DateFiles = Maincontrol.TravelOther();
                string path = AppDomain.CurrentDomain.BaseDirectory + ConfigurationManager.AppSettings["Path"];
                ViewBag.DateList = Filetransfer.GetallDirectory(new List<FileNames>(), path).ToArray();
                return View();
            }
            else               //其他搜索界面
            {
                ViewBag.DateFiles = Maincontrol.GetFindFiledate(search);
                string path = AppDomain.CurrentDomain.BaseDirectory + ConfigurationManager.AppSettings["Path"];
                ViewBag.DateList = Filetransfer.GetallDirectory(new List<FileNames>(), path).ToArray();
                return View();
            }
          
        }
        /// <summary>
        /// 解压文件名
        /// </summary>
        /// <returns></returns>
        public JsonResult UnRAR()
        {
            string name = Request["name"].ToString();
            string newname = Request["newname"].ToString();
            if (RarHelper.UnRAR(Maincontrol.GetFullPath(name), Maincontrol.GetFullPath(newname) + @"\"+name.Split('.')[0]))
            {
                Maincontrol.NewFloder(name.Split('.')[0], Maincontrol.GetID(newname), Maincontrol.GetRelativePath(newname) + @"\" + name.Split('.')[0]);
                Filetransfer.FindFile(new DirectoryInfo(Maincontrol.GetFullPath(newname) + @"\" + name.Split('.')[0]));
                return Json(true);
            }
            else
            {
                return Json(false);
            }
        }
       
    }

}