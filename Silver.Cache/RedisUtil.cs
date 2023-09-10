using CSRedis;
using System;
using System.ComponentModel;
using System.IO;
using Silver.Cache.Model;
using Silver.Basic;

namespace Silver.Cache
{
    /// <summary>
    /// Redis
    /// </summary>
    public class RedisUtil
    { 
        private static CSRedisClient csredis;
        private event PropertyChangedEventHandler PropertyChanged;

        public RedisUtil()
        {
            IsConnection();
        }


        private static void IsConnection()
        {
            if (csredis == null)
            {
                //普通模式和集群模式
                if (ConfigurationUtil.GetSection("Redis:Mode").ToInt() == 1)
                {
                    csredis = new CSRedisClient(ConfigurationUtil.GetSection("Redis:Connection"));
                }
                //哨兵模式
                else if (ConfigurationUtil.GetSection("Redis:Mode").ToInt() == 2)
                {
                    string[] redisConnection = ConfigurationUtil.GetSection("Redis:Connection").Split(';');
                    csredis = new CSRedisClient(ConfigurationUtil.GetSection("Redis:Sentry"), redisConnection);
                }
                //分区模式
                else if (ConfigurationUtil.GetSection("Redis:Mode").ToInt() == 3)
                {
                    string[] redisConnection = ConfigurationUtil.GetSection("Redis:Connection").Split(';');
                    csredis = new CSRedisClient(null, redisConnection);
                }
                else
                {
                    csredis = new CSRedisClient(ConfigurationUtil.GetSection("Redis:Connection"));
                }
                RedisHelper.Initialization(csredis);
            }
        }

        public CSRedisClient GetClient()
        {
            return csredis;
        }

        /// <summary>
        /// 一小时内随机秒
        /// </summary>
        /// <returns></returns>
        public static int RandomHour()
        {
            Random rdom = new Random();
            return rdom.Next(100,3600);
        }


        /// <summary>
        /// 一日内随机秒
        /// </summary>
        /// <returns></returns>
        public static int RandomDay()
        { 
            Random rdom = new Random();
            return rdom.Next(3600, 3600 * 24);
        }

        /// <summary>
        /// 一周内随机秒
        /// </summary>
        /// <returns></returns>
        public static int RandomWeek()
        {
            Random rdom = new Random();
            return rdom.Next(3600 * 24, 3600 * 24 * 7);
        }

        /// <summary>
        /// 一月内随机秒
        /// </summary>
        /// <returns></returns>
        public static int RandomMonth()
        {
            Random rdom = new Random();
            return rdom.Next(3600 * 24, 3600 * 24 * 31);
        }

        /// <summary>
        /// 分布式锁
        /// </summary>
        /// <param name="cachekey">键名称</param>
        /// <param name="value">值</param>
        /// <param name="secondsTimeout">过期时间（毫秒）</param>
        /// <returns></returns>
        public bool CapSetNX(string cachekey, string value, int secondsTimeout = 1000)
        {
            string NamespacePrefix = "cap_lock_";
            string key = NamespacePrefix + cachekey;
            try
            {
                var result = csredis.SetNx(key, value);
                var setnx = result;
                if (setnx == true)
                {
                    csredis.Set(key, value, TimeSpan.FromMilliseconds(secondsTimeout));//将Key缓存5秒
                }
                return setnx;
            }
            catch (Exception ex)
            {
                return false;
            }
        }


        /// <summary>
        /// 释放分布式锁
        /// </summary>
        /// <param name="client"></param>
        /// <param name="cachekey">键名称</param>
        /// <param name="value">值</param> 
        /// <returns></returns>
        public bool CapUnSetNX(string cachekey)
        {
            string NamespacePrefix = "cap_lock_";
            string key = NamespacePrefix + cachekey;
            try
            {
                csredis.Del(key);//将Key缓存5秒 
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// 设置锁名称
        /// </summary>
        private string SetLockName { get; set; } = "lockkey";

        /// <summary>
        /// 设置超时时间，触发事件
        /// </summary>
        public int SetLockTime
        {
            get { return SetLockTime; }
            set
            {
                var lockTimeout = value;//单位是毫秒
                var currentTime = DateTime.Now.Ticks;
                if (csredis.SetNx(SetLockName, DateTime.Now.Ticks + lockTimeout))
                {
                    //设置过期时间
                    csredis.Expire(SetLockName, TimeSpan.FromMilliseconds(lockTimeout));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SetLock"));
                    Console.WriteLine("执行完毕");
                    csredis.Del(SetLockName);
                }
                else
                {
                    //未获取到锁，继续判断，判断时间戳看看是否可以重置并获取锁
                    int resetNum = 0;
                ResetLock:
                    var lockValue = csredis.Get(SetLockName);
                    var time = DateTime.Now.Ticks;
                    if (!string.IsNullOrEmpty(lockValue) && time > Convert.ToInt64(lockValue))
                    {
                        //再次用当前时间戳getset
                        //返回固定key的旧值，旧值判断是否可以获取锁
                        var getsetResult = csredis.GetSet(SetLockName, time);
                        if (getsetResult == null || (getsetResult != null && getsetResult == lockValue))
                        {
                            Console.WriteLine("获取到Redis锁了");
                            //真正获取到锁
                            csredis.Expire(SetLockName, TimeSpan.FromMilliseconds(5000));
                            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SetLock"));
                            Console.WriteLine("执行完毕");
                            csredis.Del(SetLockName);
                            return;
                        }
                    }
                    Console.WriteLine("未拿到锁");
                    if (resetNum <= 3)
                    {
                        resetNum++;
                        goto ResetLock;
                    }
                }
            }
        }

    }
}
