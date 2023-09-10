using Silver.NLog.Core.Model;
using SqlSugar;

namespace Silver.NLog.Core
{
    public class NLogConfig
    {

        /// <summary>
        /// 日志存储模式
        /// </summary>
        public static LogsMode Mode { get; set; } = LogsMode.Local;

        /// <summary>
        /// 日志数据库地址
        /// </summary>
        public static string ConnectionString { get; set; }

        /// <summary>
        /// 日志数据库类型
        /// </summary>
        public static DbType DbBaseType { get; set; } = DbType.MySql;

    }
}
