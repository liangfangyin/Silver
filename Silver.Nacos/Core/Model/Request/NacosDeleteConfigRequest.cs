namespace Silver.Nacos.Core.Model.Request
{
    public class NacosDeleteConfigRequest
    {

        /// <summary>
        /// 否   租户信息，对应 Naocs 的命名空间ID字段
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

    }
}
