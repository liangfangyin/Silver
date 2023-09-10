using COSXML;
using COSXML.Auth;
using COSXML.Model.Bucket;
using COSXML.Model.Object;
using COSXML.Model.Service;
using COSXML.Transfer;
using Silver.Basic;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Silver.OSS.Cloud
{
    public class TencentOSS
    {
        private List<string> listBucketName = new List<string>();
        private string ossEndpoint { get; set; }
        private string ossAppID { get; set; }
        private string ossAppSecret { get; set; }
        private CosXml cosXml;


        public TencentOSS()
        {
            ossAppID = ConfigurationUtil.GetSection("OSS:AppID");
            ossAppSecret = ConfigurationUtil.GetSection("OSS:AppSecret");
            ossEndpoint = ConfigurationUtil.GetSection("OSS:Endpoint");

            CosXmlConfig config = new CosXmlConfig.Builder().SetRegion(ossEndpoint).Build();
            QCloudCredentialProvider qCloudCredentialProvider = new DefaultQCloudCredentialProvider(ossAppID, ossAppSecret, 600);
            cosXml = new CosXmlServer(config, qCloudCredentialProvider);
        }

        public TencentOSS(string _ossEndpoint, string _ossAppID, string _ossAppSecret)
        {
            ossEndpoint = _ossEndpoint;
            ossAppID = _ossAppID;
            ossAppSecret = _ossAppSecret;

            CosXmlConfig config = new CosXmlConfig.Builder().SetRegion(ossEndpoint).Build();
            QCloudCredentialProvider qCloudCredentialProvider = new DefaultQCloudCredentialProvider(ossAppID, ossAppSecret, 600);
            cosXml = new CosXmlServer(config, qCloudCredentialProvider);
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
            if (IsExitesBucket(bucketName))
            {
                return false;
            }
            try
            {
                //// 存储桶名称，此处填入格式必须为 BucketName-APPID
                PutBucketRequest request = new PutBucketRequest($"{bucketName}-{ossAppID}");
                PutBucketResult result = cosXml.PutBucket(request);
                return result.httpCode == 200;
            }
            catch (Exception ex)
            {
                throw new Exception("TencentOSS.CreateBucket" + ex.Message);
            }
        }

        /// <summary>
        /// 删除存储空间
        /// </summary>
        /// <param name="bucketName"></param>
        /// <returns></returns>
        public bool DeleteBucket(string bucketName)
        {
            try
            {
                DeleteBucketRequest request = new DeleteBucketRequest($"{bucketName}-{ossAppID}");
                //执行请求
                DeleteBucketResult result = cosXml.DeleteBucket(request);
                return result.httpCode == 200;
            }
            catch (Exception ex)
            {
                throw new Exception("TencentOSS.DeleteBucket" + ex.Message);
            }
        }

        /// <summary>
        /// 存储空间列表
        /// </summary>
        /// <returns></returns>
        public List<string> ListBucket()
        {
            GetServiceRequest request = new GetServiceRequest();
            GetServiceResult result = cosXml.GetService(request);
            var resultBuckets = result.listAllMyBuckets.buckets;
            foreach (var bucket in resultBuckets)
            {
                if (listBucketName.Where(t => t == bucket.name).Count() <= 0)
                {
                    listBucketName.Add(bucket.name);
                }
            }
            return listBucketName;
        }

        /// <summary>
        /// 判断存储空间是否存在
        /// </summary>
        /// <param name="bucketName"></param>
        /// <returns></returns>
        public bool IsExitesBucket(string bucketName)
        {
            if (listBucketName.Where(t => t == bucketName).Count() > 0)
            {
                return true;
            }
            ListBucket();
            if (listBucketName.Where(t => t == bucketName).Count() > 0)
            {
                return true;
            }
            return false;
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
        public async Task<MinioUpdateResponse> PutFileAsync(string bucketName, string localFilename, string objectDictory)
        { 
            CosXmlConfig config = new CosXmlConfig.Builder().SetRegion(ossEndpoint).Build();

            string secretId = ossAppID;   // 云 API 密钥 SecretId, 获取 API 密钥请参照 https://console.tencentcloud.com/cam/capi
            string secretKey = ossAppSecret; // 云 API 密钥 SecretKey, 获取 API 密钥请参照 https://console.tencentcloud.com/cam/capi
            long durationSecond = 6000;          //每次请求签名有效时长，单位为秒
            QCloudCredentialProvider qCloudCredentialProvider = new DefaultQCloudCredentialProvider(secretId, secretKey, durationSecond);

            CosXml cosXml = new CosXmlServer(config, qCloudCredentialProvider);
            // 初始化 TransferConfig
            TransferConfig transferConfig = new TransferConfig();
            // 手动设置开始分块上传的大小阈值为10MB，默认值为5MB
            transferConfig.DivisionForUpload = 100 * 1024 * 1024;
            // 手动设置分块上传中每个分块的大小为2MB，默认值为1MB
            transferConfig.SliceSizeForUpload = 1 * 1024 * 1024; 
            Random random = new Random();
            string filesName = DateTime.Now.ToString("yyyyMMddHHmmssfff") + random.Next(10000000, 99999999) + Path.GetExtension(localFilename);
            string objectName = DateTime.Now.ToString("yyyyMMdd") + "/" + filesName;
            if (!string.IsNullOrEmpty(objectDictory))
            {
                if (!objectDictory.EndsWith("/"))
                {
                    objectDictory += "/";
                }
                objectName = objectDictory + objectName;
            }
            // 初始化 TransferManager
            TransferManager transferManager = new TransferManager(cosXml, transferConfig);
            // 存储桶名称，此处填入格式必须为 bucketname-APPID, 其中 APPID 获取参考 https://console.tencentcloud.com/developer
            String bucket = bucketName;
            String cosPath = objectName+ filesName; //对象在存储桶中的位置标识符，即称对象键
            String srcPath = localFilename;//本地文件绝对路径
           
            
            // 上传对象
            COSXMLUploadTask uploadTask = new COSXMLUploadTask(bucket, cosPath);
            uploadTask.SetSrcPath(srcPath);

            uploadTask.progressCallback = delegate (long completed, long total)
            {
                Console.WriteLine(String.Format("progress = {0:##.##}%", completed * 100.0 / total));
            };
            try
            {
                COSXML.Transfer.COSXMLUploadTask.UploadTaskResult result = await
                  transferManager.UploadAsync(uploadTask);
                Console.WriteLine(result.GetResultInfo());
                string eTag = result.eTag;
            }
            catch (Exception e)
            {
                Console.WriteLine("CosException: " + e);
            }
            MinioUpdateResponse updateResponse = new MinioUpdateResponse();
            FileInfo infoFile = new FileInfo(localFilename); 
            try
            {
                COSXMLUploadTask.UploadTaskResult result = await transferManager.UploadAsync(uploadTask);
                updateResponse.Etag = result.eTag;
                updateResponse.Size = infoFile.Length;
                updateResponse.ObjectName = $"/{bucketName}/{objectName}";
            }
            catch (Exception ex)
            {
                throw new Exception("TencentOSS.PutFileAsync" + ex.Message);
            }
            return updateResponse;
        }

        /// <summary>
        /// 文件普通上传
        /// </summary>
        /// <param name="bucketName">存储空间</param>
        /// <param name="filename">文件名：1.jpg</param>
        /// <param name="sr">本地文件流</param>
        /// <returns></returns>
        public MinioUpdateResponse PutFileStream(string bucketName, string objectDictory, Stream sr, string extension = ".jpg")
        {
            MinioUpdateResponse updateResponse = new MinioUpdateResponse();
            CreateBucket(bucketName);
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
            try
            { 
                PutObjectRequest request = new PutObjectRequest(bucketName, objectName, sr);
                request.SetCosProgressCallback(delegate (long completed, long total)
                {
                    Console.WriteLine(string.Format("progress = {0:##.##}%", completed * 100.0 / total));
                });
                PutObjectResult result = cosXml.PutObject(request);
                updateResponse.Etag = DateTime.Now.Ticks.ToString();
                updateResponse.Size = sr.Length;
                updateResponse.ObjectName = $"/{bucketName}/{objectName}";
                return updateResponse;
            }
            catch (Exception ex)
            {
                throw new Exception("TencentOSS.PutFileStream" + ex.Message);
            }
        }

        #endregion

        #region 文件下载

        /// <summary>
        /// 文件下载
        /// </summary>
        /// <param name="bucketName">存储空间名称</param>
        /// <param name="folder">OSS文件路径</param>
        /// <param name="downloadFilename">本地存储文件路径</param>
        public async Task DownFileAsync(string bucketName, string folder, string downloadFolder, string downloadFilename)
        {
            // 初始化 TransferConfig
            TransferConfig transferConfig = new TransferConfig();
            // 手动设置高级下载接口的分块阈值为 20MB(默认为20MB), 从5.4.26版本开始支持！
            transferConfig.DivisionForDownload = 20 * 1024 * 1024;
            // 初始化 TransferManager
            TransferManager transferManager = new TransferManager(cosXml, transferConfig);
            string bucket = $"{bucketName}-{ossAppID}"; //存储桶，格式：BucketName-APPID
            string cosPath = folder; //对象在存储桶中的位置标识符，即称对象键
            string localDir = downloadFolder;//本地文件夹
            string localFileName = downloadFilename; //指定本地保存的文件名 
            COSXMLDownloadTask downloadTask = new COSXMLDownloadTask(bucket, cosPath, localDir, localFileName);
            // 手动设置高级下载接口的并发数 (默认为5), 从5.4.26版本开始支持！
            //downloadTask.SetMaxTasks(10);
            downloadTask.progressCallback = delegate (long completed, long total)
            {
                Console.WriteLine(string.Format("progress = {0:##.##}%", completed * 100.0 / total));
            };
            try
            {
                COSXMLDownloadTask.DownloadTaskResult result = await transferManager.DownloadAsync(downloadTask);
            }
            catch (Exception ex)
            {
                throw new Exception("TencentOSS.DownFile" + ex.Message);
            }
        }

        #endregion

        #region 删除文件

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="bucketName">存储空间名称</param>
        /// <param name="filename">文件名称</param>
        /// <returns></returns>
        public bool DeleteFile(string bucketName, string filename)
        {
            string bucket = $"{bucketName}-{ossAppID}";
            string key = filename; //对象键
            DeleteObjectRequest request = new DeleteObjectRequest(bucket, key);
            //执行请求
            DeleteObjectResult result = cosXml.DeleteObject(request);
            return result.httpCode == 200;
        }

        #endregion

    }
}
