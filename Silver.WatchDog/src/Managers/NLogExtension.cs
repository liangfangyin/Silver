using Silver.WatchDog.src.Helpers;
using Silver.WatchDog.src.Models;

namespace Silver.WatchDog.src.Managers
{
    public class NLogExtension
    {
        public static bool isWatchLog = false;
        public static bool isWatchExceptionLog = false;
        public static bool isWatchLoggerModel = false;
        public static object lockWatchLog = new object();
        public static object lockWatchExceptionLog = new object();
        public static object lockWatchLoggerModel = new object();
        public static Queue<WatchLog> logsWatchLogQueue = new Queue<WatchLog>();
        public static Queue<WatchExceptionLog> logsWatchExceptionLogQueue = new Queue<WatchExceptionLog>();
        public static Queue<WatchLoggerModel> logsWatchLoggerModelQueue = new Queue<WatchLoggerModel>();

        /// <summary>
        /// 普通日志
        /// </summary>
        /// <param name="watchLog"></param>
        public static void InsertWatchLog(WatchLog watchLog)
        {
            int ticker = 0;
            Reset:
            try
            {
            
                lock (lockWatchLog)
                {
                    logsWatchLogQueue.Enqueue(watchLog);
                }
            }
            catch (Exception ex)
            {
                ticker++;
                Thread.Sleep(2);
                if (ticker <= 3)
                {
                    goto Reset;
                }
            }
            InitWatchLog();
        }

        /// <summary>
        /// 异常日志
        /// </summary>
        /// <param name="watchLog"></param>
        public static void InsertWatchExceptionLog(WatchExceptionLog watchExceptionLog)
        {
            int ticker = 0;
            Reset:
            try
            {

                lock (lockWatchLog)
                {
                    logsWatchExceptionLogQueue.Enqueue(watchExceptionLog);
                }
            }
            catch (Exception ex)
            {
                ticker++;
                Thread.Sleep(2);
                if (ticker <= 3)
                {
                    goto Reset;
                }
            }
            InitWatchExceptionLog();
        }

        /// <summary>
        /// 模块日志
        /// </summary>
        /// <param name="watchLoggerModel"></param>
        public static void InsertWatchLogger(WatchLoggerModel watchLoggerModel)
        {
            int ticker = 0;
            Reset:
            try
            {

                lock (lockWatchLog)
                {
                    logsWatchLoggerModelQueue.Enqueue(watchLoggerModel);
                }
            }
            catch (Exception ex)
            {
                ticker++;
                Thread.Sleep(2);
                if (ticker <= 3)
                {
                    goto Reset;
                }
            }
            InitWatchLogger();
        }


        private static void InitWatchLog()
        {
            if (isWatchLog)
            {
                return;
            }
            isWatchLog = true;
            Task.Factory.StartNew(() =>
            {
                int timeOut = 1000;
                while (true)
                {
                    try
                    {
                        int count = logsWatchLogQueue.Count;
                        List<WatchLog> listLogs = new List<WatchLog>();
                        for (int i = 0; i < count; i++)
                        {
                            WatchLog content = new WatchLog();
                            lock (lockWatchLog)
                            {
                                content = logsWatchLogQueue.Dequeue();
                            }
                            listLogs.Add(content);
                        }
                        SQLDbHelper.BatchInsertWatchLog(listLogs);
                    }
                    catch (Exception ex) { }
                    Task.Delay(timeOut).Wait();
                }
            });
        }

        private static void InitWatchExceptionLog()
        {
            if (isWatchExceptionLog)
            {
                return;
            }
            isWatchExceptionLog = true;
            Task.Factory.StartNew(() =>
            {
                int timeOut = 1000;
                while (true)
                {
                    try
                    {
                        int count = logsWatchExceptionLogQueue.Count;
                        List<WatchExceptionLog> listLogs = new List<WatchExceptionLog>();
                        for (int i = 0; i < count; i++)
                        {
                            WatchExceptionLog content = new WatchExceptionLog();
                            lock (lockWatchLog)
                            {
                                content = logsWatchExceptionLogQueue.Dequeue();
                            }
                            listLogs.Add(content);
                        }
                        SQLDbHelper.BatchInsertWatchExceptionLog(listLogs);
                    }
                    catch (Exception ex) { }
                    Task.Delay(timeOut).Wait();
                }
            });
        }

        private static void InitWatchLogger()
        {
            if (isWatchLoggerModel)
            {
                return;
            }
            isWatchLoggerModel = true;
            Task.Factory.StartNew(() =>
            {
                int timeOut = 1000;
                while (true)
                {
                    try
                    {
                        int count = logsWatchLoggerModelQueue.Count;
                        List<WatchLoggerModel> listLogs = new List<WatchLoggerModel>();
                        for (int i = 0; i < count; i++)
                        {
                            WatchLoggerModel content = new WatchLoggerModel();
                            lock (lockWatchLog)
                            {
                                content = logsWatchLoggerModelQueue.Dequeue();
                            }
                            listLogs.Add(content);
                        }
                        SQLDbHelper.BatchInsertLog(listLogs);
                    }
                    catch (Exception ex) { }
                    Task.Delay(timeOut).Wait();
                }
            });
        }

    }
}
