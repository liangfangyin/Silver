namespace Silver.Nacos.Core.Model.Request
{
    public class NacosInfoInstanceRequest
    {
        /// <summary>
        /// 是 服务名
        /// </summary>
        public string serviceName { get; set; } = "";

        /// <summary>
        /// 否 分组名
        /// </summary>
        public string groupName { get; set; } = "";

        /// <summary>
        /// 是 服务实例IP
        /// </summary>
        public string ip { get; set; } = "localhost";

        /// <summary>
        ///  是   服务实例port
        /// </summary>
        public int port { get; set; }

        /// <summary>
        /// 否   命名空间ID
        /// </summary>
        public string namespaceId { get; set; } = "";

        /// <summary>
        /// 否   多个集群用逗号分隔    集群名称
        /// </summary>
        public string clusters { get; set; } = "";

        /// <summary>
        /// 否，默认为false 是否只返回健康实例
        /// </summary>
        public bool healthyOnly { get; set; } = false;

        /// <summary>
        /// 否   是否临时实例
        /// </summary>
        public bool ephemeral { get; set; } = true;

    }
}
