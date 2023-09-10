namespace Silver.Nacos.Core.Model.Request
{
    public class NacosListInstanceRequest
    {

        /// <summary>
        /// 是 服务名
        /// </summary>
        public string serviceName { get; set; } = "";

        /// <summary>
        /// 否 分组名
        /// </summary>
        public string groupName { get; set; } = "DEFAULT_GROUP";

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

    }
}
