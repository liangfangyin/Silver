using System;
using System.Collections.Generic;
using System.Text;

namespace Silver.Quartz.Model
{
    public class TaskOptions
    {
        /// <summary>
        /// 任务ID
        /// </summary>
        public string TaskID { get; set; } = Guid.NewGuid().ToString().ToLower();

        /// <summary>
        /// 项目名称
        /// </summary>
        public string TaskName { get; set; } = "";

        /// <summary>
        /// 分组名称
        /// </summary>
        public string GroupName { get; set; } = "";

        /// <summary>
        /// Cron表达式
        /// </summary>
        public string BasicsCron { get; set; } = "";

        /// <summary>
        /// Api地址
        /// </summary>
        public string ApiUrl { get; set; } = "";

        /// <summary>
        /// 请求参数
        /// </summary>
        public string Parameter { get; set; } = "";

        /// <summary>
        /// Header 关键词
        /// </summary>
        public string AuthKey { get; set; } = "";

        /// <summary>
        /// Header 描述
        /// </summary>
        public string AuthValue { get; set; } = "";

        /// <summary>
        /// 描述
        /// </summary>
        public string Describe { get; set; } = "";

        /// <summary>
        /// 请求方式：GET、POST、DELETE、PUT
        /// </summary>
        public string RequestType { get; set; } = "POST";

        /// <summary>
        /// 请求内容方式
        /// </summary>
        public string ContentType { get; set; } = "application/json";

        /// <summary>
        /// 最后更新日期
        /// </summary>
        public DateTime LastRunTime { get; set; } = Convert.ToDateTime("2000-01-01");

        /// <summary>
        /// 状态 0：启用 
        /// </summary>
        public int Status { get; set; } = 0;

        /// <summary>
        /// 过期时间（毫秒）
        /// </summary>
        public int TimeOut { get; set; } = 5000;


    }
}
