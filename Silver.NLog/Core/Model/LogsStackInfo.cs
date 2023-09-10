using System;
using System.Collections.Generic;
using System.Text;

namespace Silver.NLog.Core.Model
{
    public class LogsStackInfo
    {
        /// <summary>
        /// 日志时间
        /// </summary>
        public string time { get; set; } = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

        /// <summary>
        /// 日志标题/方法
        /// </summary>
        public string method { get; set; } = "";

        /// <summary>
        /// 日志内容
        /// </summary>
        public string message { get; set; } = "";

        /// <summary>
        /// 日志等级
        /// </summary>
        public LogsLevel level { get; set; } = LogsLevel.INFO;
         

    }
}
