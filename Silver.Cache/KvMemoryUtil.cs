using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Silver.Cache.Model;

namespace Silver.Cache
{
    /// <summary>
    /// K-V内存库
    /// </summary>
    public class KvMemoryUtil
    {

        //键列表
        private static Dictionary<string, MemoryInfo> keyValues = new Dictionary<string, MemoryInfo>();
        //键过期列表
        private static Dictionary<string, DateTime> keyTimers = new Dictionary<string, DateTime>();
        //键更新  1：添加、更新  2：删除 
        private static Queue<QueueMessage> logs_queue = new Queue<QueueMessage>();
        //本地写入队列
        private static Queue<QueueMessage> logs_queue_copy = new Queue<QueueMessage>();


        /// <summary>
        /// 验证缓存项是否存在
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <returns></returns>
        public bool Exists(string key)
        {
            return KvMemoryUtil.keyValues.ContainsKey(key);
        }

        /// <summary>
        /// 添加缓存
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <param name="value">缓存Value</param>
        /// <param name="expiresIn">缓存时长</param> 
        /// <returns></returns>
        public void Set(string key, object value, int expiresIn = -1)
        {
            DateTime endTime = expiresIn == -1 ? DateTime.MaxValue : DateTime.Now.AddSeconds(expiresIn);
            keyValues[key] = new MemoryInfo()
            {
                value = value
            };
            keyTimers[key] = endTime;
            AutoResetEvent:
            try
            {
                lock (objlock)
                {
                    logs_queue.Enqueue(new QueueMessage() { name = key, typeid = 1, endTime = endTime });
                }
            }
            catch
            {
                Thread.Sleep(1);
                goto AutoResetEvent;
            }
        }

        /// <summary>
        /// 删除缓存
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <returns></returns>
        public void Remove(string key)
        {
            if (Exists(key))
            {
                keyValues.Remove(key);
                keyTimers.Remove(key);
                lock (objlock)
                {
                    logs_queue.Enqueue(new QueueMessage() { name = key, typeid = 2 });
                }
            }
        }

        /// <summary>
        /// 批量删除缓存
        /// </summary>
        /// <param name="keys">缓存Key集合</param>
        public void RemoveAll(List<string> keys)
        {
            if (keys == null)
            {
                return;
            }
            lock (objlock)
            {
                foreach (var item in keys)
                {
                    if (Exists(item))
                    {
                        keyValues.Remove(item);
                        keyTimers.Remove(item);
                        logs_queue.Enqueue(new QueueMessage() { name = item, typeid = 2 });
                    }
                }
            }
        }

        /// <summary>
        /// 删除所有缓存
        /// </summary>
        public void RemoveCacheAll()
        {
            foreach (var item in keyTimers.Keys)
            {
                logs_queue.Enqueue(new QueueMessage() { name = item, typeid = 2 });
            }
            keyValues.Clear();
            keyTimers.Clear();
        }

        /// <summary>
        /// 删除匹配到的缓存
        /// </summary>
        /// <param name="pattern">匹配字符串</param>
        /// <returns></returns>
        public void RemoveCacheRegex(string pattern)
        {
            var listKey = keyTimers.Where(t => t.Key.Contains(pattern)).Select(t => t.Key).ToList();
            foreach (var item in listKey)
            {
                keyValues.Remove(item);
                keyTimers.Remove(item);
                lock (objlock)
                {
                    logs_queue.Enqueue(new QueueMessage() { name = item, typeid = 2 });
                }
            }
        }

        /// <summary>
        /// 获取匹配的key
        /// </summary>
        /// <param name="pattern"></param>
        /// <returns></returns>
        public List<string> GetRegexKeys(string pattern)
        {
            return keyTimers.Where(t => t.Key.Contains(pattern)).Select(t => t.Key).ToList();
        }
         
        /// <summary>
        /// 获取缓存-实体类
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <returns></returns>
        public T Get<T>(string key)
        {
            return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(keyValues[key].value));
        }

        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <returns></returns>
        public string GetString(string key)
        {
            if (!Exists(key))
            {
                return "";
            }
            return keyValues[key].value.ToString();
        }

        /// <summary>
        /// 获取缓存-整形
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <returns></returns>
        public int GetInt(string key)
        {
            if (!Exists(key))
            {
                return 0;
            }
            return Convert.ToInt32(keyValues[key].value.ToString());
        }

        /// <summary>
        /// 获取缓存-长整形
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <returns></returns>
        public long GetLong(string key)
        {
            if (!Exists(key))
            {
                return 0;
            }
            return Convert.ToInt64(keyValues[key].value.ToString());
        }


        /// <summary>
        /// 获取缓存集合
        /// </summary>
        /// <param name="keys">缓存Key集合</param>
        /// <returns></returns>
        public Dictionary<string, object> GetAll(List<string> keys)
        {
            Dictionary<string, object> keyval = new Dictionary<string, object>();
            var listKey = keyTimers.Where(t => keys.Contains(t.Key)).Select(t => t.Key).ToList();
            foreach (var key in listKey)
            {
                keyval[key] = keyValues[key].value;
            }
            return keyval;
        }

        /// <summary>
        /// 获取所有缓存键
        /// </summary>
        /// <returns></returns>
        public List<string> GetCacheKeys()
        {
            return keyTimers.Select(t => t.Key).ToList();
        }

        /// <summary>
        /// 持久化到本地服务
        /// </summary>
        private static object objlock = new object();
        public static void Persistence()
        {
            Task.Factory.StartNew(() =>
            {
                Reset:
                if (logs_queue_copy.Count == 0)
                {
                    lock (objlock)
                    {
                        logs_queue_copy = logs_queue;
                        logs_queue.Clear();
                    }
                }
                Thread.Sleep(1000);
                goto Reset;
            });

            Task.Factory.StartNew(() =>
            {
                Reset:
                string path = $"{Directory.GetCurrentDirectory()}/PersisLogs/";
                if (!File.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                if (logs_queue_copy.Count > 0)
                {
                    try
                    {
                        int count = logs_queue_copy.Count;
                        List<QueueMessage> kvitem = new List<QueueMessage>();
                        for (int i = 0; i < count; i++)
                        {
                            kvitem.Add(logs_queue_copy.Dequeue());
                        }
                        foreach (var item in kvitem)
                        {
                            try
                            {
                                //添加、更新
                                if (item.typeid == 1)
                                {
                                    object value = keyValues[item.name].value;
                                    item.type = value.GetType().Name.ToLower();
                                    if (value.GetType().Name.ToLower() == "string" || value.GetType().Name.ToLower() == "int32" || value.GetType().Name.ToLower() == "int64")
                                    {
                                        item.val = value.ToString();
                                    }
                                    else
                                    {
                                        item.type = "json";
                                        item.val = JsonConvert.SerializeObject(value);
                                    }
                                    File.WriteAllText($"{path}/{item.name}.txt", JsonConvert.SerializeObject(item));
                                }
                                //删除
                                else if (item.typeid == 2)
                                {
                                    if (File.Exists($"{path}/{item.name}.txt"))
                                    {
                                        File.Delete($"{path}/{item.name}.txt");
                                    }
                                }
                            }
                            catch { }
                        }
                    }
                    catch { }
                }
                Thread.Sleep(5000);
                goto Reset;
            });
        }

        /// <summary>
        /// 本地导入内存初始化服务
        /// </summary>
        public static void InitPersistence()
        {
            Task.Factory.StartNew(() =>
            {
                string path = $"{Directory.GetCurrentDirectory()}/PersisLogs/";
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                string[] files = Directory.GetFiles(path);
                if (files.Length <= 0)
                {
                    KvMemoryUtil.LastTimePersis();
                    return;
                }
                KvMemoryUtil info = new KvMemoryUtil();
                foreach (var item in files)
                {
                    try
                    {
                        string content = File.ReadAllText(item);
                        if (string.IsNullOrEmpty(content))
                        {
                            continue;
                        }
                        var result = JsonConvert.DeserializeObject<QueueMessage>(content);
                        int timer = -1;
                        if (result.endTime.ToString("yyyy-MM-dd") == "9999-12-31")
                        {
                            timer = -1;
                        }
                        else
                        {
                            timer = Convert.ToInt32((result.endTime - DateTime.Now).TotalSeconds);
                        }
                        if (result.type == "string")
                        {
                            info.Set(result.name, result.val, timer);
                        }
                        else if (result.type == "int32")
                        {
                            info.Set(result.name, Convert.ToInt32(result.val), timer);
                        }
                        else if (result.type == "int64")
                        {
                            info.Set(result.name, Convert.ToInt64(result.val), timer);
                        }
                        else
                        {
                            info.Set(result.name, JsonConvert.DeserializeObject(result.val), timer);
                        }
                    }
                    catch { }
                }
                KvMemoryUtil.LastTimePersis();
            });
        }

        /// <summary>
        /// 过期自动清除服务
        /// </summary>
        private static void LastTimePersis()
        {
            Task.Factory.StartNew(() =>
            {
                AutoResetEvent:
                try
                {
                    string path = $"{Directory.GetCurrentDirectory()}/PersisLogs/";
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                    KvMemoryUtil info = new KvMemoryUtil();
                    string[] files = Directory.GetFiles(path);
                    if (files.Length <= 0)
                    {
                        return;
                    }
                    foreach (var item in files)
                    {
                        try
                        {
                            string name = item.Substring(item.LastIndexOf("/") + 1).Replace(".txt", "");
                            DateTime endTime = keyTimers[name];
                            if (DateTime.Now > endTime)
                            {
                                info.Remove(name);
                            }
                        }
                        catch { }
                    }
                }
                catch { }
                Thread.Sleep(1000 * 60 * 1);
                goto AutoResetEvent;
            });
        }

    }
}
