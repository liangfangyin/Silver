namespace Silver.Nacos.Core.Model.Request
{
    public class NacosAddConfigRequest
    {

        /// <summary>
        /// 否   租户信息，对应 Nacos 的命名空间ID字段
        /// </summary>
        public string tenant { get; set; } = "";

        /// <summary>
        /// 是   配置 ID
        /// </summary>
        public string dataId { get; set; } = "";

        /// <summary>
        /// 是   配置分组
        /// </summary>
        public string group { get; set; } = "";

        /// <summary>
        /// 是   配置内容
        /// </summary>
        public string content { get; set; } = "";

        /// <summary>
        /// 否   配置类型
        /// </summary>
        public string type { get; set; } = "";

    }
}
