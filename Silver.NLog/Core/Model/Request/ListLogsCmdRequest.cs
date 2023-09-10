using System;
using System.Collections.Generic;
using System.Text;

namespace Silver.NLog.Core.Model.Request
{
    public class ListLogsCmdRequest
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
        /// 接口
        /// </summary>
        public string? cmd { get; set; } = "";

        /// <summary>
        /// 参数
        /// </summary>
        public string? data { get; set; } = "";

        /// <summary>
        /// 返回
        /// </summary>
        public string? result { get; set; } = "";

        /// <summary>
        /// code
        /// </summary>
        public string? code { get; set; } = "";

        /// <summary>
        /// 服务名称
        ///</summary>
        public string serviceId { get; set; } = "";

        /// <summary>
        /// 平台类型     operation：运维平台  card：一卡通   merchant：商户平台
        ///</summary> 
        public string platform { get; set; } = "";

        /// <summary>
        /// 最小用时
        /// </summary>
        public string? min { get; set; }

        /// <summary>
        /// 最大用时
        /// </summary>
        public string? max { get; set; }
    }
}
