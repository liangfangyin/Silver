using Minio;
using Minio.DataModel;
using Minio.DataModel.ObjectLock;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace Silver.OSS.Cloud
{

    public class MinioOSS
    {
        private List<string> listBucketName = new List<string>();
        private Dictionary<string, string> parameExites = new Dictionary<string, string>();
        private MinioClient minioClient;

        public MinioOSS()
        {
            MinioOption minioOption = new MinioOption();
            minioClient = new MinioClient()
                                        .WithEndpoint(minioOption.ApiUrl)
                                        .WithCredentials(minioOption.AccessKey, minioOption.SecretKey)
                                        .WithSSL(minioOption.Secure)
                                        .Build();
        }

        public MinioOSS(MinioOption minioOption)
        {
            minioClient = new MinioClient()
                                        .WithEndpoint(minioOption.ApiUrl)
                                        .WithCredentials(minioOption.AccessKey, minioOption.SecretKey)
                                        .WithSSL(minioOption.Secure)
                                        .Build();
        }

        #region Bucket

        /// <summary>
        /// 判断Bucket节点是否存在
        /// </summary>
        /// <param name="minioClient"></param>
        /// <param name="bucketName"></param>
        /// <returns></returns>
        public async Task<bool> BucketExistsAsync(string bucketName)
        {
            if (listBucketName.Where(t => t == bucketName).Count() > 0)
            {
                return true;
            }
            await ListBucketsAsync();
            if (listBucketName.Where(t => t == bucketName).Count() > 0)
            {
                return true;
            }
            return false;
        }


        /// <summary>
        /// Bucket节点列表
        /// </summary>
        /// <returns></returns>
        public async Task<ListAllMyBucketsResult> ListBucketsAsync()
        {
            var result = await minioClient.ListBucketsAsync().ConfigureAwait(false);
            foreach (var bucket in result.Buckets)
            {
                if (listBucketName.Where(t => t == bucket.Name).Count() <= 0)
                {
                    listBucketName.Add(bucket.Name);
                }
            }
            return result;
        }

        /// <summary>
        /// Bucket节点删除
        /// </summary>
        /// <param name="bucketName"></param>
        /// <returns></returns>
        public async Task<bool> RemoveBucketAsync(string bucketName)
        {
            if (!await BucketExistsAsync(bucketName))
            {
                return true;
            }
            var argsBucket = new RemoveBucketArgs().WithBucket(bucketName);
            await minioClient.RemoveBucketAsync(argsBucket).ConfigureAwait(false);
            return true;
        }

        /// <summary>
        /// Bucket节点创建
        /// </summary>
        /// <param name="bucketName"></param>
        /// <returns></returns>
        public async Task<bool> CreateBucketAsync(string bucketName)
        {
            if (await BucketExistsAsync(bucketName))
            {
                return true;
            }
            var argsBucket = new MakeBucketArgs().WithBucket(bucketName);
            await minioClient.MakeBucketAsync(argsBucket).ConfigureAwait(false);
            await SetPolicyAsync(bucketName);
            return false;
        }

        #endregion

        #region 文件对象

        /// <summary>
        /// 上传本地文件
        /// </summary>
        /// <param name="bucketName"></param>
        /// <param name="filePath">本地文件地址</param>
        /// <param name="objectDictory">项目前缀</param>
        /// <returns></returns>
        public async Task<MinioUpdateResponse> PutObjectAsync(string bucketName, string filePath, string objectDictory = "")
        {
            await CreateBucketAsync(bucketName);
            Random random = new Random();
            string objectName = DateTime.Now.ToString("yyyyMMdd") + "/" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + random.Next(10000000, 99999999) + Path.GetExtension(filePath);
            if (!string.IsNullOrEmpty(objectDictory))
            {
                if (!objectDictory.EndsWith("/"))
                {
                    objectDictory += "/";
                }
                objectName = objectDictory + objectName;
            }
            var args = new PutObjectArgs()
                     .WithBucket(bucketName)
                     .WithObject(objectName)
                     .WithContentType("application/octet-stream")
                     .WithFileName(filePath);
            var result = await minioClient.PutObjectAsync(args).ConfigureAwait(false);
            return new MinioUpdateResponse()
            {
                Etag = result.Etag,
                ObjectName = $"/{bucketName}/{objectName}",
                Size = result.Size,
            };
        }

        /// <summary>
        /// 上传文件流
        /// </summary>
        /// <param name="bucketName"></param>
        /// <param name="fileStream"></param>
        /// <param name="objectDictory">项目前缀</param>
        /// <returns></returns>
        public async Task<MinioUpdateResponse> PutObjectStreamAsync(string bucketName,  string objectDictory, Stream fileStream, string extension=".jpg")
        {
            await CreateBucketAsync(bucketName);
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
            var args = new PutObjectArgs()
                     .WithBucket(bucketName)
                     .WithObject(objectName)
                     .WithContentType("application/octet-stream")
                     .WithStreamData(fileStream);
            var result = await minioClient.PutObjectAsync(args).ConfigureAwait(false);
            return new MinioUpdateResponse()
            {
                Etag = result.Etag,
                ObjectName = $"/{bucketName}/{objectName}",
                Size = result.Size,
            };
        }

        /// <summary>
        /// 文件下载本地
        /// </summary>
        /// <param name="bucketName"></param>
        /// <param name="objectName">文件线上路径，不要包括Bucket </param>
        /// <param name="fileName">本地保存路径 C:\\300_25.jpg</param>
        /// <param name="sse"></param>
        /// <returns></returns>
        public async Task DownObjectAsync(string bucketName, string objectName = "", string fileName = "", IServerSideEncryption sse = null)
        {
            var args = new GetObjectArgs().WithBucket(bucketName).WithObject(objectName).WithFile(fileName).WithServerSideEncryption(sse);
            await minioClient.GetObjectAsync(args).ConfigureAwait(false);
        }

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="bucketName"></param>
        /// <param name="objectName">文件线上路径，不要包括Bucket </param>
        /// <returns></returns>
        public async Task RemoveObjectAsync(string bucketName, string objectName = "")
        {
            var args = new RemoveObjectArgs().WithBucket(bucketName).WithObject(objectName);
            await minioClient.RemoveObjectAsync(args).ConfigureAwait(false);
        }

        /// <summary>
        /// 文件列表
        /// </summary>
        /// <param name="bucketName"></param>
        /// <param name="prefix">扩展名</param>
        /// <param name="recursive">显示子集   true：显示  false：不显示</param>
        /// <returns></returns>
        public async Task<List<Item>> ListObjectAsync(string bucketName, string prefix = "", bool recursive = false)
        {
            var args = new ListObjectsArgs().WithBucket(bucketName);
            if (!string.IsNullOrEmpty(prefix))
            {
                args.WithPrefix(prefix);
            }
            args.WithRecursive(recursive);
            return (List<Item>)await minioClient.ListObjectsAsync(args).ToList();
        }

        /// <summary>
        /// 设置文件保留天数
        /// </summary>
        /// <param name="bucketName"></param>
        /// <param name="objectName"></param>
        /// <param name="days"></param>
        /// <returns></returns>
        public async Task SetObjectRetentionAsync(string bucketName, string objectName, int days = 30)
        {
            var args = new SetObjectRetentionArgs()
                .WithBucket(bucketName)
                .WithObject(objectName)
                .WithRetentionMode(RetentionMode.COMPLIANCE)  // 设置为合规性保留模式
                .WithRetentionUntilDate(DateTime.Now.AddDays(days));  // 设置保留天数
            await minioClient.SetObjectRetentionAsync(args).ConfigureAwait(false);
        }

        /// <summary>
        /// 获取文件url 7天内
        /// </summary>
        /// <param name="bucketName"></param>
        /// <param name="objectName"></param>
        /// <param name="expiryInSeconds"></param>
        /// <returns></returns>
        public async Task<string> GetObjectUrlAsync(string bucketName, string objectName, int expiryInSeconds = 604800)
        {
            var args = new PresignedGetObjectArgs()
                .WithBucket(bucketName)
                .WithObject(objectName)
                .WithExpiry(expiryInSeconds);
            return await minioClient.PresignedGetObjectAsync(args).ConfigureAwait(false);
        }

        /// <summary>
        /// 设置目录外网访问
        /// </summary>
        /// <param name="bucketName"></param>
        /// <returns></returns>
        public async Task SetPolicyAsync(string bucketName)
        {
            var policyJson = $@"{{""Version"":""2012-10-17"",""Statement"":[{{""Action"":[""s3:GetBucketLocation""],""Effect"":""Allow"",""Principal"":{{""AWS"":[""*""]}},""Resource"":[""arn:aws:s3:::{bucketName}""],""Sid"":""""}},{{""Action"":[""s3:ListBucket""],""Condition"":{{""StringEquals"":{{""s3:prefix"":[""foo"",""prefix/""]}}}},""Effect"":""Allow"",""Principal"":{{""AWS"":[""*""]}},""Resource"":[""arn:aws:s3:::{bucketName}""],""Sid"":""""}},{{""Action"":[""s3:GetObject""],""Effect"":""Allow"",""Principal"":{{""AWS"":[""*""]}},""Resource"":[""arn:aws:s3:::{bucketName}/*"",""arn:aws:s3:::{bucketName}/*""],""Sid"":""""}}]}}";
            var argsSetPolicyArgs = new SetPolicyArgs().WithBucket(bucketName).WithPolicy(policyJson);
            await minioClient.SetPolicyAsync(argsSetPolicyArgs);
        }

        #endregion

    }

    public class MinioOption
    {

        /// <summary>
        /// Minio地址
        /// </summary>
        public string ApiUrl { get; set; } = "localhost:9000";

        /// <summary>
        /// key
        /// </summary>
        public string AccessKey { get; set; } = "UKQqf1fiuHhyX8PMnhMk";

        /// <summary>
        /// 秘钥
        /// </summary>
        public string SecretKey { get; set; } = "o6vRcLTJEwcAS8vsTPIzcR0scJMPvN35WZEkYMC4";

        /// <summary>
        /// 是否SSL
        /// </summary>
        public bool Secure { get; set; }


    }

    public class MinioUpdateResponse
    {
        public string ObjectName;

        public long Size;

        public string Etag;

    }

}
