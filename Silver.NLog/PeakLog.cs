using NLog;
using NLog.Web;
using Silver.Basic;
using Silver.NLog.Core;
using Silver.NLog.Core.Model;
using Silver.NLog.Core.Model.Request;
using Silver.NLog.Core.ORM;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Silver.NLog
{
    /// <summary>
    /// 日志帮助类
    /// Local：本地日志
    /// Cloud：数据库日志
    /// Console：记录打印输出
    /// </summary>
    public class PeakLog
    {
        private static Queue<LogsStackInfo> logsINFO = new Queue<LogsStackInfo>();
        private static Queue<LogsService> logsService = new Queue<LogsService>();
        private static Queue<LogsCmd> logsCmd = new Queue<LogsCmd>();
        private static Logger logger = NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();
        private static object lockINFO = new object();
        private static object lockService = new object();
        private static object lockCmd = new object();
        private static bool isInitLog = false;

        /// <summary>
        /// 日志初始化
        /// </summary>
        /// <param name="mode"></param>
        /// <param name="connectionString"></param>
        /// <param name="DbBaseType"></param>
        public static void InitLog(LogsMode mode,string connectionString, DbType DbBaseType)
        {
            NLogConfig.ConnectionString = connectionString;
            NLogConfig.Mode = mode;
            NLogConfig.DbBaseType = DbBaseType;
        }

        /// <summary>
        /// 调试日志
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="name"></param>
        public static void DEBUG(string msg, string name = "")
        {
            if (string.IsNullOrEmpty(name))
            {
                name = GetMethodName(new StackTrace());
            }
            AddQueue(new LogsStackInfo() { level = LogsLevel.DEBUG, message = msg, method = name });
        }

        /// <summary>
        /// 基础信息
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="name"></param>
        public static void INFO(string msg, string name = "")
        {
            if (string.IsNullOrEmpty(name))
            {
                name = GetMethodName(new StackTrace());
            }
            AddQueue(new LogsStackInfo() { level = LogsLevel.INFO, message = msg, method = name });
        }

        /// <summary>
        /// 警告信息
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="name"></param>
        public static void WARN(string msg, string name = "")
        {
            if (string.IsNullOrEmpty(name))
            {
                name = GetMethodName(new StackTrace());
            }
            AddQueue(new LogsStackInfo() { level = LogsLevel.WARN, message = msg, method = name });
        }

        /// <summary>
        /// 警告信息
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="name"></param>
        public static void ERROR(string msg, string name = "")
        {
            if (string.IsNullOrEmpty(name))
            {
                name = GetMethodName(new StackTrace());
            }
            AddQueue(new LogsStackInfo() { level = LogsLevel.ERROR, message = msg, method = name });
        }

        /// <summary>
        /// 严重错误日志
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="name"></param>
        public static void FATAL(string msg, string name = "")
        {
            if (string.IsNullOrEmpty(name))
            {
                name = GetMethodName(new StackTrace());
            }
            AddQueue(new LogsStackInfo() { level = LogsLevel.FATAL, message = msg, method = name });
        }

        /// <summary>
        /// 服务错误日志
        /// </summary>
        /// <param name="text">文本</param>
        /// <param name="method">方法</param>
        /// <param name="timeOut">用时</param>
        /// <param name="serviceId">服务名称</param>
        /// <param name="platform">平台名称</param>
        public static void LogsService(string text, string method = "", int timeOut = 0, string serviceId = "", string platform = "")
        {
            LogsService infoService = new LogsService();
            infoService.Id = DateTime.Now.Ticks;
            infoService.Methods = method;
            infoService.Timer = timeOut;
            infoService.Result = text;
            infoService.Code = "error";
            infoService.ServiceId = serviceId;
            infoService.Platform = platform;
            logsService.Enqueue(infoService);
            InitLog();
        }

        /// <summary>
        /// 接口日志
        /// </summary>
        /// <param name="text">文本</param>
        /// <param name="method">方法</param>
        /// <param name="timeOut">用时</param>
        /// <param name="serviceId">服务名称</param>
        /// <param name="platform">平台名称</param>
        public static void InfoCmd(LogsCmd info)
        {
            logsCmd.Enqueue(info);
            InitLog();
        }

        /// <summary>
        /// 获取方法
        /// </summary>
        /// <param name="trace"></param>
        /// <returns></returns>
        private static string GetMethodName(StackTrace trace)
        {
            string invokerType = trace.GetFrame(1).GetMethod().DeclaringType.FullName;
            string invokerMethod = trace.GetFrame(1).GetMethod().Name;
            return $"{invokerType}.{invokerMethod}";
        }

        /// <summary>
        /// 日志入队列
        /// </summary>
        /// <param name="info"></param>
        private static void AddQueue(LogsStackInfo info)
        {
            int resetTicker = 0;
            ResetQueue:
            try
            {
                lock (lockINFO)
                {
                    logsINFO.Enqueue(info);
                }
                InitLog();
            }
            catch (Exception ex)
            {
                resetTicker++;
                if (resetTicker > 3)
                {
                    return;
                }
                Thread.Sleep(5);
                goto ResetQueue;
            }
        }

        private static void InitLog()
        {
            if (isInitLog)
            {
                return;
            }
            isInitLog = true;
            Task.Factory.StartNew(async () =>
            {
                while (true)
                {
                    try
                    {
                        List<LogsStackInfo> listStack = new List<LogsStackInfo>();
                        lock (PeakLog.lockINFO)
                        {
                            if (PeakLog.logsINFO.Count > 0)
                            {
                                for (var i = 0; i < PeakLog.logsINFO.Count; i++)
                                {
                                    LogsStackInfo infoLog = PeakLog.logsINFO.Dequeue();
                                    listStack.Add(infoLog);
                                }
                            }
                        }
                        //打印模式
                        if (NLogConfig.Mode == LogsMode.Console)
                        {
                            #region 打印模式
                            if (listStack.Count > 0)
                            {
                                foreach (var item in listStack)
                                {
                                    Console.WriteLine($"时间：{item.time} - 方法:{item.method} - 级别：{GetEnumNameByKey((int)item.level)} - 信息:{item.message} ");
                                }
                            }
                            #endregion
                        }
                        //本地模式
                        else if (NLogConfig.Mode == LogsMode.Local)
                        {
                            #region 本地模式
                            if (listStack.Count > 0)
                            {
                                foreach (var item in listStack)
                                {
                                    if (item.level == LogsLevel.DEBUG)
                                    {
                                        logger.Debug($"方法:{item.method} \r\n 信息:{item.message} ");
                                    }
                                    if (item.level == LogsLevel.INFO)
                                    {
                                        logger.Info($"方法:{item.method} \r\n 信息:{item.message} ");
                                    }
                                    if (item.level == LogsLevel.WARN)
                                    {
                                        logger.Warn($"方法:{item.method} \r\n 信息:{item.message} ");
                                    }
                                    if (item.level == LogsLevel.ERROR)
                                    {
                                        logger.Error($"方法:{item.method} \r\n 信息:{item.message} ");
                                    }
                                    if (item.level == LogsLevel.FATAL)
                                    {
                                        logger.Fatal($"方法:{item.method} \r\n 信息:{item.message} ");
                                    }
                                }
                            }
                            #endregion
                        }
                        else
                        {
                            #region 云端模式 
                            if (listStack.Count > 0)
                            {
                                List<LogsApplication> listAppLog = new List<LogsApplication>();
                                foreach (var item in listStack)
                                {
                                    LogsApplication infoApp = new LogsApplication();
                                    infoApp.Id = DateTime.Now.Ticks;
                                    infoApp.Methods = item.method;
                                    infoApp.Timer = 0;
                                    infoApp.Result = item.message;
                                    infoApp.ServiceId = "";
                                    infoApp.Platform = "";
                                    listAppLog.Add(infoApp);
                                }
                                if (listAppLog.Count > 0)
                                {
                                    using (var db = new SqlSugarExtension().GetCustomInstance())
                                    {
                                        db.Insertable(listAppLog).SplitTable().ExecuteCommand();
                                    }
                                }
                            }
                            #endregion
                        }
                    }
                    catch (Exception ex){ }
                    try
                    {
                        List<LogsService> listStack = new List<LogsService>();
                        lock (lockService)
                        {
                            if (logsService.Count > 0)
                            {
                                for (int i = 0; i < logsService.Count; i++)
                                {
                                    LogsService infoLog = logsService.Dequeue();
                                    listStack.Add(infoLog);
                                }
                            }
                        }
                        //打印模式
                        if (NLogConfig.Mode == LogsMode.Console)
                        {
                            #region 打印模式
                            if (listStack.Count > 0)
                            {
                                foreach (var item in listStack)
                                {
                                    Console.WriteLine($"时间：{item.CreateDate.ToString("yyyy-MM-dd HH:mm:ss")} - 方法:{item.Methods} - 信息:{item.Result} ");
                                }
                            }
                            #endregion
                        }
                        //本地模式
                        else if (NLogConfig.Mode == LogsMode.Local)
                        {
                            #region 本地模式
                            if (listStack.Count > 0)
                            {
                                foreach (var item in listStack)
                                {
                                    logger.Info($"时间：{item.CreateDate.ToString("yyyy-MM-dd HH:mm:ss")} - 方法:{item.Methods} - 信息:{item.Result} ");
                                }
                            }
                            #endregion
                        }
                        else
                        {
                            #region 云端模式 
                            if (listStack.Count > 0)
                            {
                                using (var db = new SqlSugarExtension().GetCustomInstance())
                                {
                                    db.Insertable(listStack).SplitTable().ExecuteCommand();
                                }
                            }
                            #endregion
                        }
                    }
                    catch { }
                    try
                    {
                        List<LogsCmd> listStack = new List<LogsCmd>();
                        lock (lockCmd)
                        {
                            if (logsCmd.Count > 0)
                            {
                                for (int i = 0; i < logsCmd.Count; i++)
                                {
                                    LogsCmd infoLog = logsCmd.Dequeue();
                                    listStack.Add(infoLog);
                                }
                            }
                        }
                        //打印模式
                        if (NLogConfig.Mode == LogsMode.Console)
                        {
                            #region 打印模式
                            if (listStack.Count > 0)
                            {
                                foreach (var item in listStack)
                                {
                                    Console.WriteLine($"时间：{item.CreateDate.ToString("yyyy-MM-dd HH:mm:ss")} - 方法:{item.Cmd} - 用时：{item.Timer} - 参数：{item.Data} - 信息:{item.Result} ");
                                }
                            }
                            #endregion
                        }
                        //本地模式
                        else if (NLogConfig.Mode == LogsMode.Local)
                        {
                            #region 本地模式
                            if (listStack.Count > 0)
                            {
                                foreach (var item in listStack)
                                {
                                    logger.Info($"时间：{item.CreateDate.ToString("yyyy-MM-dd HH:mm:ss")} - 方法:{item.Cmd} - 用时：{item.Timer} - 参数：{item.Data} - 信息:{item.Result} ");
                                }
                            }
                            #endregion
                        }
                        else
                        {
                            #region 云端模式 
                            if (listStack.Count > 0)
                            {
                                using (var db = new SqlSugarExtension().GetCustomInstance())
                                {
                                    db.Insertable(listStack).SplitTable().ExecuteCommand();
                                }
                            }
                            #endregion
                        }
                    }
                    catch { }
                    await Task.Delay(1500);
                } 
            });
        }

        private static string GetEnumNameByKey(int key)
        { 
            return LogsLevel.GetName(typeof(LogsLevel), key); 
        }

        /// <summary>
        /// 服务日志
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public static (List<LogsService>, int) ListLogsService(ListLogsServiceRequest info)
        {
            int total = 0;
            DateTime beginDate = DateTime.Now.AddDays(-1);
            DateTime endDate = DateTime.Now;
            using (var db = new SqlSugarExtension().GetCustomInstance())
            {
                var extLog = LinqUtil.True<LogsService>();
                if (!string.IsNullOrEmpty(info.beginDate) && !string.IsNullOrEmpty(info.endDate))
                {
                    beginDate = info.beginDate.ToDateTime();
                    endDate = info.endDate.ToDateTime();
                    extLog = extLog.And(t => SqlFunc.Between(t.CreateTicker, beginDate.Ticks, endDate.Ticks));
                }
                if (!string.IsNullOrEmpty(info.methods))
                {
                    extLog = extLog.And(t => t.Methods.Contains(info.methods));
                }
                if (!string.IsNullOrEmpty(info.result))
                {
                    extLog = extLog.And(t => t.Result.Contains(info.result));
                }
                if (!string.IsNullOrEmpty(info.serviceId))
                {
                    extLog = extLog.And(t => t.ServiceId.Contains(info.serviceId));
                }
                if (!string.IsNullOrEmpty(info.platform))
                {
                    extLog = extLog.And(t => t.Platform.Contains(info.platform));
                }
                return (db.Queryable<LogsService>().Where(extLog).SplitTable(beginDate, endDate).ToPageList(info.page, info.rows, ref total), total);
            }
        }

        /// <summary>
        /// 应用日志
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public static (List<LogsApplication>, int) ListLogsApplication(ListLogsServiceRequest info)
        {
            int total = 0;
            DateTime beginDate = DateTime.Now.AddDays(-1);
            DateTime endDate = DateTime.Now;
            using (var db = new SqlSugarExtension().GetCustomInstance())
            {
                var extLog = LinqUtil.True<LogsApplication>();
                if (!string.IsNullOrEmpty(info.beginDate) && !string.IsNullOrEmpty(info.endDate))
                {
                    beginDate = info.beginDate.ToDateTime();
                    endDate = info.endDate.ToDateTime();
                    extLog = extLog.And(t => SqlFunc.Between(t.CreateTicker, beginDate.Ticks, endDate.Ticks));
                }
                if (!string.IsNullOrEmpty(info.methods))
                {
                    extLog = extLog.And(t => t.Methods.Contains(info.methods));
                }
                if (!string.IsNullOrEmpty(info.result))
                {
                    extLog = extLog.And(t => t.Result.Contains(info.result));
                }
                if (!string.IsNullOrEmpty(info.serviceId))
                {
                    extLog = extLog.And(t => t.ServiceId.Contains(info.serviceId));
                }
                if (!string.IsNullOrEmpty(info.platform))
                {
                    extLog = extLog.And(t => t.Platform.Contains(info.platform));
                }
                return (db.Queryable<LogsApplication>().Where(extLog).SplitTable(beginDate, endDate).ToPageList(info.page, info.rows, ref total), total);
            }
        }

        /// <summary>
        /// 接口日志
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public static (List<LogsCmd>, int) ListLogsCmd(ListLogsCmdRequest info)
        {
            int total = 0;
            DateTime beginDate = DateTime.Now.AddDays(-1);
            DateTime endDate = DateTime.Now;
            using (var db = new SqlSugarExtension().GetCustomInstance())
            {
                var extLog = LinqUtil.True<LogsCmd>();
                if (!string.IsNullOrEmpty(info.beginDate) && !string.IsNullOrEmpty(info.endDate))
                {
                    beginDate = info.beginDate.ToDateTime();
                    endDate = info.endDate.ToDateTime();
                    extLog = extLog.And(t => SqlFunc.Between(t.CreateTicker, beginDate.Ticks, endDate.Ticks));
                }
                if (!string.IsNullOrEmpty(info.code))
                {
                    int.TryParse(info.code, out int code);
                    extLog = extLog.And(t => t.Code == code);
                }
                if (!string.IsNullOrEmpty(info.min))
                {
                    int.TryParse(info.min, out int min);
                    extLog = extLog.And(t => t.Timer >= min);
                }
                if (!string.IsNullOrEmpty(info.max))
                {
                    int.TryParse(info.max, out int max);
                    extLog = extLog.And(t => t.Timer <= max);
                }
                if (!string.IsNullOrEmpty(info.cmd))
                {
                    extLog = extLog.And(t => t.Cmd.Contains(info.cmd));
                }
                if (!string.IsNullOrEmpty(info.data))
                {
                    extLog = extLog.And(t => t.Data.Contains(info.data));
                }
                if (!string.IsNullOrEmpty(info.result))
                {
                    extLog = extLog.And(t => t.Result.Contains(info.result));
                }
                if (!string.IsNullOrEmpty(info.serviceId))
                {
                    extLog = extLog.And(t => t.ServiceId.Contains(info.serviceId));
                }
                if (!string.IsNullOrEmpty(info.platform))
                {
                    extLog = extLog.And(t => t.Platform.Contains(info.platform));
                }
                return (db.Queryable<LogsCmd>().Where(extLog).SplitTable(beginDate, endDate).ToPageList(info.page, info.rows, ref total), total);
            }
        }



    }
}
