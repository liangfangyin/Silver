namespace Silver.Nacos.Core.Model.Request
{
    public class NacosConfigListenerRequest
    {

        /// <summary>
        /// 配置 ID -是
        /// </summary>
        public string dataId { get; set; } = "";

        /// <summary>
        /// 配置分组
        /// </summary>
        public string group { get; set; } = "";

        /// <summary>
        /// 配置内容 MD5 值
        /// </summary>
        public string contentMD5 { get; set; } = "";

        /// <summary>
        /// 租户信息，对应 Nacos 的命名空间字段(非必填)
        /// </summary>
        public string tenant { get; set; } = "";

    }
}
