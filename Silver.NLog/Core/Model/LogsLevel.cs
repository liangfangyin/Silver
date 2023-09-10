using System;
using System.Collections.Generic;
using System.Text;

namespace Silver.NLog.Core.Model
{
    /// <summary>
    /// 日志级别
    /// </summary>
    public enum LogsLevel
    {
        /// <summary>
        /// 所有日志
        /// </summary>
        ALL = 0,
        /// <summary>
        /// 调试模式
        /// </summary>
        DEBUG = 1,
        /// <summary>
        /// 基础信息
        /// </summary>
        INFO = 2,
        /// <summary>
        /// 警告信息
        /// </summary>
        WARN = 3,
        /// <summary>
        /// 错误
        /// </summary>
        ERROR = 4,
        /// <summary>
        /// 严重错误日志，程序退出
        /// </summary>
        FATAL = 5,
        /// <summary>
        /// 关闭所有日志
        /// </summary>
        OFF = 6

    }
}
