using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using ICSharpCode.SharpZipLib.Checksums;
using ICSharpCode.SharpZipLib.Zip;

namespace YunNetworkDisk.Models
{
    public class Filetransfer
    {
        /// <summary>
        /// 新建文件夹
        /// </summary>
        /// <param name="name">文件夹名称</param>
        /// <returns>成功与否</returns>
        public static bool NewDirectory(string path)
        {
            try
            {
                if (!Directory.Exists(path))
                {
                    // Create the directory it does not exist.
                    Directory.CreateDirectory(path);
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }

        }
        /// <summary>
        /// 删除文件夹或者文件
        /// </summary>
        /// <param name="name">文件名</param>
        /// <returns>成功or失败</returns>
        public static bool Deletefile(string path)
        {
            try
            {
                FileAttributes attr = File.GetAttributes(path);
                if (attr == FileAttributes.Directory)
                {
                    Directory.Delete(path, true);
                    return true;
                }
                else
                {
                    File.Delete(path);
                    return true;
                }

            }
            catch
            {
                return false;
            }
        }
        /// <summary>
        /// 清空文件夹
        /// </summary>
        /// <param name="dir"></param>
        public static void DeleteFolder(string dir)
        {
            foreach (string d in Directory.GetFileSystemEntries(dir))
            {
                if (File.Exists(d))
                {
                    FileInfo fi = new FileInfo(d);
                    if (fi.Attributes.ToString().IndexOf("ReadOnly") != -1)
                        fi.Attributes = FileAttributes.Normal;
                    File.Delete(d);//直接删除其中的文件  
                }
                else
                {
                    DirectoryInfo d1 = new DirectoryInfo(d);
                    if (d1.GetFiles().Length != 0)
                    {
                        DeleteFolder(d1.FullName);////递归删除子文件夹
                    }
                    Directory.Delete(d);
                }
            }
        }
        /// <summary>
        /// 复制文件
        /// </summary>
        /// <param name="oldname">旧文件名</param>
        /// <param name="newname">新文件名</param>
        /// <returns>成功or失败</returns>
        public static bool Copyfile(string oldname, string newname)
        {
            try
            {
                File.Copy(oldname, newname, true);
                return true;
            }
            catch
            {
                return false;
            }
        }
        /// <summary>
        /// 移动文件
        /// </summary>
        /// <param name="oldname">旧文件名</param>
        /// <param name="newname">新文件名</param>
        /// <returns>成功or失败</returns>
        public static bool Movefile(string oldname, string newname)
        {
            try
            {
                File.Move(oldname, newname);
                return true;
            }
            catch
            {
                return false;
            }
        }
        /// <summary>
        /// 修改文件夹或者文件名称
        /// </summary>
        /// <param name="oldname">旧文件名</param>
        /// <param name="newname">新文件名</param>
        /// <returns>成功or失败</returns>
        public static bool AlerName(string oldname, string newname)
        {
            try
            {

                if (System.IO.File.Exists(oldname))
                {
                    System.IO.File.Move(oldname, newname);
                }
                if (System.IO.Directory.Exists(oldname))
                {
                    System.IO.Directory.Move(oldname, newname)
;
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
        /// <summary>
        /// 分享链接
        /// </summary>
        /// <param name="name">文件名</param>
        /// <returns>分享路径</returns>
        public static string SharePath(string name, string path)
        {
            return path + name;
        }
        #region 树形图数据获取
        /// <summary>
        /// 文件夹类
        /// </summary>
        public class FileNames
        {
            public string text { get; set; }
            public List<FileNames> children { get; set; }
        }
        /// <summary>
        /// 递归获取数据
        /// </summary>
        /// <param name="list">集合</param>
        /// <param name="path">路径</param>
        /// <returns>数据集合</returns>
        public static List<FileNames> GetallDirectory(List<FileNames> list, string path)
        {
            DirectoryInfo root = new DirectoryInfo(path);
            var dirs = root.GetDirectories();
            if (dirs.Count() != 0)
            {
                foreach (DirectoryInfo d in dirs)
                {
                    list.Add(new FileNames
                    {
                        text = d.Name,
                        children = GetallDirectory(new List<FileNames>(), d.FullName)
                    });

                }
            }
            return list;
        }
        #endregion
        /// <summary>
        /// 压缩文件夹
        /// </summary>
        /// <param name="strFile">文价夹路径</param>
        /// <param name="strZip">压缩后路径</param>
        /// <returns>成功or失败</returns>
        public static bool ZipFile(string strFile, string strZip)
        {
            try
            {
                var len = strFile.Length;
                var strlen = strFile[len - 1];
                if (strFile[strFile.Length - 1] != Path.DirectorySeparatorChar)
                {
                    strFile += Path.DirectorySeparatorChar;
                }
                ZipOutputStream outstream = new ZipOutputStream(File.Create(strZip));
                outstream.SetLevel(6);
                zip(strFile, outstream, strFile);
                outstream.Finish();
                outstream.Close();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public static void zip(string strFile, ZipOutputStream outstream, string staticFile)
        {
            if (strFile[strFile.Length - 1] != Path.DirectorySeparatorChar)
            {
                strFile += Path.DirectorySeparatorChar;
            }
            Crc32 crc = new Crc32();
            //获取指定目录下所有文件和子目录文件名称
            string[] filenames = Directory.GetFileSystemEntries(strFile);
            //遍历文件
            foreach (string file in filenames)
            {
                if (Directory.Exists(file))
                {
                    zip(file, outstream, staticFile);
                }
                //否则，直接压缩文件
                else
                {
                    //打开文件
                    FileStream fs = File.OpenRead(file);
                    //定义缓存区对象
                    byte[] buffer = new byte[fs.Length];
                    //通过字符流，读取文件
                    fs.Read(buffer, 0, buffer.Length);
                    //得到目录下的文件（比如:D:\Debug1\test）,test
                    string tempfile = file.Substring(staticFile.LastIndexOf("\\") + 1);
                    ZipEntry entry = new ZipEntry(tempfile);
                    entry.DateTime = DateTime.Now;
                    entry.Size = fs.Length;
                    fs.Close();
                    crc.Reset();
                    crc.Update(buffer);
                    entry.Crc = crc.Value;
                    outstream.PutNextEntry(entry);
                    //写文件
                    outstream.Write(buffer, 0, buffer.Length);
                }
            }
        }

        /// <summary>
        /// 遍历解压文件夹添加数据库
        /// </summary>
        /// <param name="di">文件夹名称</param>
        public static void FindFile(DirectoryInfo di)
        {
            FileInfo[] fis = di.GetFiles();
            for (int i = 0; i < fis.Length; i++)
            {
                Maincontrol.UpdateFile(fis[i].Name, GetMD5HashFromFile(fis[i].FullName) + System.IO.Path.GetExtension(fis[i].FullName), System.IO.Path.GetExtension(fis[i].FullName).Substring(1), fis[i].Length, fis[i].DirectoryName);
              
            }
            DirectoryInfo[] dis = di.GetDirectories();
            for (int j = 0; j < dis.Length; j++)
            {
                Maincontrol.NewFloder(dis[j].Name, Maincontrol.GetID(dis[j].Parent.ToString()), Maincontrol.GetRelativePath(dis[j].Parent.ToString()) + @"\" + dis[j].Name);
                FindFile(dis[j]);
            }
        }
        /// <summary>
        /// 获取文件的MD5码
        /// </summary>
        /// <param name="fileName">传入的文件名（含路径及后缀名）</param>
        /// <returns></returns>
        public static string GetMD5HashFromFile(string fileName)
        {
            try
            {
                FileStream file = new FileStream(fileName, System.IO.FileMode.Open);
                MD5 md5 = new MD5CryptoServiceProvider();
                byte[] retVal = md5.ComputeHash(file);
                file.Close();
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < retVal.Length; i++)
                {
                    sb.Append(retVal[i].ToString("x2"));
                }
                return sb.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("GetMD5HashFromFile() fail,error:" + ex.Message);
            }
        }
    }
}