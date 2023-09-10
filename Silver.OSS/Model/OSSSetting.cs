using Silver.Basic;
using System;

namespace Silver.OSS.Model
{
    public class OSSSetting
    {
        /// <summary>
        /// 网关地址
        /// </summary>
        public string OssEndpoint { get; set; } = ConfigurationUtil.GetSection("OSS:Endpoint"); 

        /// <summary>
        /// AppID
        /// </summary>
        public string OssAppID { get; set; } = ConfigurationUtil.GetSection("OSS:AppID");

        /// <summary>
        /// 秘钥
        /// </summary>
        public string OssAppSecret { get; set; }= ConfigurationUtil.GetSection("OSS:AppSecret");

        /// <summary>
        /// OSS类型
        /// </summary>
        public OSSMode Mode { get; set; } = (OSSMode)Convert.ToInt32(ConfigurationUtil.GetSection("OSS:Mode"));

        /// <summary>
        /// 是否SSL
        /// </summary>
        public bool Secure { get; set; } = Convert.ToBoolean(ConfigurationUtil.GetSection("OSS:Secure"));


    }
}
