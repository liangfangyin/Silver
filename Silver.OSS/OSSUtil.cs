using Silver.OSS.Cloud;
using Silver.OSS.Model;
using System.IO;
using System.Threading.Tasks;

namespace Silver.OSS
{
    public class OSSUtil
    {
        private OSSSetting setting;
        private AliPayOSS aliPayOSS;
        private MinioOSS minioOSS;
        private QiniuOSS qiniuOSS;
        private TencentOSS tencentOSS;

        public OSSUtil()
        {
            setting = new OSSSetting();
            InitOSS();
        }

        public OSSUtil(OSSSetting setting)
        {
            this.setting = setting;
            InitOSS();
        }

        private void InitOSS()
        {
            switch (setting.Mode)
            {
                case OSSMode.AliPay:
                    aliPayOSS = new AliPayOSS(setting.OssEndpoint, setting.OssAppID, setting.OssAppSecret);
                    break;
                case OSSMode.Minio:
                    minioOSS = new MinioOSS(new MinioOption()
                    {
                        ApiUrl = setting.OssEndpoint,
                        AccessKey = setting.OssAppID,
                        SecretKey = setting.OssAppSecret,
                        Secure = setting.Secure
                    });
                    break;
                case OSSMode.Tencent: 
                    tencentOSS = new TencentOSS(setting.OssEndpoint, setting.OssAppID, setting.OssAppSecret);
                    break;
                case OSSMode.Qiniu:
                    qiniuOSS = new QiniuOSS(setting.OssEndpoint, setting.OssAppID, setting.OssAppSecret);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 创建桶
        /// </summary>
        /// <param name="bucketName">桶名称</param>
        /// <returns></returns>
        public async Task<bool> CreateBucket(string bucketName)
        {
            switch (setting.Mode)
            {
                case OSSMode.AliPay:
                    return aliPayOSS.CreateBucket(bucketName);
                case OSSMode.Minio:
                    return await minioOSS.CreateBucketAsync(bucketName);
                case OSSMode.Tencent:
                    return tencentOSS.CreateBucket(bucketName);
                case OSSMode.Qiniu://缺失
                    return true; //tencentOSS.CreateBucket(bucketName);
                default:
                    return false;
            }
        }

        /// <summary>
        /// 删除桶
        /// </summary>
        /// <param name="bucketName">桶名称</param>
        /// <returns></returns>
        public async Task<bool> RemoveBucket(string bucketName)
        {
            switch (setting.Mode)
            {
                case OSSMode.AliPay:
                    return aliPayOSS.DeleteBucket(bucketName);
                case OSSMode.Minio:
                    return await minioOSS.RemoveBucketAsync(bucketName);
                case OSSMode.Tencent:
                    return tencentOSS.DeleteBucket(bucketName);
                case OSSMode.Qiniu://缺失
                    return true;// tencentOSS.CreateBucket(bucketName);
                default:
                    return false;
            }
        }

        /// <summary>
        /// 上传文件-本地
        /// </summary>
        /// <param name="bucketName">桶名称</param>
        /// <param name="filePath">本地文件地址 C://11.jpg</param>
        /// <param name="objectDictory">前缀  peko或peko/images </param>
        /// <returns></returns>
        public async Task<MinioUpdateResponse> PutObject(string bucketName, string filePath, string objectDictory = "")
        {
            switch (setting.Mode)
            {
                case OSSMode.AliPay:
                    return aliPayOSS.PutFile(bucketName, filePath, objectDictory);
                case OSSMode.Minio:
                    return await minioOSS.PutObjectAsync(bucketName, filePath, objectDictory);
                case OSSMode.Tencent:
                    return await tencentOSS.PutFileAsync(bucketName, filePath, objectDictory);
                case OSSMode.Qiniu:
                    return qiniuOSS.PutFile(bucketName, filePath, objectDictory);
                default:
                    return null;
            }
        }

        /// <summary>
        /// 上传文件-流
        /// </summary>
        /// <param name="bucketName">桶名称</param>
        /// <param name="filePath">本地文件地址 C://11.jpg</param>
        /// <param name="objectDictory">前缀  peko或peko/images </param>
        /// <returns></returns>
        public async Task<MinioUpdateResponse> PutObjectStream(string bucketName, string objectDictory, Stream sr, string extension = ".jpg")
        {
            switch (setting.Mode)
            {
                case OSSMode.AliPay:
                    return aliPayOSS.PutFileStream(bucketName, objectDictory, sr, extension);
                case OSSMode.Minio:
                    return await minioOSS.PutObjectStreamAsync(bucketName, objectDictory, sr, extension);
                case OSSMode.Tencent:
                    return tencentOSS.PutFileStream(bucketName, objectDictory, sr, extension);
                case OSSMode.Qiniu:
                    return qiniuOSS.PutFileStream(bucketName, objectDictory, sr, extension);
                default:
                    return null;
            }
        }

    }
}
