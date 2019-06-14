using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;

namespace YunNetworkDisk.Models
{
    public class Maincontrol
    {
        /// <summary>
        /// 获得全部文件夹集合
        /// </summary>
        /// <returns>全部文件夹集合</returns>
        public static List<Floder> TravelFloder()
        {
            using (mainEntities db = new mainEntities())
            {
                return db.Floder.Where(u => u.FatherDirectory == 1).ToList();
            }

        }
        /// <summary>
        /// 获得全部文件集合
        /// </summary>
        /// <returns>全部文件集合</returns>
        public static List<Filedate> TravelFile()
        {
            using (mainEntities db = new mainEntities())
            {
                return db.Filedate.Where(u => u.FatherFolder == 1).ToList();
            }

        }
        /// <summary>
        /// 获得全部文本集合
        /// </summary>
        /// <returns>全部文本集合</returns>
        public static List<Filedate > GetFile()
        {
            using (mainEntities db = new mainEntities())
            {
                return db.Filedate.Where(u => u.Type.Contains("xlsx") || u.Type.Contains("doc")|| u.Type.Contains("ppt")).ToList();
            }
        }
        /// <summary>
        /// 获得全部音乐文件
        /// </summary>
        /// <returns>全部音乐文件集合</returns>
        public static List<Filedate> TravelMusic()
        {
            using (mainEntities db = new mainEntities())
            {
                return db.Filedate.Where(u => u.Type.Contains("mp3") || u.Type.Contains("wmv")).ToList();
            }

        }
        /// <summary>
        ///  获得全部视频文件
        /// </summary>
        /// <returns>全部视频文件</returns>
        public static List<Filedate> TravelVideo()
        {
            using (mainEntities db = new mainEntities())
            {
                return db.Filedate.Where(u => u.Type.Contains("mp4")).ToList();
            }
        }
        /// <summary>
        /// 获得全部其他文件集合
        /// </summary>
        /// <returns>全部其他文件集合</returns>
        public static List<Filedate> TravelOther()
        {
            using (mainEntities db = new mainEntities())
            {
                return db.Filedate.Where(u => u.Type.Contains("exe") || u.Type.Contains("zip")|| u.Type.Contains("rar")).ToList();
            }

        }
        /// <summary>
        /// 获得照片集合
        /// </summary>
        /// <returns>全部照片集合</returns>
        public static List<Filedate> TravelPicture()
        {
            using (mainEntities db = new mainEntities())
            {
                return db.Filedate.Where(u => u.Type.Contains("png") || u.Type.Contains("jpg") || u.Type.Contains("gif")).ToList();
            }

        }
        /// <summary>
        /// 新建文件夹EF
        /// </summary>
        /// <param name="name">文件夹名称</param>
        /// <param name="directoryNum">文件夹层级</param>
        /// <param name="fatherDirectory">父文件夹ID</param>
        /// <param name="relativePath">相对路径</param>
        /// <returns>成功OR失败</returns>
        public static bool NewFloder(string name, long fatherDirectory, string relativePath)
        {
            using (mainEntities db = new mainEntities())
            {
                if (db.Floder.Where(u=>u.RelativePath==relativePath).FirstOrDefault()==null)
                {
                    Floder floder = new Floder()
                    {
                        Name = name,
                        FatherDirectory = fatherDirectory,
                        FullPath = AppDomain.CurrentDomain.BaseDirectory + relativePath,
                        RelativePath = relativePath,
                        Date = DateTime.Now.ToString()
                    };
                    db.Floder.Add(floder);
                    return db.SaveChanges() > 0 ? true : false;
                }
                else
                {
                    return false;
                }
               
            }
        }
        /// <summary>
        /// 删除文件夹或文件EF
        /// </summary>
        /// <param name="name">文件夹或文件名称</param>
        /// <returns>成功OR失败</returns>
        public static bool DeleteFloderOrFile(string name)
        {
            using (mainEntities db = new mainEntities())
            {
                var floder = db.Floder.Where(u => u.Name == name).FirstOrDefault();
                var filedate = db.Filedate.Where(u => u.Name == name).FirstOrDefault();
                if (filedate != null)
                {
                    db.Filedate.Remove(filedate);
                }
                if (floder != null)
                {
                    db.Floder.Remove(floder);
                }
                return db.SaveChanges() > 0 ? true : false;
            }
        }
        /// <summary>
        /// 上传文件EF
        /// </summary>
        /// <param name="name">文件名称</param>
        /// <param name="MD5">MD5</param>
        /// <param name="type">文件类型</param>
        /// <param name="size">文件大小</param>
        /// <param name="relativePath">文件相对路径</param>
        /// <returns>成功OR失败</returns>
        public static bool FileUpLoad(string name, string MD5, string type, double size)
        {
            using (mainEntities db = new mainEntities())
            {
                Filedate filedate = new Filedate()
                {
                    Name = name,
                    MD5Name = MD5,
                    Type = type,
                    FatherFolder = 0,
                    Size = size,
                    FullPath = AppDomain.CurrentDomain.BaseDirectory + @"NetworkDisk\" + name,
                    RelativePath = @"NetworkDisk\" + name,
                    Date = DateTime.Now.ToString()
                };
                db.Filedate.Add(filedate);
                return db.SaveChanges() > 0 ? true : false;
            }
        }
        /// <summary>
        /// 重命名文件夹EF
        /// </summary>
        /// <param name="name">旧文件名</param>
        /// <param name="newname">新文件名</param>
        /// <returns>成功OR失败</returns>
        public static bool NewNameFloderOrFile(string name, string newname)
        {
            using (mainEntities db = new mainEntities())
            {
                Floder floder = db.Floder.Where(u => u.FullPath == name).FirstOrDefault();
                if (floder != null)
                {
                    floder.Name = newname;
                }
                Filedate filedate = db.Filedate.Where(u => u.FullPath == name).FirstOrDefault();
                if (filedate != null)
                {
                    filedate.Name = newname;
                }
                return db.SaveChanges() > 0 ? true : false;
            }
        }
        /// <summary>
        /// 移动文件夹EF
        /// </summary>
        /// <param name="name">文件夹名称</param>
        /// <param name="relativePath">相对路径</param>
        /// <returns>成功OR失败</returns>
        public static bool MoveFloder(string name, string relativePath)
        {
            using (mainEntities db = new mainEntities())
            {
                Floder floder = db.Floder.Where(u => u.Name == name).FirstOrDefault();
                floder.RelativePath = relativePath;
                // floder.FullPath = path + relativePath;
                return db.SaveChanges() > 0 ? true : false;
            }
        }
        /// <summary>
        /// 移动文件EF
        /// </summary>
        /// <param name="name">文件名</param>
        /// <param name="relativePath">相对路径</param>
        /// <returns>成功OR失败</returns>
        public static bool MoveFile(string name, string fullPath)
        {
            using (mainEntities db = new mainEntities())
            {
                Floder floder = db.Floder.Where(u => u.FullPath == fullPath).FirstOrDefault();
                Filedate filedate = db.Filedate.Where(u => u.Name == name).FirstOrDefault();
                filedate.FatherFolder = floder.ID;
                filedate.FullPath = floder.FullPath + @"\" + filedate.MD5Name;
                filedate.RelativePath = floder.RelativePath + @"\" + filedate.MD5Name;
                return db.SaveChanges() > 0 ? true : false;
            }
        }
        /// <summary>
        /// 复制文件EF
        /// </summary>
        /// <param name="name"></param>
        /// <param name="fullPath"></param>
        /// <returns></returns>
        public static bool CopyFile(string name, string fullPath)
        {
            using (mainEntities db = new mainEntities())
            {
                Floder floder = db.Floder.Where(u => u.FullPath == fullPath).FirstOrDefault();
                Filedate filedate = db.Filedate.Where(u => u.Name == name).FirstOrDefault();
                filedate.FatherFolder = floder.ID;
                filedate.FullPath = floder.FullPath + @"\" + filedate.MD5Name;
                filedate.RelativePath = floder.RelativePath + @"\" + filedate.MD5Name;
                db.Filedate.Add(filedate);
                return db.SaveChanges() > 0 ? true : false;
            }
        }
        /// <summary>
        /// 获取完整路径EF
        /// </summary>
        /// <param name="name">文件夹名称</param>
        /// <returns>文件夹路径</returns>
        public static string GetFullPath(string name)
        {
            using (mainEntities db = new mainEntities())
            {
                Floder floder = db.Floder.Where(u => u.Name == name).FirstOrDefault();
                Filedate filedate = db.Filedate.Where(u => u.Name == name).FirstOrDefault();
                if (filedate != null)
                {
                    return filedate.FullPath;
                }
                return floder.FullPath;
            }
        }
        /// <summary>
        /// 获取MD5名称EF
        /// </summary>
        /// <param name="name">名字</param>
        /// <returns>MD5名称</returns>
        public static string GetMd5Name(string name)
        {
            using (mainEntities db = new mainEntities())
            {
                Filedate filedate = db.Filedate.Where(u => u.Name == name).FirstOrDefault();
                return filedate.MD5Name;
            }
        }
        /// <summary>
        /// 获得父文件夹路径
        /// </summary>
        /// <param name="name">名称</param>
        /// <returns>父文件夹路径</returns>
        public static string GetFatherPath(string name)
        {
            using (mainEntities db = new mainEntities())
            {
                Filedate filedate = db.Filedate.Where(u => u.Name == name).FirstOrDefault();
                if (filedate != null)
                {
                    Floder floder = db.Floder.Where(u => u.ID == filedate.FatherFolder).FirstOrDefault();
                    return floder.FullPath;
                }
                Floder flode = db.Floder.Where(u => u.Name == name).FirstOrDefault();
                if (flode != null)
                {
                    Floder flod = db.Floder.Where(u => u.ID == flode.ID).FirstOrDefault();
                    return flod.FullPath;
                }
                return null;
            }
        }
        /// <summary>
        /// 得到相对路径
        /// </summary>
        /// <param name="name">名称</param>
        /// <returns>路径</returns>
        public static string GetRelativePath(string name)
        {
            using (mainEntities db = new mainEntities())
            {
                Filedate filedate = db.Filedate.Where(u => u.Name == name).FirstOrDefault();
                if (filedate != null)
                {
                    return filedate.RelativePath;
                }
                Floder flode = db.Floder.Where(u => u.Name == name).FirstOrDefault();
                if (flode != null)
                {
                    Floder flod = db.Floder.Where(u => u.ID == flode.ID).FirstOrDefault();
                    return flod.RelativePath;
                }
                return null;
            }
        }
        /// <summary>
        /// 分享生成
        /// </summary>
        /// <param name="id">文件或文件夹id</param>
        /// <param name="type">公开还是私密</param>
        /// <param name="path">分享路径</param>
        /// <param name="code">分享密码</param>
        /// <param name="time">持续时间</param>
        /// <returns>成功失败</returns>
        public static bool ShareFileOrFolder( long id, int type, string path, string code, double time)
        {
            using (mainEntities db = new mainEntities())
            {
                Share share = new Share()
                {
                    FileID = id,
                    Type = type,
                    ShareCode = code,
                    StartTime = DateTime.Now.ToString(),
                    EndTime = DateTime.Now.AddDays(time).ToString(),
                    Path = path
                };
                db.Share.Add(share);
                return db.SaveChanges() > 0 ? true : false;
            }
        }
        /// <summary>
        /// 获得ID
        /// </summary>
        /// <returns>ID</returns>
        public static long GetID( string name)
        {
            using (mainEntities db = new mainEntities())
            {
                Filedate filedate = db.Filedate.Where(u => u.Name == name).FirstOrDefault();
                if (filedate != null)
                {
                    Floder floder = db.Floder.Where(u => u.ID == filedate.FatherFolder).FirstOrDefault();
                    return floder.ID;
                }
                Floder flode = db.Floder.Where(u => u.Name == name).FirstOrDefault();
                if (flode != null)
                {
                    Floder flod = db.Floder.Where(u => u.ID == flode.ID).FirstOrDefault();
                    return flod.ID;
                }
                return 0;
            }
        }
        /// <summary>
        /// 获得子文件夹
        /// </summary>
        /// <param name="name">文件名</param>
        /// <returns>子文件夹集合</returns>
        public static List<Floder> GetChildfolder(string name)
        {
            using (mainEntities db = new mainEntities())
            {
                Floder flode = db.Floder.Where(u => u.Name == name).FirstOrDefault();           
                return db.Floder.Where(u => u.FatherDirectory == flode.ID).ToList();
            }
        }
        /// <summary>
        /// 获得子文件
        /// </summary>
        /// <param name="name">文件名</param>
        /// <returns>子文件集合</returns>
        public static List<Filedate> GetChildfiledate(string name)
        {
            using (mainEntities db = new mainEntities())
            {
                Floder flode = db.Floder.Where(u => u.Name == name).FirstOrDefault();
                return db.Filedate.Where(u => u.FatherFolder == flode.ID).ToList();
            }
        }
        /// <summary>
        /// 获得查询文件夹
        /// </summary>
        /// <param name="name">文件名</param>
        /// <returns>查询文件夹集合</returns>
        public static List<Floder> GetFindFloder(string name)
        {
            using (mainEntities db = new mainEntities())
            {
                return db.Floder.Where(s => s.Name.Contains(name)).ToList();
            }
        }
        /// <summary>
        /// 获得查询文件
        /// </summary>
        /// <param name="name">文件名</param>
        /// <returns>查询文件集合</returns>
        public static List<Filedate> GetFindFiledate(string name)
        {
            using (mainEntities db = new mainEntities())
            { 
                return db.Filedate.Where(s => s.Name.Contains(name)).ToList();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="MD5"></param>
        /// <param name="type"></param>
        /// <param name="size"></param>
        /// <param name="fatherFolder"></param>
        /// <returns></returns>
        public static bool UpdateFile(string name, string MD5, string type, double size ,string fatherFolder)
        {
            using (mainEntities db = new mainEntities())
            {
                var folder = db.Floder.Where(u => u.FullPath == fatherFolder).FirstOrDefault();
                Filedate filedate = new Filedate()
                {
                    Name = name,
                    MD5Name = MD5,
                    Type = type,
                    FatherFolder = folder.ID,
                    Size = size,
                    FullPath = fatherFolder + @"\"+MD5,
                    RelativePath = folder.RelativePath + @"\" + MD5,
                    Date = DateTime.Now.ToString()
                };
                db.Filedate.Add(filedate);
                return db.SaveChanges() > 0 ? true : false;
            }
        }
        
    }
}