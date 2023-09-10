using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Silver.Cache
{
    /// <summary>
    /// 本地缓存
    /// </summary>
    public class MemoryCacheUtil
    {
        private string cache_key = "";
        private MemoryCache Cache = new MemoryCache(new MemoryCacheOptions());

        /// <summary>
        /// 验证缓存项是否存在
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <returns></returns>
        public bool Exists(string key)
        {
            return Cache.TryGetValue(cache_key + key, out _);
        }
         
        /// <summary>
        /// 添加缓存
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <param name="value">缓存Value</param>
        /// <param name="expiresIn">缓存时长</param> 
        /// <returns></returns>
        public void Set(string key, object value, TimeSpan expiresIn)
        {
            string json = JsonConvert.SerializeObject(value);
            Cache.Set(cache_key + key, json, new MemoryCacheEntryOptions().SetSlidingExpiration(expiresIn)); 
        }

        /// <summary>
        /// 删除缓存
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <returns></returns>
        public void Remove(string key)
        {
            Cache.Remove(cache_key + key);
        }

        /// <summary>
        /// 批量删除缓存
        /// </summary>
        /// <param name="keys">缓存Key集合</param>
        public void RemoveAll(IEnumerable<string> keys)
        {
            keys.ToList().ForEach(item => Cache.Remove(cache_key + item));
        }

        /// <summary>
        /// 删除所有缓存
        /// </summary>
        public void RemoveCacheAll()
        {
            var l = GetCacheKeys();
            foreach (var s in l)
            {
                Cache.Remove(s);
            }
        }

        /// <summary>
        /// 删除匹配到的缓存
        /// </summary>
        /// <param name="pattern">匹配字符串</param>
        /// <returns></returns>
        public void RemoveCacheRegex(string pattern)
        {
            IList<string> l = SearchCacheRegex(pattern);
            foreach (var s in l)
            {
                Cache.Remove(s);
            }
        }

        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <returns></returns>
        public T Get<T>(string key)
        {
            return Cache.Get<T>(cache_key + key);
        }

        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <returns></returns>
        public string GetString(string key)
        {
            return Cache.Get(cache_key + key).ToString();
        }


        /// <summary>
        /// 获取缓存集合
        /// </summary>
        /// <param name="keys">缓存Key集合</param>
        /// <returns></returns>
        public IDictionary<string, object> GetAll(IEnumerable<string> keys)
        {
            var dict = new Dictionary<string, object>();
            keys.ToList().ForEach(item => dict.Add(item, Cache.Get(cache_key + item)));
            return dict;
        }

        /// <summary>
        /// 搜索 匹配到的缓存
        /// </summary>
        /// <param name="pattern"></param>
        /// <returns></returns>
        public IList<string> SearchCacheRegex(string pattern)
        {
            var cacheKeys = GetCacheKeys();
            var l = cacheKeys.Where(k => Regex.IsMatch(k, pattern)).ToList();
            return l.AsReadOnly();
        }
         
        /// <summary>
        /// 获取所有缓存键
        /// </summary>
        /// <returns></returns>
        public List<string> GetCacheKeys()
        {
            const BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;
            var entries = Cache.GetType().GetField("_entries", flags).GetValue(Cache);
            var cacheItems = entries as IDictionary;
            var keys = new List<string>();
            if (cacheItems == null) return keys;
            foreach (DictionaryEntry cacheItem in cacheItems)
            {
                if (cacheItem.Key.ToString().StartsWith(cache_key))
                {
                    keys.Add(cacheItem.Key.ToString());
                }
            }
            return keys;
        }

        public void Dispose()
        {
            Cache.Dispose();
        }


    }
}
