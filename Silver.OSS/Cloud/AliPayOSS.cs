using Aliyun.OSS;
using Silver.Basic;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Silver.OSS.Cloud
{
    /// <summary>
    /// 阿里云OSS存储
    /// </summary>
    public class AliPayOSS
    {
        private List<string> listBucketName = new List<string>();
        private string ossEndpoint { get; set; }
        private string ossAppID { get; set; }
        private string ossAppSecret { get; set; }


        public AliPayOSS()
        {
            ossEndpoint = ConfigurationUtil.GetSection("OSS:Endpoint");
            ossAppID = ConfigurationUtil.GetSection("OSS:AppID");
            ossAppSecret = ConfigurationUtil.GetSection("OSS:AppSecret");
        }

        public AliPayOSS(string _ossEndpoint, string _ossAppID, string _ossAppSecret)
        {
            ossEndpoint = _ossEndpoint;
            ossAppID = _ossAppID;
            ossAppSecret = _ossAppSecret;
        }

        #region 存储空间 
        /// <summary>
        /// 创建存储空间
        /// </summary>
        /// <param name="bucketName">空间名称</param>
        /// <returns></returns>
        public bool CreateBucket(string bucketName)
        {
            if (string.IsNullOrEmpty(bucketName))
            {
                return false;
            }
            if (DoesBucketExist(bucketName))
            {
                return false;
            }
            // 创建OSSClient实例。
            var client = new OssClient(ossEndpoint, ossAppID, ossAppSecret);
            try
            {
                // 创建存储空间。BucketName具有全局唯一性。
                client.CreateBucket(bucketName);
                SetBucketRole(bucketName);
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("AliPayOSS.CreateBucket" + ex.Message);
            }
        }

        /// <summary>
        /// 删除存储空间
        /// </summary>
        /// <param name="bucketName"></param>
        /// <returns></returns>
        public bool DeleteBucket(string bucketName)
        {
            if (string.IsNullOrEmpty(bucketName))
            {
                return false;
            }
            // 创建OSSClient实例。
            var client = new OssClient(ossEndpoint, ossAppID, ossAppSecret);
            try
            {
                client.DeleteBucket(bucketName);
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("AliPayOSS.DeleteBucket" + ex.Message);
            }
        }


        /// <summary>
        /// 存储空间列表
        /// </summary>
        /// <returns></returns>
        public List<string> ListBuckets()
        {
            // 创建OSSClient实例。
            var client = new OssClient(ossEndpoint, ossAppID, ossAppSecret);
            try
            {
                List<string> list_bucket = new List<string>();
                var buckets = client.ListBuckets();
                foreach (var bucket in buckets)
                {
                    list_bucket.Add(bucket.Name);
                    if (listBucketName.Where(t => t == bucket.Name).Count() <= 0)
                    {
                        listBucketName.Add(bucket.Name);
                    }
                }
                return list_bucket;
            }
            catch (Exception ex)
            {
                throw new Exception("AliPayOSS.ListBuckets" + ex.Message);
            }
        }

        /// <summary>
        /// 判断存储空间是否存在
        /// </summary>
        /// <param name="bucketName">空间名称</param>
        /// <returns></returns>
        public bool DoesBucketExist(string bucketName)
        {
            if (listBucketName.Where(t => t == bucketName).Count() > 0)
            {
                return true;
            }
            ListBuckets();
            if (listBucketName.Where(t => t == bucketName).Count() > 0)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 设置存储空间访问权限
        /// </summary>
        /// <param name="bucketName">空间名称</param>
        /// <param name="role">权限</param>
        /// <returns></returns>
        public bool SetBucketRole(string bucketName, CannedAccessControlList role = CannedAccessControlList.PublicReadWrite)
        {
            if (string.IsNullOrEmpty(bucketName))
            {
                return false;
            }
            var client = new OssClient(ossEndpoint, ossAppID, ossAppSecret);
            try
            {
                // 设置存储空间的访问权限为公共读。
                client.SetBucketAcl(bucketName, role);
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("AliPayOSS.SetBucketRole" + ex.Message);
            }
        }

        /// <summary>
        /// 获取存储空间访问权限
        /// </summary>
        /// <param name="bucketName"></param>
        /// <param name="role"></param>
        /// <returns></returns>
        public CannedAccessControlList GetBucketRole(string bucketName, CannedAccessControlList role = CannedAccessControlList.PublicRead)
        {
            if (string.IsNullOrEmpty(bucketName))
            {
                return CannedAccessControlList.Default;
            }
            var client = new OssClient(ossEndpoint, ossAppID, ossAppSecret);
            try
            {
                var acl = client.GetBucketAcl(bucketName);
                return acl.ACL;
            }
            catch (Exception ex)
            {
                throw new Exception("AliPayOSS.GetBucketRole" + ex.Message);
            }
        }
        #endregion

        #region 设置OSS防盗链
        /// <summary>
        /// 设置OSS防盗链
        /// </summary>
        /// <param name="urls">链接列表 如：http://*.52sye.com http://52sye.com http://www.?.52sye.com </param>
        /// <returns></returns>
        public bool BucketReferer(List<string> urls, string bucketName)
        {
            if (urls == null || urls.Count == 0)
            {
                return false;
            }
            // 创建OSSClient实例。
            var client = new OssClient(ossEndpoint, ossAppID, ossAppSecret);
            try
            {
                var refererList = new List<string>();
                // 添加Referer白名单。Referer参数支持通配符星号（*）和问号（？）。
                foreach (string url in urls)
                {
                    if (url.ToLower().IndexOf("http") == -1)
                    {
                        refererList.Add("http://" + url);
                    }
                    else
                    {
                        refererList.Add(url);
                    }
                }
                var srq = new SetBucketRefererRequest(bucketName, refererList);
                // 设置防盗链。
                client.SetBucketReferer(srq);
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("AliPayOSS.BucketReferer" + ex.Message);
            }
        }

        /// <summary>
        /// 清空OSS防盗链
        /// </summary>
        /// <param name="bucketName"></param>
        /// <returns></returns>
        public bool BucketClear(string bucketName)
        {

            var client = new OssClient(ossEndpoint, ossAppID, ossAppSecret);
            try
            {
                // 防盗链不能直接清空，需要新建一个允许空Referer的规则来覆盖之前的规则。
                var srq = new SetBucketRefererRequest(bucketName);
                client.SetBucketReferer(srq);
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("AliPayOSS.BucketClear" + ex.Message);
            }
        }

        /// <summary>
        /// 获取OSS防盗链列表
        /// </summary>
        /// <param name="bucketName"></param>
        /// <returns></returns>
        public List<string> BucketList(string bucketName)
        {
            List<string> listBucket = new List<string>();
            var client = new OssClient(ossEndpoint, ossAppID, ossAppSecret);
            try
            {
                // 获取防盗链信息。
                var rc = client.GetBucketReferer(bucketName);
                if (rc.RefererList.Referers != null)
                {
                    for (var i = 0; i < rc.RefererList.Referers.Length; i++)
                    {
                        Console.WriteLine(rc.RefererList.Referers[i]);
                        listBucket.Add(rc.RefererList.Referers[i]);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("AliPayOSS.BucketList" + ex.Message);
            }
            finally
            {
                client.SetBucketReferer(new SetBucketRefererRequest(bucketName));
            }
            return listBucket;
        }
        #endregion

        #region 文件上传 
        /// <summary>
        /// 文件普通上传
        /// </summary>
        /// <param name="folder">OSS路径</param>
        /// <param name="filename">文件名：1.jpg</param>
        /// <param name="localFilename">本地文件路径</param>
        /// <returns></returns>
        public MinioUpdateResponse PutFile(string bucketName, string localFilename, string objectDictory)
        {
            FileInfo infoFile = new FileInfo(localFilename);
            MinioUpdateResponse updateResponse = new MinioUpdateResponse();
            CreateBucket(bucketName);
            Random random = new Random();
            string objectName = DateTime.Now.ToString("yyyyMMdd") + "/" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + random.Next(10000000, 99999999) + Path.GetExtension(localFilename);
            if (!string.IsNullOrEmpty(objectDictory))
            {
                if (!objectDictory.EndsWith("/"))
                {
                    objectDictory += "/";
                }
                objectName = objectDictory + objectName;
            }
            // 创建OssClient实例。
            var client = new OssClient(ossEndpoint, ossAppID, ossAppSecret);
            try
            {
                // 上传文件。
                var result= client.PutObject(bucketName, objectName, localFilename);
                updateResponse.Etag= result.ETag;
                updateResponse.Size = infoFile.Length;
                updateResponse.ObjectName = $"/{bucketName}/{objectName}";
            }
            catch (Exception ex)
            {
                throw new Exception("AliPayOSS.PutFile" + ex.Message);
            }
            return updateResponse;
        }

        /// <summary>
        /// 文件普通上传
        /// </summary>
        /// <param name="folder">OSS路径</param>
        /// <param name="filename">文件名：1.jpg</param>
        /// <param name="localFilename">本地文件路径</param>
        /// <returns></returns>
        public MinioUpdateResponse PutFileStream(string bucketName, string objectDictory,Stream sr, string extension = ".jpg")
        {
            CreateBucket(bucketName); 
            MinioUpdateResponse updateResponse = new MinioUpdateResponse();
            Random random = new Random();
            string objectName = DateTime.Now.ToString("yyyyMMdd") + "/" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + random.Next(10000000, 99999999) + extension;
            if (!string.IsNullOrEmpty(objectDictory))
            {
                if (!objectDictory.EndsWith("/"))
                {
                    objectDictory += "/";
                }
                objectName = objectDictory + objectName;
            }
            // 创建OssClient实例。
            var client = new OssClient(ossEndpoint, ossAppID, ossAppSecret);
            try
            {
                // 上传文件。
                var result= client.PutObject(bucketName, objectName, sr);
                updateResponse.Etag = result.ETag;
                updateResponse.Size = sr.Length;
                updateResponse.ObjectName = $"/{bucketName}/{objectName}";
            }
            catch (Exception ex)
            {
                throw new Exception("AliPayOSS.PutFileStream" + ex.Message);
            }
            return updateResponse;
        }


        /// <summary>
        /// 异步文件普通上传
        /// </summary>
        /// <param name="folder">OSS路径</param>
        /// <param name="filename">文件名：1.jpg</param>
        /// <param name="localFilename">本地文件路径</param>
        /// <returns></returns>
        public async Task<bool> PutFileAsync(string bucketName, string folder, string filename, string localFilename)
        {
            CreateBucket(bucketName);
            folder = GetEditPath(folder);
            return await Task.Run(() =>
            {
                // 创建OssClient实例。
                var client = new OssClient(ossEndpoint, ossAppID, ossAppSecret);
                try
                {
                    // 上传文件。
                    client.PutObject(bucketName, folder + "/" + filename, localFilename);
                    return true;
                }
                catch (Exception ex)
                {
                    throw new Exception("AliPayOSS.PutFileAsync" + ex.Message);
                }
            });
        }

        /// <summary>
        /// 异步文件普通上传
        /// </summary>
        /// <param name="folder">OSS路径</param>
        /// <param name="filename">文件名：1.jpg</param>
        /// <param name="localFilename">本地文件路径</param>
        /// <returns></returns>
        public async Task<bool> PutFileStreamAsync(string bucketName, string folder, string filename, Stream sr)
        {
            CreateBucket(bucketName);
            folder = GetEditPath(folder);
            return await Task.Run(() =>
            {
                // 创建OssClient实例。
                var client = new OssClient(ossEndpoint, ossAppID, ossAppSecret);
                try
                {
                    // 上传文件。
                    client.PutObject(bucketName, folder + "/" + filename, sr);
                    return true;
                }
                catch (Exception ex)
                {
                    throw new Exception("AliPayOSS.PutFileStreamAsync" + ex.Message);
                }
            });
        }


        /// <summary>
        /// 分片上传
        /// </summary>
        /// <param name="folder">OSS路径</param>
        /// <param name="filename">文件名：1.jpg</param>
        /// <param name="localFilename">本地文件路径</param>
        /// <returns></returns>
        public bool PutFileLump(string bucketName, string folder, string filename, string localFilename)
        {
            CreateBucket(bucketName);
            folder = GetEditPath(folder);
            // 创建OssClient实例。
            var client = new OssClient(ossEndpoint, ossAppID, ossAppSecret);
            // 初始化分片上传。
            var uploadId = "";
            try
            {
                // 定义上传文件的名字和所属存储空间。在InitiateMultipartUploadRequest中，可以设置ObjectMeta，但不必指定其中的ContentLength。
                var request = new InitiateMultipartUploadRequest(bucketName, folder);
                var result = client.InitiateMultipartUpload(request);
                uploadId = result.UploadId;
            }
            catch (Exception ex)
            {
                throw new Exception("AliPayOSS.PutFileLump" + ex.Message);
            }
            // 计算分片总数。
            var partSize = 100 * 1024;
            var fi = new FileInfo(localFilename);
            var fileSize = fi.Length;
            var partCount = fileSize / partSize;
            if (fileSize % partSize != 0)
            {
                partCount++;
            }
            // 开始分片上传。partETags是保存partETag的列表，OSS收到用户提交的分片列表后，会逐一验证每个分片数据的有效性。 当所有的数据分片通过验证后，OSS会将这些分片组合成一个完整的文件。
            var partETags = new List<PartETag>();
            try
            {
                using (var fs = File.Open(localFilename, FileMode.Open))
                {
                    for (var i = 0; i < partCount; i++)
                    {
                        var skipBytes = (long)partSize * i;
                        // 定位到本次上传起始位置。
                        fs.Seek(skipBytes, 0);
                        // 计算本次上传的片大小，最后一片为剩余的数据大小。
                        var size = partSize < fileSize - skipBytes ? partSize : fileSize - skipBytes;
                        var request = new UploadPartRequest(bucketName, folder, uploadId)
                        {
                            InputStream = fs,
                            PartSize = size,
                            PartNumber = i + 1
                        };
                        // 调用UploadPart接口执行上传功能，返回结果中包含了这个数据片的ETag值。
                        var result = client.UploadPart(request);
                        partETags.Add(result.PartETag);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("AliPayOSS.PutFileLump" + ex.Message);
            }
            // 完成分片上传。
            try
            {
                var completeMultipartUploadRequest = new CompleteMultipartUploadRequest(bucketName, folder, uploadId);
                foreach (var partETag in partETags)
                {
                    completeMultipartUploadRequest.PartETags.Add(partETag);
                }
                var result = client.CompleteMultipartUpload(completeMultipartUploadRequest);
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("AliPayOSS.PutFileLump" + ex.Message);
            }
        }

        /// <summary>
        /// 异步分片上传
        /// </summary>
        /// <param name="folder">OSS路径</param>
        /// <param name="filename">文件名：1.jpg</param>
        /// <param name="localFilename">本地文件路径</param>
        /// <returns></returns>
        public async Task<bool> PutFileLumpAsync(string bucketName, string folder, string filename, string localFilename)
        {
            CreateBucket(bucketName);
            folder = GetEditPath(folder);
            return await Task.Run(() =>
            {
                // 创建OssClient实例。
                var client = new OssClient(ossEndpoint, ossAppID, ossAppSecret);
                // 初始化分片上传。
                var uploadId = "";
                try
                {
                    // 定义上传文件的名字和所属存储空间。在InitiateMultipartUploadRequest中，可以设置ObjectMeta，但不必指定其中的ContentLength。
                    var request = new InitiateMultipartUploadRequest(bucketName, folder);
                    var result = client.InitiateMultipartUpload(request);
                    uploadId = result.UploadId;
                }
                catch (Exception ex)
                {
                    throw new Exception("AliPayOSS.PutFileLumpAsync" + ex.Message);
                }
                // 计算分片总数。
                var partSize = 100 * 1024;
                var fi = new FileInfo(localFilename);
                var fileSize = fi.Length;
                var partCount = fileSize / partSize;
                if (fileSize % partSize != 0)
                {
                    partCount++;
                }
                // 开始分片上传。partETags是保存partETag的列表，OSS收到用户提交的分片列表后，会逐一验证每个分片数据的有效性。 当所有的数据分片通过验证后，OSS会将这些分片组合成一个完整的文件。
                var partETags = new List<PartETag>();
                try
                {
                    using (var fs = File.Open(localFilename, FileMode.Open))
                    {
                        for (var i = 0; i < partCount; i++)
                        {
                            var skipBytes = (long)partSize * i;
                            // 定位到本次上传起始位置。
                            fs.Seek(skipBytes, 0);
                            // 计算本次上传的片大小，最后一片为剩余的数据大小。
                            var size = partSize < fileSize - skipBytes ? partSize : fileSize - skipBytes;
                            var request = new UploadPartRequest(bucketName, folder, uploadId)
                            {
                                InputStream = fs,
                                PartSize = size,
                                PartNumber = i + 1
                            };
                            // 调用UploadPart接口执行上传功能，返回结果中包含了这个数据片的ETag值。
                            var result = client.UploadPart(request);
                            partETags.Add(result.PartETag);
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("AliPayOSS.PutFileLumpAsync" + ex.Message);
                }
                // 完成分片上传。
                try
                {
                    var completeMultipartUploadRequest = new CompleteMultipartUploadRequest(bucketName, folder, uploadId);
                    foreach (var partETag in partETags)
                    {
                        completeMultipartUploadRequest.PartETags.Add(partETag);
                    }
                    var result = client.CompleteMultipartUpload(completeMultipartUploadRequest);
                    return true;
                }
                catch (Exception ex)
                {
                    throw new Exception("AliPayOSS.PutFileLumpAsync" + ex.Message);
                }
            });
        }

        #endregion

        #region 文件下载

        /// <summary>
        /// 流式文件下载
        /// </summary>
        /// <param name="bucketName">存储空间名称</param>
        /// <param name="folder">OSS文件路径</param>
        /// <param name="downloadFilename">本地存储文件路径</param>
        public void DownFile(string bucketName, string folder, string downloadFilename)
        {
            folder = GetEditPath(folder);
            // 创建OssClient实例。
            var client = new OssClient(ossEndpoint, ossAppID, ossAppSecret);
            try
            {
                // 下载文件到流。OssObject 包含了文件的各种信息，如文件所在的存储空间、文件名、元信息以及一个输入流。
                var obj = client.GetObject(bucketName, folder);
                using (var requestStream = obj.Content)
                {
                    byte[] buf = new byte[1024];
                    var fs = File.Open(downloadFilename, FileMode.OpenOrCreate);
                    var len = 0;
                    // 通过输入流将文件的内容读取到文件或者内存中。
                    while ((len = requestStream.Read(buf, 0, 1024)) != 0)
                    {
                        fs.Write(buf, 0, len);
                    }
                    fs.Close();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("AliPayOSS.DownFile" + ex.Message);
            }
        }

        /// <summary>
        /// 异步流式文件下载
        /// </summary>
        /// <param name="bucketName">存储空间名称</param>
        /// <param name="folder">OSS文件路径</param>
        /// <param name="downloadFilename">本地存储文件路径</param>
        public async Task DownFileAsync(string bucketName, string folder, string downloadFilename)
        {
            folder = GetEditPath(folder);
            await Task.Run(() =>
            {
                // 创建OssClient实例。
                var client = new OssClient(ossEndpoint, ossAppID, ossAppSecret);
                try
                {
                    // 下载文件到流。OssObject 包含了文件的各种信息，如文件所在的存储空间、文件名、元信息以及一个输入流。
                    var obj = client.GetObject(bucketName, folder);
                    using (var requestStream = obj.Content)
                    {
                        byte[] buf = new byte[1024];
                        var fs = File.Open(downloadFilename, FileMode.OpenOrCreate);
                        var len = 0;
                        // 通过输入流将文件的内容读取到文件或者内存中。
                        while ((len = requestStream.Read(buf, 0, 1024)) != 0)
                        {
                            fs.Write(buf, 0, len);
                        }
                        fs.Close();
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("AliPayOSS.DownFileAsync" + ex.Message);
                }
            });
        }

        /// <summary>
        /// 文件下载并转换Stream
        /// </summary>
        /// <param name="bucketName"></param>
        /// <param name="folder"></param>
        /// <returns></returns>
        public Stream DownFileStream(string bucketName, string folder)
        {
            folder = GetEditPath(folder);
            // 创建OssClient实例。
            var client = new OssClient(ossEndpoint, ossAppID, ossAppSecret);
            var blob = client.GetObject(bucketName, folder);
            return blob.Content;
        }

        #endregion

        #region 列表 

        /// <summary>
        /// 文件是否存在
        /// </summary>
        /// <param name="bucketName"></param>
        /// <param name="objectName"></param>
        /// <returns></returns>
        public bool FileExites(string bucketName, string objectName)
        {
            var client = new OssClient(ossEndpoint, ossAppID, ossAppSecret);
            try
            {
                // 判断文件是否存在。
                var exist = client.DoesObjectExist(bucketName, objectName);
                return exist;
            }
            catch (Exception ex)
            {
                throw new Exception("AliPayOSS.FileExites" + ex.Message);
            }
        }

        /// <summary>
        /// 删除多个文件，一次最多1000个文件,并返回成功的文件
        /// </summary>
        /// <param name="bucketName"></param>
        /// <param name="fileall"></param>
        public List<string> FileDeleteAll(string bucketName, List<string> keys)
        {
            var listsuccess = new List<string>();
            var client = new OssClient(ossEndpoint, ossAppID, ossAppSecret);
            try
            {
                var listResult = client.ListObjects(bucketName);
                foreach (var summary in listResult.ObjectSummaries)
                {
                    keys.Add(summary.Key);
                }
                // quietMode为true表示简单模式，为false表示详细模式。默认为详细模式。
                var quietMode = false;
                // DeleteObjectsRequest的第三个参数指定返回模式。
                var request = new DeleteObjectsRequest(bucketName, keys, quietMode);
                // 删除多个文件。
                var result = client.DeleteObjects(request);
                if (!quietMode && result.Keys != null)
                {
                    foreach (var obj in result.Keys)
                    {
                        listsuccess.Add(obj.Key);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("AliPayOSS.FileDeleteAll" + ex.Message);
            }
            return listsuccess;
        }

        /// <summary>
        /// 文件列表
        /// </summary>
        /// <param name="bucketName">存储空间名</param>
        /// <returns></returns>
        public List<string> FileList(string bucketName, int maxkeys = 1000000)
        {
            List<string> listfile = new List<string>();
            var client = new OssClient(ossEndpoint, ossAppID, ossAppSecret);
            try
            {
                ObjectListing result = null;
                string nextMarker = string.Empty;
                do
                {
                    // 每页列举的文件个数通过maxKeys指定，超过指定数将进行分页显示。
                    var listObjectsRequest = new ListObjectsRequest(bucketName)
                    {
                        Marker = nextMarker,
                        MaxKeys = maxkeys
                    };
                    result = client.ListObjects(listObjectsRequest);
                    foreach (var summary in result.ObjectSummaries)
                    {
                        listfile.Add(summary.Key);
                    }
                    nextMarker = result.NextMarker;
                } while (result.IsTruncated);
            }
            catch (Exception ex)
            {
                throw new Exception("AliPayOSS.FileList" + ex.Message);
            }
            return listfile;
        }

        /// <summary>
        /// 修复路径不规范问题
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private static string GetEditPath(string path)
        {
            if (path.StartsWith("/"))
            {
                path = path.Substring(1);
            }
            if (path.EndsWith("/"))
            {
                path = path.Substring(0, path.Length - 1);
            }
            return path;
        }


        #endregion

    }
}
