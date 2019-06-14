using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using YunNetworkDisk.Models;

namespace YunNetworkDisk.Controllers
{
    public class MusicController : Controller
    {
        // GET: Music
        public ActionResult Index(string search)
        {
            if (search == null)//音乐界面
            {
                ViewBag.DateFiles = Maincontrol.TravelMusic();
                return View();
            }
            else               //音乐搜索界面
            {
                ViewBag.DateFiles = Maincontrol.GetFindFiledate(search);
                return View();
            }
        }
        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public FileResult Download(string name)
        {
            string path = Maincontrol.GetFullPath(name);
            string contentType = MimeMapping.GetMimeMapping(path);
            return File(path, contentType, name);
        }
        /// <summary>
        /// 播放文件名
        /// </summary>
        /// <returns></returns>
        public JsonResult Play()
        {
            return Json("/" + urlconvertor(Maincontrol.GetRelativePath(Request["name"].ToString())));
           
        }
        /// <summary>
        /// 删除文件
        /// </summary>
        /// <returns></returns>
        public ActionResult Delete()
        {
            string name = Request["name"].ToString();
            if (Filetransfer.Deletefile(Maincontrol.GetFullPath(name)))
            {
                if (Maincontrol.DeleteFloderOrFile(name))
                {
                    return Json(true);
                }
                else
                {
                    return Json(false);
                }
            }
            else
            {
                return Json(false);
            }
        }
        /// <summary>
        /// 分享文件
        /// </summary>
        /// <returns></returns>
        public JsonResult Share()
        {
            string name = Request["name"].ToString();
            if (Maincontrol.ShareFileOrFolder(Maincontrol.GetID(name), Convert.ToInt32(Request["type"].ToString()), Maincontrol.GetFullPath(name), Request["code"].ToString(), Convert.ToDouble(Request["time"].ToString())))
            {
                return Json(Request["host"].ToString() + "/" + urlconvertor(Maincontrol.GetRelativePath(name)));
            }
            return null;
        }
        private string urlconvertor(string imagesurl1)
        {
            string tmpRootDir = Server.MapPath(System.Web.HttpContext.Current.Request.ApplicationPath.ToString());//获取程序根目录
            string imagesurl2 = imagesurl1.Replace(tmpRootDir, ""); //转换成相对路径
            imagesurl2 = imagesurl2.Replace(@"\", @"/");
            return imagesurl2;
        }
    }
}