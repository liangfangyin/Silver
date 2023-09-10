namespace Silver.Nacos.Core.Model.Request
{
    public class NacosUpdateInstanceRequest
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
        /// 否   集群名称
        /// </summary>
        public string clusterName { get; set; } = "";

        /// <summary>
        /// 否   命名空间ID
        /// </summary>
        public string namespaceId { get; set; } = "";

        /// <summary>
        /// 否   权重
        /// </summary>
        public double weight { get; set; } = 100;

        /// <summary>
        /// 否   扩展信息
        /// </summary>
        public string metadata { get; set; } = "";

        /// <summary>
        /// 否   是否打开流量
        /// </summary>
        public bool enabled { get; set; } = true;

        /// <summary>
        /// 否   是否临时实例
        /// </summary>
        public bool ephemeral { get; set; } = true;

        /// <summary>
        /// 是   用户名
        /// </summary>
        public string userName { get; set; } = "";

        /// <summary>
        /// 是   密码
        /// </summary>
        public string passWord { get; set; } = "";

    }
}
