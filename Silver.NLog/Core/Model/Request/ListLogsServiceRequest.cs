using System;
using System.Collections.Generic;
using System.Text;

namespace Silver.NLog.Core.Model.Request
{
    public class ListLogsServiceRequest
    {

        /// <summary>
        /// 每页行数
        /// </summary>  
        public int rows { get; set; } = 15;
        /// <summary>
        /// 当前页
        /// </summary>
        public int page { get; set; } = 1;

        /// <summary>
        /// 开始时间
        /// </summary>
        public string? beginDate { get; set; } = "";

        /// <summary>
        /// 结束时间
        /// </summary>
        public string? endDate { get; set; } = "";

        /// <summary>
        /// 方法名称
        /// </summary>
        public string? methods { get; set; } = "";

        /// <summary>
        /// 返回内容
        /// </summary>
        public string? result { get; set; } = "";

        /// <summary>
        /// 服务名称
        ///</summary>
        public string serviceId { get; set; } = "";

        /// <summary>
        /// 平台类型     operation：运维平台  card：一卡通   merchant：商户平台
        ///</summary> 
        public string platform { get; set; } = "";

    }
}
