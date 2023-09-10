using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Silver.Basic
{
    /// <summary>
    /// 文件帮助类
    /// </summary>
    public static class FileUtil
    {
        #region  文件基础操作

        /// <summary>
        /// 检测指定目录是否存在
        /// </summary>
        /// <param name="directoryPath">目录的绝对路径</param>
        /// <returns></returns>
        public static bool IsExistDirectory(string directoryPath)
        {
            return Directory.Exists(directoryPath);
        }

        /// <summary>
        /// 检测指定文件是否存在,如果存在则返回true。
        /// </summary>
        /// <param name="filePath">文件的绝对路径</param>        
        public static bool IsExistFile(string filePath)
        {

            return File.Exists(filePath);
        }

        /// <summary>
        /// 获取指定目录中所有文件列表
        /// </summary>
        /// <param name="directoryPath">指定目录的绝对路径</param>        
        public static string[] GetFileNames(string directoryPath)
        {
            //如果目录不存在，则抛出异常
            if (!IsExistDirectory(directoryPath))
            {
                return new string[] { };
            }
            //获取文件列表
            return Directory.GetFiles(directoryPath);
        }

        /// <summary>
        /// 获取指定目录中所有子目录列表
        /// </summary>
        /// <param name="directoryPath">指定目录的绝对路径</param>        
        public static string[] GetDirectories(string directoryPath)
        {
            return Directory.GetDirectories(directoryPath);
        }

        /// <summary>
        /// 获取指定目录及子目录中所有文件列表
        /// </summary>
        /// <param name="directoryPath">指定目录的绝对路径</param>
        /// <param name="searchPattern">模式字符串，"*"代表0或N个字符，"?"代表1个字符。</param>
        /// <param name="isSearchChild">是否搜索子目录</param>
        public static string[] GetFileNames(string directoryPath, string searchPattern, bool isSearchChild)
        {
            //如果目录不存在，则抛出异常
            if (!IsExistDirectory(directoryPath))
            {
                return new string[] { };
            }
            if (isSearchChild)
            {
                return Directory.GetFiles(directoryPath, searchPattern, SearchOption.AllDirectories);
            }
            else
            {
                return Directory.GetFiles(directoryPath, searchPattern, SearchOption.TopDirectoryOnly);
            }
        }

        /// <summary>
        /// 复制文件夹(递归)
        /// </summary>
        /// <param name="varFromDirectory">源文件夹路径</param>
        /// <param name="varToDirectory">目标文件夹路径</param>
        public static void CopyFolder(string varFromDirectory, string varToDirectory)
        {
            Directory.CreateDirectory(varToDirectory);

            if (!Directory.Exists(varFromDirectory)) return;

            string[] directories = Directory.GetDirectories(varFromDirectory);

            if (directories.Length > 0)
            {
                foreach (string d in directories)
                {
                    CopyFolder(d, varToDirectory + d.Substring(d.LastIndexOf("\\")));
                }
            }
            string[] files = Directory.GetFiles(varFromDirectory);
            if (files.Length > 0)
            {
                foreach (string s in files)
                {
                    File.Copy(s, varToDirectory + s.Substring(s.LastIndexOf("\\")), true);
                }
            }
        }

        /// <summary>
        /// 追加文件写入
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="content">内容</param>
        public static void AppendText(string path, string content)
        {
            FileInfo fi = new FileInfo(path);
            var di = fi.Directory;
            if (!di.Exists)
            {
                di.Create();
            }
            using (StreamWriter writer = File.AppendText(path))
            {
                writer.Write(content);
            } 
        }

        /// <summary>
        /// 追加文件写入 换行
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="content">内容</param>
        public static void AppendLineText(string path, string content)
        {
            FileInfo fi = new FileInfo(path);
            var di = fi.Directory;
            if (!di.Exists)
            {
                di.Create();
            }
            File.AppendAllText(path, content);
        }

        /// <summary>
        /// 追加文件写入 多行
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="content">内容</param>
        public static void AppendLinesText(string path, List<string> content)
        {
            FileInfo fi = new FileInfo(path);
            var di = fi.Directory;
            if (!di.Exists)
            {
                di.Create();
            }
            File.AppendAllLines(path, content);
        }
         
        /// <summary>
        /// 删除指定文件夹对应其他文件夹里的文件
        /// </summary>
        /// <param name="varFromDirectory">指定文件夹路径</param>
        /// <param name="varToDirectory">对应其他文件夹路径</param>
        public static void DeleteFolderFiles(string varFromDirectory, string varToDirectory)
        {
            Directory.CreateDirectory(varToDirectory);

            if (!Directory.Exists(varFromDirectory)) return;

            string[] directories = Directory.GetDirectories(varFromDirectory);

            if (directories.Length > 0)
            {
                foreach (string d in directories)
                {
                    DeleteFolderFiles(d, varToDirectory + d.Substring(d.LastIndexOf("\\")));
                }
            }


            string[] files = Directory.GetFiles(varFromDirectory);

            if (files.Length > 0)
            {
                foreach (string s in files)
                {
                    File.Delete(varToDirectory + s.Substring(s.LastIndexOf("\\")));
                }
            }
        }

        /// <summary>
        /// 从文件的绝对路径中获取文件名( 包含扩展名 )
        /// </summary>
        /// <param name="filePath">文件的绝对路径</param>        
        public static string GetFileName(string filePath)
        {
            //获取文件的名称
            FileInfo fi = new FileInfo(filePath);
            return fi.Name;
        }

        /// <summary>
        /// 创建一个目录
        /// </summary>
        /// <param name="directoryPath">目录的绝对路径</param>
        public static void CreateDirectory(string directoryPath)
        {
            if (!IsExistDirectory(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
        }

        /// <summary>
        /// 创建一个文件。
        /// </summary>
        /// <param name="filePath">文件的绝对路径</param>
        public static void CreateFile(string filePath)
        {
            if (!IsExistFile(filePath))
            {
                File.Create(filePath);
            }
        }

        /// <summary>
        /// 将字节流写入文件。
        /// </summary>
        /// <param name="filePath">文件的绝对路径</param>
        /// <param name="buffer">二进制流数据</param>
        public static void WriteFile(string filePath, byte[] buffer)
        { 
            //如果文件不存在则创建该文件
            if (!IsExistFile(filePath))
            {
                File.Create(filePath);
            }
            FileInfo file = new FileInfo(filePath);
            //创建文件
            FileStream fs = file.Create();

            //写入二进制流
            fs.Write(buffer, 0, buffer.Length);

            //关闭文件流
            fs.Close();
        }
           
        /// <summary>
        /// 向文本文件中写入内容
        /// </summary>
        /// <param name="filePath">文件的绝对路径</param>
        /// <param name="text">写入的内容</param>
        /// <param name="encoding">编码</param>
        public static void WriteFile(string filePath, string text, Encoding encoding)
        {
            //向文件写入内容
            File.WriteAllText(filePath, text, encoding);
        }
         
        /// <summary>
        /// 将源文件的内容复制到目标文件中
        /// </summary>
        /// <param name="sourceFilePath">源文件的绝对路径</param>
        /// <param name="destFilePath">目标文件的绝对路径</param>
        public static void Copy(string sourceFilePath, string destFilePath)
        {
            File.Copy(sourceFilePath, destFilePath, true);
        }

        /// <summary>
        /// 从文件的绝对路径中获取文件名( 不包含扩展名 )
        /// </summary>
        /// <param name="filePath">文件的绝对路径</param>        
        public static string GetFileNameNoExtension(string filePath)
        {
            //获取文件的名称
            FileInfo fi = new FileInfo(filePath);
            return fi.Name.Split('.')[0];
        }

        /// <summary>
        /// 从文件的绝对路径中获取扩展名
        /// </summary>
        /// <param name="filePath">文件的绝对路径</param>        
        public static string GetExtension(string filePath)
        {
            //获取文件的名称
            FileInfo fi = new FileInfo(filePath);
            return fi.Extension;
        }

        /// <summary>
        /// 清空文件内容
        /// </summary>
        /// <param name="filePath">文件的绝对路径</param>
        public static void ClearFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                //删除文件
                File.Delete(filePath);
            }
            //重新创建该文件
            CreateFile(filePath);
        }
         
        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="filePath"></param>
        public static void DeleteFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }

        /// <summary>
        /// 获得文件Hash值
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <returns></returns>
        public static string GetFileHash(string filePath)
        {
            HashAlgorithm hash = SHA256.Create();
            using (FileStream fileStream = File.OpenRead(filePath))
            {
                byte[] HashValue = hash.ComputeHash(fileStream);
                return BitConverter.ToString(HashValue).Replace("-", "");
            }
        }

        #endregion

        #region 网络文件下载

        /// <summary>
        /// 在线文件下载
        /// </summary>
        /// <param name="url">文件网址</param>
        /// <param name="filepath">存放本地地址</param>
        /// <returns></returns>
        public static void DownFile(string url, string filepath)
        {
            filepath = filepath.Replace("\\", "/");
            string dirpath = filepath.Substring(0, filepath.LastIndexOf("/"));
            if (!Directory.Exists(dirpath))
            {
                Directory.CreateDirectory(dirpath);
            }
            WebClient myWebClient = new WebClient();
            myWebClient.DownloadFile(url, filepath);
        }

        /// <summary>
        /// 在线异步文件下载
        /// </summary>
        /// <param name="url">文件网址</param>
        /// <param name="filepath">存放本地地址</param>
        /// <returns></returns>
        public static async Task DownFileAsync(string url, string filepath)
        {
            filepath = filepath.Replace("\\", "/");
            string dirpath = filepath.Substring(0, filepath.LastIndexOf("/"));
            if (!Directory.Exists(dirpath))
            {
                Directory.CreateDirectory(dirpath);
            }
            WebClient myWebClient = new WebClient();
            await myWebClient.DownloadFileTaskAsync(url, filepath);
        }

        /// <summary>
        /// 分块文件下载
        /// </summary>
        /// <param name="urls">网络文件地址</param>
        /// <param name="path">存放文件文件夹</param>
        public static void DownFileBlock(string urls, string filepath)
        {
            filepath = filepath.Replace("\\", "/");
            string dirpath = filepath.Substring(0, filepath.LastIndexOf("/"));
            if (!Directory.Exists(dirpath))
            {
                Directory.CreateDirectory(dirpath);
            }
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(urls);
            request.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
            request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/40.0.2214.111 Safari/537.36";
            using (Stream stream = request.GetResponse().GetResponseStream())
            {
                FileStream fs = new FileStream(filepath, FileMode.Create, FileAccess.Write);
                byte[] bytes = new byte[1024 * 512];
                int readCount = 0;
                while (true)
                {
                    readCount = stream.Read(bytes, 0, bytes.Length);
                    if (readCount <= 0)
                        break;
                    fs.Write(bytes, 0, readCount);
                    fs.Flush();
                }
                fs.Close();
            }
            request.Abort();
        }

        /// <summary>
        /// 断点文件下载
        /// </summary>
        /// <param name="urls">网络文件地址</param>
        /// <param name="path">存放文件文件夹</param>
        public static void DownFilePoint(string urls, string filepath)
        {
            filepath = filepath.Replace("\\", "/");
            string dirpath = filepath.Substring(0, filepath.LastIndexOf("/"));
            if (!Directory.Exists(dirpath))
            {
                Directory.CreateDirectory(dirpath);
            }
            //创建写入流
            FileStream fs = new FileStream(filepath, FileMode.Create, FileAccess.Write);
            //指定url 下载文件
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(urls);
            long position = 0;
            //判断文件是否存在，如果存在获取文件继续的位置
            FileInfo info = new FileInfo(filepath);
            if (info.Exists)
            {
                position = info.Length;
                //将文件写入流指针移动到最后
                fs.Seek(position, SeekOrigin.Current);
            }
            //获取响应流
            //添加Range段标识
            request.AddRange(position);
            using (Stream stream = request.GetResponse().GetResponseStream())
            {
                byte[] bytes = new byte[1024 * 512];
                int readCount = 0;
                while (true)
                {
                    readCount = stream.Read(bytes, 0, bytes.Length);
                    if (readCount <= 0)
                        break;
                    fs.Write(bytes, 0, readCount);
                    fs.Flush();
                }
                fs.Close();
            }
        }

        /// <summary>
        /// 网络图片转Base64
        /// </summary>
        /// <param name="urls">网络图片地址</param>
        /// <returns></returns>
        public static string ImageUrlToBase(string urls)
        {
            string base64 = "";
            WebClient mywebclient = new WebClient();
            byte[] Bytes = mywebclient.DownloadData(urls);
            using (MemoryStream ms = new MemoryStream(Bytes))
            {
                System.Drawing.Image outputImg = System.Drawing.Image.FromStream(ms);
                base64 = ImageUtil.ImageToBase64(outputImg);
            }
            return base64;
        }

        /// <summary>
        /// Image转base64
        /// </summary>
        /// <param name="img"></param>
        /// <returns></returns>
        private static string ImageToBase64(System.Drawing.Image img)
        {
            Bitmap bmp = new Bitmap(img);
            MemoryStream ms = new MemoryStream();
            bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
            byte[] arr = new byte[ms.Length];
            ms.Position = 0;
            ms.Read(arr, 0, (int)ms.Length);
            ms.Close();
            return Convert.ToBase64String(arr);
        }

        #endregion




    }
}
