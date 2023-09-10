using Microsoft.Extensions.Configuration;
using System;
using System.Runtime.InteropServices;

namespace Silver.Basic
{
    /// <summary>
    /// 配置帮助类
    /// </summary>
    public class ConfigurationUtil
    {
        /// <summary>
        /// 应用全局配置
        /// </summary>
        public static IConfiguration Configuration { get; set; }

        /// <summary>
        /// 获取配置信息，支持环境变量
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetSection(string key)
        {
            string value = Environment.GetEnvironmentVariable(key, RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? EnvironmentVariableTarget.Machine : EnvironmentVariableTarget.Process);
            if (value.IsNotEmpty())
            {
                return value;
            }
            return Configuration.GetSection(key).Value ?? "";
        }
    }
}
