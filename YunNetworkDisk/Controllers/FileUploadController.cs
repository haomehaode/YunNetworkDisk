using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using YunNetworkDisk.Models;

namespace YunNetworkDisk.Controllers
{
    public class FileUploadController : Controller
    {
        /// <summary>
        /// 文件存放路径
        /// </summary>
        const string UploadFiles = "/NetworkDisk/";
        const string temporary = "/temporary/";
        // GET: FileUpload
        public ActionResult Index()
        {
            return View();
        }
        #region 获取指定文件的已上传的文件块
        /// <summary>
        /// 获取指定文件的已上传的文件块
        /// </summary>
        /// <returns></returns>
        public string GetMaxChunk()
        {
            string root = Server.MapPath(temporary);
            try
            {
                var md5 = Convert.ToString(Request["md5"]);
                var ext = Convert.ToString(Request["ext"]);
                int chunk = 0;

                var fileName = md5 + "." + ext;

                FileInfo file = new FileInfo(root + fileName);
                if (file.Exists)
                {
                    chunk = Int32.MaxValue;
                }
                else
                {
                    if (Directory.Exists(root + "chunk\\" + md5))
                    {
                        DirectoryInfo dicInfo = new DirectoryInfo(root + "chunk\\" + md5);
                        var files = dicInfo.GetFiles();
                        chunk = files.Count();
                        if (chunk > 1)
                        {
                            chunk = chunk - 1; //当文件上传中时，页面刷新，上传中断，这时最后一个保存的块的大小可能会有异常，所以这里直接删除最后一个块文件
                        }
                    }
                }

                return CommonResult.ToJsonStr(0, string.Empty, chunk);
            }
            catch
            {
                return CommonResult.ToJsonStr(0, string.Empty, 0);
            }
        }
        #endregion

        #region 上传文件
        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="fileData"></param>
        /// <returns></returns>
        public string Upload(HttpPostedFileBase file)
        {
            string s = Request.Files[0].FileName;
            string root = Server.MapPath(UploadFiles);
            string md5_key = string.Format("{0}md5", Request["id"]);
            string md5_val = Request[md5_key];
            //如果进行了分片
            if (Request.Form.AllKeys.Any(m => m == "chunk"))
            {
                root = Server.MapPath(temporary);
                //取得chunk和chunks
                int chunk = Convert.ToInt32(Request.Form["chunk"]);//当前分片在上传分片中的顺序（从0开始）
                int chunks = Convert.ToInt32(Request.Form["chunks"]);//总分片数
                //根据GUID创建用该GUID命名的临时文件夹
                //string folder = Server.MapPath("~/UploadFiles/" + Request["md5"] + "/");
                string folder = root + "chunk\\" + md5_val + "\\";
                string path = folder + chunk;

                //建立临时传输文件夹
                if (!Directory.Exists(Path.GetDirectoryName(folder)))
                {
                    Directory.CreateDirectory(folder);
                }

                FileStream addFile = null;
                BinaryWriter AddWriter = null;
                Stream stream = null;
                BinaryReader TempReader = null;

                try
                {
                    //addFile = new FileStream(path, FileMode.Append, FileAccess.Write);
                    addFile = new FileStream(path, FileMode.Create, FileAccess.Write);
                    AddWriter = new BinaryWriter(addFile);
                    //获得上传的分片数据流
                    stream = file.InputStream;
                    TempReader = new BinaryReader(stream);
                    //将上传的分片追加到临时文件末尾
                    AddWriter.Write(TempReader.ReadBytes((int)stream.Length));
                }
                finally
                {
                    if (addFile != null)
                    {
                        addFile.Close();
                        addFile.Dispose();
                    }
                    if (AddWriter != null)
                    {
                        AddWriter.Close();
                        AddWriter.Dispose();
                    }
                    if (stream != null)
                    {
                        stream.Close();
                        stream.Dispose();
                    }
                    if (TempReader != null)
                    {
                        TempReader.Close();
                        TempReader.Dispose();
                    }
                }
                return CommonResult.ToJsonStr(0, string.Empty, "{\"chunked\" : true, \"ext\" : \"" + Path.GetExtension(file.FileName) + "\"}");
            }
            else//没有分片直接保存
            {
                //根据md5加密后的文件名保存
                string path = root + md5_val + Path.GetExtension(Request.Files[0].FileName);
                //根据上传的文件名保存
                //string path = root + Request.Files[0].FileName;
                file.SaveAs(path);
                return CommonResult.ToJsonStr(0, string.Empty, "{\"chunked\" : false}");
            }
        }
        #endregion

        #region 合并文件
        /// <summary>
        /// 合并文件
        /// </summary>
        /// <returns></returns>
        public string MergeFiles(HttpPostedFileBase File)
        {
            string root = Server.MapPath(temporary);
            string guid = Request["md5"];
            string ext = Request["ext"];
            string sourcePath = Path.Combine(root, "chunk\\" + guid + "\\");//源数据文件夹
            string targetPath = Path.Combine(Server.MapPath(UploadFiles), guid + ext);//合并后的文件
            DirectoryInfo dicInfo = new DirectoryInfo(sourcePath);
            if (Directory.Exists(Path.GetDirectoryName(sourcePath)))
            {
                FileInfo[] files = dicInfo.GetFiles();
                foreach (FileInfo file in files.OrderBy(f => int.Parse(f.Name)))
                {
                    FileStream addFile = new FileStream(targetPath, FileMode.Append, FileAccess.Write);
                    BinaryWriter AddWriter = new BinaryWriter(addFile);

                    //获得上传的分片数据流 
                    Stream stream = file.Open(FileMode.Open);
                    BinaryReader TempReader = new BinaryReader(stream);
                    //将上传的分片追加到临时文件末尾
                    AddWriter.Write(TempReader.ReadBytes((int)stream.Length));
                    //关闭BinaryReader文件阅读器
                    TempReader.Close();
                    stream.Close();
                    AddWriter.Close();
                    addFile.Close();

                    TempReader.Dispose();
                    stream.Dispose();
                    AddWriter.Dispose();
                    addFile.Dispose();
                }
                DeleteFolder(sourcePath);
                return CommonResult.ToJsonStr(0, string.Empty, "{\"chunked\" : true, \"hasError\" : false, \"savePath\" :\"" + System.Web.HttpUtility.UrlEncode(targetPath) + "\"}");
            }
            else
            {
                return CommonResult.ToJsonStr(0, string.Empty, "{\"hasError\" : true}");
            }
        }
        #endregion

        #region 删除文件夹及其内容
        /// <summary>
        /// 删除文件夹及其内容
        /// </summary>
        /// <param name="dir"></param>
        private void DeleteFolder(string strPath)
        {
            //删除这个目录下的所有子目录
            if (Directory.GetDirectories(strPath).Length > 0)
            {
                foreach (string fl in Directory.GetDirectories(strPath))
                {
                    Directory.Delete(fl, true);
                }
            }
            //删除这个目录下的所有文件
            if (Directory.GetFiles(strPath).Length > 0)
            {
                foreach (string f in Directory.GetFiles(strPath))
                {
                    System.IO.File.Delete(f);
                }
            }
            Directory.Delete(strPath, true);
        }
        #endregion

        #region 更新用户上传记录
        /// <summary>
        /// 更新用户上传记录
        /// </summary>
        /// <returns></returns>
        public JsonResult AddUploadRecord()
        {
            using (mainEntities db = new mainEntities())
            {
                Filedate filedate = new Filedate()
                {
                    Name = Request["name"].ToString(),
                    MD5Name = Request["md5"].ToString(),
                    Size = Convert.ToInt32(Request["size"].ToString()),
                    Type = Request["type"].ToString(),
                    FatherFolder = 1,
                    RelativePath = @"NetworkDisk\" + Request["md5"].ToString(),
                    FullPath = AppDomain.CurrentDomain.BaseDirectory + @"NetworkDisk\" + Request["md5"].ToString(),
                    Date = DateTime.Now.ToString()
                };
                db.Filedate.Add(filedate);
                return Json(db.SaveChanges() > 0 ? true : false);
            }
        }
        #endregion
    }
}