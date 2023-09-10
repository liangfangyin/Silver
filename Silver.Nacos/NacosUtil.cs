using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Silver.Nacos.Core.Model.Request;
using Silver.Nacos.Core.Model.Respone;
using Silver.Nacos.Service;
using System;
using System.Threading;

namespace Silver.Nacos
{
    public static class NacosUtil
    {
        public static DateTime lastTokenDate=DateTime.Now.AddHours(2);
        public static NacosLoginRespone nacosLoginRespone = new NacosLoginRespone();
        public static NacosRegisterInstanceRequest nacosRegisterInstanceRequest=new NacosRegisterInstanceRequest();

        #region 服务注册与注销

        /// <summary>
        /// 服务注册-站点
        /// </summary>
        /// <param name="services"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public static IServiceCollection AddNacosAspNet(this IServiceCollection services, NacosRegisterInstanceRequest request)
        {
            ResetRegister:
            try
            {
                NacosResterService.Register(request);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("nacos链接成功");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.ForegroundColor = ConsoleColor.Red;
                Thread.Sleep(1000);
                goto ResetRegister;
            }
            return services;
        }

        /// <summary>
        /// 服务注册-终端
        /// </summary>
        /// <param name=""></param>
        /// <param name="request"></param>
        public static void AddNacosClient(NacosRegisterInstanceRequest request)
        {
            ResetRegister:
            try
            {
                NacosResterService.Register(request);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("nacos链接成功");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.ForegroundColor = ConsoleColor.Red;
                Thread.Sleep(1000);
                goto ResetRegister;
            }
        }

        /// <summary>
        /// 服务注销-站点
        /// </summary>
        /// <param name="app"></param>
        /// <param name="lifetime"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseNacosAspNet(this IApplicationBuilder app, IApplicationLifetime lifetime)
        {
            lifetime.ApplicationStopped.Register(() =>
            {
                NacosResterService.Stopping();
            });
            return app;
        }

        /// <summary>
        /// 服务注销-终端
        /// </summary>
        public static void UseNacosClient()
        {
            NacosResterService.Stopping();
        }

        #endregion

        #region 配置管理

        /// <summary>
        /// 获取配置信息
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns> 
        public static string SelectOfConfig(NacosConfigRequest request)
        {
            return NacosConfigService.SelectOfConfig(request);
        }

        /// <summary>
        /// 获取配置信息
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns> 
        public static T SelectOfConfig<T>(NacosConfigRequest request)
        {
            return NacosConfigService.SelectOfConfig<T>(request);
        }

        /// <summary>
        /// 发布设置
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static bool SetOfConfig(NacosAddConfigRequest request)
        {
            return NacosConfigService.SetOfConfig(request);
        }

        /// <summary>
        /// 删除设置
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static bool DeleteOfConfig(NacosDeleteConfigRequest request)
        {
            return NacosConfigService.DeleteOfConfig(request);
        }

        #endregion

        #region 服务调用

        /// <summary>
        /// 获取指定服务实例-权重均衡
        /// </summary>
        /// <param name="listInstanceRequest"></param>
        /// <returns></returns>
        public static NacosListInstanceHosts SelectOneHealthyInstance(NacosListInstanceRequest listInstanceRequest)
        {
            return NacosResterService.SelectOneHealthyInstance(listInstanceRequest);
        }

        /// <summary>
        /// Get请求
        /// </summary>
        /// <param name="forGetRequest"></param>
        public static (bool, String, string) GetForAsk(ForGetRequest forGetRequest)
        {
            return ForGetService.GetForAsk(forGetRequest);
        }

        /// <summary>
        /// Get请求
        /// </summary>
        /// <param name="forGetRequest"></param>
        public static (bool, T, string) GetForAsk<T>(ForGetRequest forGetRequest)
        {
            return ForGetService.GetForAsk<T>(forGetRequest);
        }


        /// <summary>
        /// Post请求
        /// </summary>
        /// <param name="forGetRequest"></param>
        public static (bool, String, string) PostForAsk(ForGetRequest forGetRequest)
        {
            return ForGetService.PostForAsk(forGetRequest);
        }

        /// <summary>
        /// Post请求
        /// </summary>
        /// <param name="forGetRequest"></param>
        public static (bool, T, string) PostForAsk<T>(ForGetRequest forGetRequest)
        {
            return ForGetService.PostForAsk<T>(forGetRequest);
        }

        /// <summary>
        /// Put请求
        /// </summary>
        /// <param name="forGetRequest"></param>
        public static (bool, String, string) PutForAsk(ForGetRequest forGetRequest)
        {
            return ForGetService.PutForAsk(forGetRequest);
        }

        /// <summary>
        /// Put请求
        /// </summary>
        /// <param name="forGetRequest"></param>
        public static (bool, T, string) PutForAsk<T>(ForGetRequest forGetRequest)
        {
            return ForGetService.PutForAsk<T>(forGetRequest);
        }

        /// <summary>
        /// Delete请求
        /// </summary>
        /// <param name="forGetRequest"></param>
        public static (bool, String, string) DeleteForAsk(ForGetRequest forGetRequest)
        {
            return ForGetService.DeleteForAsk(forGetRequest);
        }

        /// <summary>
        /// Delete请求
        /// </summary>
        /// <param name="forGetRequest"></param>
        public static (bool, T, string) DeleteForAsk<T>(ForGetRequest forGetRequest)
        {
            return ForGetService.DeleteForAsk<T>(forGetRequest);
        }

        #endregion

    }
}
