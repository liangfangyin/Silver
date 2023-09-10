using Silver.Nacos.Core;
using Silver.Nacos.Core.Model;
using Silver.Nacos.Core.Model.Request;
using Silver.Net;
using Silver.Net.Model.Http;
using System;
using System.Net;
using System.Text;

namespace Silver.Nacos.Service
{
    public class NacosConfigService
    {
        /// <summary>
        /// 获取配置信息
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static string SelectOfConfig(NacosConfigRequest request)
        {
            StringBuilder sbParame = new StringBuilder();
            if (!string.IsNullOrEmpty(request.tenant))
            {
                sbParame.Append($"&tenant={request.tenant}");
            }
            if (!string.IsNullOrEmpty(request.dataId))
            {
                sbParame.Append($"&dataId={request.dataId}");
            }
            if (!string.IsNullOrEmpty(request.group))
            {
                sbParame.Append($"&group={request.group}");
            }
            HttpUtil httpExtension = new HttpUtil();
            var result = httpExtension.GetHtml(new HttpItem()
            {
                URL = DataNacosVars.GetApiUrl($"/nacos/v1/cs/configs?accessToken={NacosUtil.nacosLoginRespone.accessToken}{sbParame.ToString()}"), 
                ContentType = DataNacosVars.ContentType,
                Timeout= NacosUtil.nacosRegisterInstanceRequest.defaultTimeOut,
                Method = DataNacosVars.Get
            });
            if (result.StatusCode!= HttpStatusCode.OK)
            {
                Console.WriteLine(result.Html);
                return null;
            }
            return result.Html;
        }

        /// <summary>
        /// 获取配置信息
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static T SelectOfConfig<T>(NacosConfigRequest request)
        {
            StringBuilder sbParame = new StringBuilder();
            if (!string.IsNullOrEmpty(request.tenant))
            {
                sbParame.Append($"&tenant={request.tenant}");
            }
            if (!string.IsNullOrEmpty(request.dataId))
            {
                sbParame.Append($"&dataId={request.dataId}");
            }
            if (!string.IsNullOrEmpty(request.group))
            {
                sbParame.Append($"&group={request.group}");
            }
            HttpUtil httpExtension = new HttpUtil();
            var result = httpExtension.AskRequest<T>(new HttpItem()
            {
                URL = DataNacosVars.GetApiUrl($"/nacos/v1/cs/configs?accessToken={NacosUtil.nacosLoginRespone.accessToken}{sbParame.ToString()}"),
                ContentType = DataNacosVars.ContentType,
                Timeout = NacosUtil.nacosRegisterInstanceRequest.defaultTimeOut,
                Method = DataNacosVars.Get
            });
            if (result.Item1 == false)
            {
                Console.WriteLine(result.Item3);
                return default(T);
            }
            return result.Item2;
        }

        /// <summary>
        /// 发布设置
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static bool SetOfConfig(NacosAddConfigRequest request)
        {
            HttpUtil httpExtension = new HttpUtil();
            var result = httpExtension.GetHtml(new HttpItem()
            {
                URL = DataNacosVars.GetApiUrl($"/nacos/v1/cs/configs?accessToken={NacosUtil.nacosLoginRespone.accessToken}"),
                Postdata = QualityEntry.ProjectKeyOfValue(request),
                ContentType = DataNacosVars.ContentType,
                Timeout = NacosUtil.nacosRegisterInstanceRequest.defaultTimeOut,
                Method = DataNacosVars.Post
            });
            if (result.StatusCode != HttpStatusCode.OK)
            {
                Console.WriteLine(result.Html);
                return false;
            }
            return result.Html.ToLower()=="true";
        }
         
        /// <summary>
        /// 删除设置
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static bool DeleteOfConfig(NacosDeleteConfigRequest request)
        {
            HttpUtil httpExtension = new HttpUtil();
            var result = httpExtension.GetHtml(new HttpItem()
            {
                URL = DataNacosVars.GetApiUrl($"/nacos/v1/cs/configs?accessToken={NacosUtil.nacosLoginRespone.accessToken}"),
                Postdata = QualityEntry.ProjectKeyOfValue(request),
                ContentType = DataNacosVars.ContentType,
                Timeout = NacosUtil.nacosRegisterInstanceRequest.defaultTimeOut,
                Method = DataNacosVars.Delete
            });
            if (result.StatusCode != HttpStatusCode.OK)
            {
                Console.WriteLine(result.Html);
                return false;
            }
            return result.Html.ToLower() == "true";
        }

    }
}
