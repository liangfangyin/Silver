namespace Silver.Nacos.Core.Model.Request
{
    public class NacosConfigRequest
    {

        /// <summary>
        /// 租户信息，对应 Nacos 的命名空间ID字段--否
        /// </summary>
        public string tenant { get; set; } = "";

        /// <summary>
        /// 配置 ID--是
        /// </summary>
        public string dataId { get; set; } = "";

        /// <summary>
        /// 配置分组--是
        /// </summary>
        public string group { get; set; } = "";

    }
}
