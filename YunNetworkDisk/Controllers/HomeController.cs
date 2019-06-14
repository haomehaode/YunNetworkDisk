using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using YunNetworkDisk.Models;
using static YunNetworkDisk.Models.Filetransfer;

namespace YunNetworkDisk.Controllers
{
    public class HomeController : Controller
    {
        // GET: Main
        public ActionResult Index(string search, string name)
        {
             //  Filetransfer.DeleteFolder(Server.MapPath("~/Zip/"));
            if (search != null)
            {
                ViewBag.DateDirectories = Maincontrol.GetFindFloder(search);
                ViewBag.DateFiles = Maincontrol.GetFindFiledate(search);
                string path = AppDomain.CurrentDomain.BaseDirectory + ConfigurationManager.AppSettings["Path"];
                ViewBag.DateList = Filetransfer.GetallDirectory(new List<FileNames>(), path).ToArray();
                return View();
            }
            else if (name != null)
            {
                ViewBag.DateDirectories = Maincontrol.GetChildfolder(name);
                ViewBag.DateFiles = Maincontrol.GetChildfiledate(name);
                string path = AppDomain.CurrentDomain.BaseDirectory + ConfigurationManager.AppSettings["Path"];
                ViewBag.DateList = Filetransfer.GetallDirectory(new List<FileNames>(), path).ToArray();
                return View();
            }
            else
            {

                ViewBag.DateDirectories = Maincontrol.TravelFloder();
                ViewBag.DateFiles = Maincontrol.TravelFile();
                string path = AppDomain.CurrentDomain.BaseDirectory + ConfigurationManager.AppSettings["Path"];
                ViewBag.DateList = Filetransfer.GetallDirectory(new List<FileNames>(), path).ToArray();
                return View();

            }
        }
        /// <summary>
        /// 新建文件夹
        /// </summary>
        /// <returns></returns>
        public JsonResult NewFile()
        {
            if (Request["path"].ToString() != "false")
            {
                if (Filetransfer.NewDirectory(Maincontrol.GetFullPath(Request["path"].ToString()) + @"\" + Request["newfile"].ToString()))
                {
                    if (Maincontrol.NewFloder(Request["newfile"].ToString(), Maincontrol.GetID(Request["path"].ToString()), Maincontrol.GetRelativePath(Request["path"].ToString()) +@"\"+ Request["newfile"].ToString()))
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
            else
            {
                string name = Request["newfile"].ToString();
                string path = Server.MapPath("~/NetworkDisk/");
                if (Filetransfer.NewDirectory(path + name))
                {
                    if (Maincontrol.NewFloder(name, 1, @"NetworkDisk\" + name))
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
        }
        /// <summary>
        /// 重命名
        /// </summary>
        /// <returns></returns>
        public JsonResult NewName()
        {
            string name = Request["name"].ToString();
            if (name.Contains("."))
            {
                int idxStart = name.LastIndexOf(".");
                string newname = Request["newname"].ToString() + name.Substring(idxStart, name.Length - idxStart);
                if (Maincontrol.NewNameFloderOrFile(Maincontrol.GetFullPath(name), newname))
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
                string newname = Request["newname"].ToString();
                if (Maincontrol.NewNameFloderOrFile(Maincontrol.GetFullPath(name), newname))
                {
                    return Json(true);
                }
                else
                {
                    return Json(false);
                }
            }


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
                if(Maincontrol.DeleteFloderOrFile(name))
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
        /// 下载文件
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public FileResult Download(string name)
        {
            Filetransfer.DeleteFolder(Server.MapPath("~/Zip/"));
            if (name.Contains("."))
            {
                string path = Maincontrol.GetFullPath(name);
                string contentType = MimeMapping.GetMimeMapping(path);
                return File(path, contentType, name);
            }
            else
            {
                if (Filetransfer.ZipFile(Server.MapPath("~/NetworkDisk/" + name), Server.MapPath("~/Zip/" + name + ".zip")))
                {
                    string contentType = MimeMapping.GetMimeMapping(Server.MapPath("~/Zip/" + name + ".zip"));
                    return File(Server.MapPath("~/Zip/" + name + ".zip"), contentType);

                }
                return null;

            }
        }
        /// <summary>
        /// 移动文件
        /// </summary>
        /// <returns></returns>
        public JsonResult Move()
        {
            string name = Request["name"].ToString();
            string newname = Request["newname"].ToString();
            if (Filetransfer.Movefile(Maincontrol.GetFullPath(name), Maincontrol.GetFullPath(newname) + @"\" + Maincontrol.GetMd5Name(name)))
            {
                if (Maincontrol.MoveFile(name, Maincontrol.GetFullPath(newname)))
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
        /// 复制文件
        /// </summary>
        /// <returns></returns>
        public JsonResult Copy()
        {
            string name = Request["name"].ToString();
            string newname = Request["newname"].ToString();
            if (Filetransfer.Copyfile(Maincontrol.GetFullPath(name), Maincontrol.GetFullPath(newname)+ @"\" + Maincontrol.GetMd5Name(name)))
            {

                if (Maincontrol.CopyFile(name, Maincontrol.GetFullPath(newname)))
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
            if (name.Contains("."))
            {
                if(Maincontrol.ShareFileOrFolder(Maincontrol.GetID(name), Convert.ToInt32(Request["type"].ToString()), Maincontrol.GetFullPath(name), Request["code"].ToString(), Convert.ToDouble(Request["time"].ToString())))
                {
                    return Json(Request["host"].ToString() + "/" + urlconvertor(Maincontrol.GetRelativePath(name)));
                }
                return null;
            }
            else
            {
                if (Filetransfer.ZipFile(Maincontrol.GetFullPath(name), Maincontrol.GetFullPath(name) + ".zip"))
                {
                    if (Maincontrol.ShareFileOrFolder(Maincontrol.GetID(name),Convert.ToInt32(Request["type"].ToString()), Maincontrol.GetFullPath(name) + ".zip", Request["code"].ToString(), Convert.ToDouble(Request["time"].ToString())))
                    {
                        return Json(Request["host"].ToString() + "/" + urlconvertor(Maincontrol.GetRelativePath(name) + ".zip"));
                    }
                }
                return null;
            }
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