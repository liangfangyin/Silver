namespace Silver.Nacos.Core.Model.Request
{
    public class NacosBeatInstanceRequest
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
        /// 否   是否临时实例
        /// </summary>
        public bool ephemeral { get; set; } = true;

        /// <summary>
        /// 是  JSON格式字符串  实例心跳内容
        /// </summary>
        public object beat { get; set; }

    }
}
