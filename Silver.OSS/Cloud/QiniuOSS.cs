using Qiniu.Http;
using Qiniu.Storage;
using Qiniu.Util;
using Silver.Basic;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Silver.OSS.Cloud
{
    //华 东 ZONE_CN_East
    //华 北 ZONE_CN_North
    //华 南 ZONE_CN_South
    //北 美 ZONE_US_North
    //东南亚 ZONE_AS_Singapore
    public class QiniuOSS
    {
        private Zone Zone = null;
        private string ossEndpoint { get; set; }
        private string ossAppID { get; set; }
        private string ossAppSecret { get; set; }

        public QiniuOSS()
        {
            ossAppID = ConfigurationUtil.GetSection("OSS:AppID");
            ossAppSecret = ConfigurationUtil.GetSection("OSS:AppSecret");
            ossEndpoint = ConfigurationUtil.GetSection("OSS:Endpoint");
            switch (ossEndpoint)
            {
                case "ZONE_CN_East":
                    Zone= Zone.ZONE_CN_East_2;
                    break;
                case "ZONE_CN_North":
                    Zone = Zone.ZONE_CN_North;
                    break;
                case "ZONE_CN_South":
                    Zone = Zone.ZONE_CN_South;
                    break;
                case "ZONE_US_North":
                    Zone = Zone.ZONE_US_North;
                    break;
                case "ZONE_AS_Singapore":
                    Zone = Zone.ZONE_AS_Singapore;
                    break;
            }
        }

        public QiniuOSS(string _ossEndpoint,string _ossAppID, string _ossAppSecret)
        {
            ossAppID = _ossAppID;
            ossAppSecret = _ossAppSecret;
            ossEndpoint = _ossEndpoint;
            switch (ossEndpoint)
            {
                case "ZONE_CN_East":
                    Zone = Zone.ZONE_CN_East_2;
                    break;
                case "ZONE_CN_North":
                    Zone = Zone.ZONE_CN_North;
                    break;
                case "ZONE_CN_South":
                    Zone = Zone.ZONE_CN_South;
                    break;
                case "ZONE_US_North":
                    Zone = Zone.ZONE_US_North;
                    break;
                case "ZONE_AS_Singapore":
                    Zone = Zone.ZONE_AS_Singapore;
                    break;
            }
        }

        #region 文件上传

        /// <summary>
        /// 文件普通上传
        /// </summary> 
        /// <param name="filename">文件名：1.jpg</param>
        /// <param name="localFilename">本地文件路径</param>
        /// <returns></returns>
        public MinioUpdateResponse PutFile(string bucketName, string localFilename, string objectDictory)
        {
            System.IO.FileInfo infoFile = new System.IO.FileInfo(localFilename); 
            MinioUpdateResponse updateResponse = new MinioUpdateResponse();
            // 存储桶名称，此处填入格式必须为 bucketname-APPID, 其中 APPID 获取参考 https://console.cloud.tencent.com/developer 
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
            Mac mac = new Mac(ossAppID, ossAppSecret); 
            // 设置上传策略
            PutPolicy putPolicy = new PutPolicy();
            // 设置要上传的目标空间
            putPolicy.Scope = bucketName;
            // 生成上传token
            string token = Auth.CreateUploadToken(mac, putPolicy.ToJsonString());
            Config config = new Config();
            config.Zone = Zone;
            config.UseHttps = true;
            config.UseCdnDomains = true;
            config.ChunkSize = ChunkUnit.U512K;
            FormUploader target = new FormUploader(config);
            HttpResult result = target.UploadFile(localFilename, objectName, token, null);
            if (result.Code == (int)HttpCode.OK)
            {
                updateResponse.Etag = DateTime.Now.Ticks.ToString();
                updateResponse.Size = infoFile.Length;
                updateResponse.ObjectName = $"/{bucketName}/{objectName}";
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
        public MinioUpdateResponse PutFileStream(string bucketName, string objectDictory, Stream sr, string extension = ".jpg")
        {
            MinioUpdateResponse updateResponse = new MinioUpdateResponse();
            Mac mac = new Mac(ossAppID, ossAppSecret);
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
            // 设置上传策略
            PutPolicy putPolicy = new PutPolicy();
            // 设置要上传的目标空间
            putPolicy.Scope = bucketName;
            // 生成上传token
            string token = Auth.CreateUploadToken(mac, putPolicy.ToJsonString());
            Config config = new Config();
            config.Zone = Zone;
            config.UseHttps = true;
            config.UseCdnDomains = true;
            config.ChunkSize = ChunkUnit.U512K;
            FormUploader target = new FormUploader(config);
            HttpResult result = target.UploadStream(sr, objectName, token, null);
            if (result.Code == (int)HttpCode.OK)
            {
                updateResponse.Etag = DateTime.Now.Ticks.ToString();
                updateResponse.Size = sr.Length;
                updateResponse.ObjectName = $"/{bucketName}/{objectName}";
            } 
            return updateResponse;
        }
          

        #endregion

        #region 文件下载

        /// <summary>
        /// 文件下载
        /// </summary> 
        /// <param name="folder">OSS文件路径</param>
        /// <param name="downloadFilename">本地存储文件路径</param>
        public void DownFile(string folder, string downloadFilename)
        {
            string domain = "http://if-pbl.qiniudn.com";
            string key = folder;
            string publicUrl = DownloadManager.CreatePublishUrl(domain, key);
            DownloadManager.Download(publicUrl, downloadFilename);
        }

        /// <summary>
        /// 文件下载
        /// </summary> 
        /// <param name="folder">OSS文件路径</param>
        /// <param name="downloadFilename">本地存储文件路径</param>
        public async Task DownFileAsync(string folder, string downloadFilename)
        {
            await Task.Factory.StartNew(() =>
            {
                DownFile(folder, downloadFilename);
            });
        }

        #endregion

        #region 删除文件

        /// <summary>
        /// 文件删除
        /// </summary>
        /// <param name="bucketName">存储空间</param>
        /// <param name="filename">文件名称</param>
        /// <returns></returns>
        public bool DeleteFile(string bucketName, string filename)
        {
            // 设置存储区域
            Config config = new Config();
            config.Zone = Zone;
            Mac mac = new Mac(ossAppID, ossAppSecret);
            BucketManager bucketManager = new BucketManager(mac, config);
            HttpResult result = bucketManager.Delete(bucketName, filename);
            return result.Code == 200;
        }

        #endregion

    }
}
