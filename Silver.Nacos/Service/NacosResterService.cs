using Microsoft.Extensions.Caching.Memory;
using Silver.Basic;
using Silver.Nacos.Core;
using Silver.Nacos.Core.Model;
using Silver.Nacos.Core.Model.Request;
using Silver.Nacos.Core.Model.Respone;
using Silver.Net;
using Silver.Net.Model.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Silver.Nacos.Service
{
    public class NacosResterService
    {

        /// <summary>
        /// 用户登录
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static NacosLoginRespone Login(NacosLoginRequest request)
        {
            HttpUtil httpExtension = new HttpUtil();
            var result = httpExtension.AskRequest<NacosLoginRespone>(new HttpItem()
            {
                URL = DataNacosVars.GetApiUrl("/nacos/v1/auth/users/login"),
                Postdata = QualityEntry.ProjectKeyOfValue(request),
                ContentType = DataNacosVars.ContentType,
                Timeout = NacosUtil.nacosRegisterInstanceRequest.defaultTimeOut,
                Method = DataNacosVars.Post
            });
            if (result.Item1 == false)
            {
                throw new Exception(result.Item3);
            }
            NacosUtil.lastTokenDate = DateTime.Now.AddSeconds(result.Item2.tokenTtl);
            return result.Item2 as NacosLoginRespone;
        }

        /// <summary>
        ///  注册
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static bool Register(NacosRegisterInstanceRequest registerRequest)
        {
            var loginResult = NacosResterService.Login(new NacosLoginRequest() { username = registerRequest.userName, password = registerRequest.passWord });
            if (loginResult == null)
            {
                return false;
            }
            if (registerRequest.ip.Length <= 0)
            {
                registerRequest.ip = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName()).AddressList.FirstOrDefault(address => address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)?.ToString();
            }
            if (registerRequest.port <= 0)
            {
                registerRequest.port = 80;
            }
            NacosUtil.nacosLoginRespone = loginResult;
            NacosUtil.nacosRegisterInstanceRequest = registerRequest; 
            var request = new {
                ip= registerRequest.ip,
                port = registerRequest.port,
                namespaceId = registerRequest.namespaceId,
                weight = registerRequest.weight,
                enabled = registerRequest.enabled,
                healthy = registerRequest.healthy,
                metadata = registerRequest.metadata,
                clusterName = registerRequest.clusterName,
                serviceName = registerRequest.serviceName,
                groupName = registerRequest.groupName,
                ephemeral = registerRequest.ephemeral,
            };
            HttpUtil httpExtension = new HttpUtil();
            var result = httpExtension.GetHtml(new HttpItem()
            {
                URL = DataNacosVars.GetApiUrl($"/nacos/v1/ns/instance?accessToken={NacosUtil.nacosLoginRespone.accessToken}"),
                Postdata = QualityEntry.ProjectKeyOfValue(request),
                ContentType = DataNacosVars.ContentType,
                Timeout = registerRequest.defaultTimeOut,
                Method = DataNacosVars.Post
            });
            if (result.StatusCode != HttpStatusCode.OK)
            {
                throw new Exception(result.Html);
            }
            NacosResterService.BeatInstance(new NacosBeatInstanceRequest()
            {
                serviceName = registerRequest.serviceName,
                groupName = registerRequest.groupName,
                ephemeral = registerRequest.ephemeral,
                beat = (new { now = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), ip = NacosUtil.nacosRegisterInstanceRequest.ip, port = NacosUtil.nacosRegisterInstanceRequest.port }).ToJson()
            });
            return result.Html.ToLower() == "ok";
        }

        /// <summary>
        /// 注销
        /// </summary>
        /// <param name="canelInstanceRequest"></param>
        /// <exception cref="Exception"></exception>
        public static void Stopping()
        {
            HttpUtil httpExtension = new HttpUtil();
            var result = httpExtension.GetHtml(new HttpItem()
            {
                URL = DataNacosVars.GetApiUrl($"/nacos/v1/ns/instance?accessToken={NacosUtil.nacosLoginRespone.accessToken}"),
                Postdata = QualityEntry.ProjectKeyOfValue(new NacosCanelInstanceRequest()
                {
                    serviceName = NacosUtil.nacosRegisterInstanceRequest.serviceName,
                    groupName = NacosUtil.nacosRegisterInstanceRequest.groupName,
                    ip = NacosUtil.nacosRegisterInstanceRequest.ip,
                    port = NacosUtil.nacosRegisterInstanceRequest.port,
                    clusterName = NacosUtil.nacosRegisterInstanceRequest.clusterName,
                    namespaceId = NacosUtil.nacosRegisterInstanceRequest.namespaceId,
                    ephemeral = NacosUtil.nacosRegisterInstanceRequest.ephemeral
                }),
                ContentType = DataNacosVars.ContentType,
                Timeout = NacosUtil.nacosRegisterInstanceRequest.defaultTimeOut,
                Method = DataNacosVars.Delete
            });
            if (result.StatusCode != HttpStatusCode.OK)
            {
                throw new Exception(result.Html);
            }
        }

        /// <summary>
        /// 修改实例
        /// </summary>
        /// <param name="updateInstanceRequest"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static bool SetInstance(NacosUpdateInstanceRequest updateInstanceRequest)
        {
            var loginResult = NacosResterService.Login(new NacosLoginRequest() { username = updateInstanceRequest.userName, password = updateInstanceRequest.passWord });
            if (loginResult == null)
            {
                return false;
            }
            var insertInstance = NacosUtil.nacosRegisterInstanceRequest;
            insertInstance.ip = updateInstanceRequest.ip;
            insertInstance.port = updateInstanceRequest.port;
            insertInstance.userName = updateInstanceRequest.userName;
            insertInstance.passWord = updateInstanceRequest.passWord;
            insertInstance.serviceName = updateInstanceRequest.serviceName;
            insertInstance.groupName = updateInstanceRequest.groupName;
            insertInstance.clusterName = updateInstanceRequest.clusterName;
            insertInstance.namespaceId = updateInstanceRequest.namespaceId;
            insertInstance.weight = updateInstanceRequest.weight;
            NacosUtil.nacosLoginRespone = loginResult;
            NacosUtil.nacosRegisterInstanceRequest = insertInstance;
            HttpUtil httpExtension = new HttpUtil();
            var result = httpExtension.GetHtml(new HttpItem()
            {
                URL = DataNacosVars.GetApiUrl($"/nacos/v1/ns/instance?accessToken={NacosUtil.nacosLoginRespone.accessToken}"),
                Postdata = QualityEntry.ProjectKeyOfValue(updateInstanceRequest), 
                ContentType = DataNacosVars.ContentType,
                Timeout = NacosUtil.nacosRegisterInstanceRequest.defaultTimeOut,
                Method = DataNacosVars.Put
            });
            if (result.StatusCode != HttpStatusCode.OK)
            {
                throw new Exception(result.Html);
            }
            return result.Html.ToLower() == "ok";
        }

        /// <summary>
        /// 实例
        /// </summary>
        /// <param name="listInstanceRequest"></param>
        public static NacosListInstanceHosts SelectOneHealthyInstance(NacosListInstanceRequest listInstanceRequest)
        {
            string cacheKey = $"Nacos:{listInstanceRequest.serviceName}";
            MemoryCache memoryCache = new MemoryCache(new MemoryCacheOptions());
            if (memoryCache.TryGetValue(cacheKey, out _))
            {
                return Balancer.GetHostByRandomWeight(memoryCache.Get<List<NacosListInstanceHosts>>($"Nacos:{listInstanceRequest.serviceName}"));
            }
            StringBuilder sbParame = new StringBuilder();
            if (!string.IsNullOrEmpty(listInstanceRequest.serviceName))
            {
                sbParame.Append($"&serviceName={listInstanceRequest.serviceName}");
            }
            if (!string.IsNullOrEmpty(listInstanceRequest.groupName))
            {
                sbParame.Append($"&groupName={listInstanceRequest.groupName}");
            }
            if (!string.IsNullOrEmpty(listInstanceRequest.namespaceId))
            {
                sbParame.Append($"&namespaceId={listInstanceRequest.namespaceId}");
            }
            if (!string.IsNullOrEmpty(listInstanceRequest.clusters))
            {
                sbParame.Append($"&clusters={listInstanceRequest.clusters}");
            }
            sbParame.Append($"&healthyOnly={listInstanceRequest.healthyOnly}");
            HttpUtil httpExtension = new HttpUtil();
            var result = httpExtension.AskRequest<NacosListInstanceRespone>(new HttpItem()
            {
                URL = DataNacosVars.GetApiUrl($"/nacos/v1/ns/instance/list?accessToken={NacosUtil.nacosLoginRespone.accessToken}{sbParame.ToString()}"),
                ContentType = DataNacosVars.ContentType,
                Timeout = NacosUtil.nacosRegisterInstanceRequest.defaultTimeOut,
                Method = DataNacosVars.Get
            });
            if (result.Item1 == false)
            {
                throw new Exception(result.Item3);
            }
            foreach (var item in result.Item2.hosts)
            {
                item.valid = item.healthy;
            }
            memoryCache.Set(cacheKey, result.Item2.hosts, new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromSeconds(3)));
            return Balancer.GetHostByRandomWeight(result.Item2.hosts); 
        }

        /// <summary>
        /// 注册心跳
        /// </summary>
        /// <param name="beatInstanceRequest"></param>
        public static void BeatInstance(NacosBeatInstanceRequest beatInstanceRequest)
        {
            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    try
                    {
                        beatInstanceRequest.beat = new
                        {
                            serviceName = NacosUtil.nacosRegisterInstanceRequest.serviceName,
                            groupName = NacosUtil.nacosRegisterInstanceRequest.groupName,
                            ip = NacosUtil.nacosRegisterInstanceRequest.ip,
                            port = NacosUtil.nacosRegisterInstanceRequest.port,
                            cluster = NacosUtil.nacosRegisterInstanceRequest.clusterName,
                            weight = NacosUtil.nacosRegisterInstanceRequest.weight,
                            now = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                            scheduled = true,
                        }.ToJson();
                        HttpUtil httpExtension = new HttpUtil();
                        var result = httpExtension.GetHtml(new HttpItem()
                        {
                            URL = DataNacosVars.GetApiUrl($"/nacos/v1/ns/instance/beat?accessToken={NacosUtil.nacosLoginRespone.accessToken}"),
                            Postdata = QualityEntry.ProjectKeyOfValue(beatInstanceRequest),
                            ContentType = DataNacosVars.ContentType,
                            Timeout = NacosUtil.nacosRegisterInstanceRequest.defaultTimeOut,
                            Method = DataNacosVars.Put
                        });
                        if (result.StatusCode != HttpStatusCode.OK)
                        {
                            Console.WriteLine($"Nacos心跳：{result.Html}");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                    }
                    finally
                    {
                        Thread.Sleep(NacosUtil.nacosRegisterInstanceRequest.bearTimeOut);
                    }
                }
            });

            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    try
                    {
                        if (Math.Abs((NacosUtil.lastTokenDate - DateTime.Now).TotalSeconds) > 200)
                        {
                            return;
                        }
                        NacosResterService.Login(new NacosLoginRequest() { username = NacosUtil.nacosRegisterInstanceRequest.userName, password = NacosUtil.nacosRegisterInstanceRequest.passWord });
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"自动登录：{ex.ToString()}");
                    }
                    finally
                    {
                        Thread.Sleep(5000);
                    }
                }
            });
        }

    }
}
