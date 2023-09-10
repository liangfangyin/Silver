using Silver.Basic;
using Silver.Nacos.Core;
using Silver.Nacos.Core.Model;
using Silver.Nacos.Core.Model.Request;
using Silver.Nacos.Core.Model.Respone;
using Silver.Net;
using Silver.Net.Model.Http;
using System;
using System.Net;
using System.Text;

namespace Silver.Nacos.Service
{
    public class ForGetService
    {

        /// <summary>
        /// Get请求
        /// </summary>
        /// <param name="forGetRequest"></param>
        public static (bool, String, string) GetForAsk(ForGetRequest forGetRequest)
        {
            var result = RequestResult(forGetRequest, DataNacosVars.Get);
            if (result.StatusCode != HttpStatusCode.OK)
            {
                return (false, "", result.Html);
            }
            return (true, result.Html, "成功");
        }

        /// <summary>
        /// Get请求
        /// </summary>
        /// <param name="forGetRequest"></param>
        public static (bool, T, string) GetForAsk<T>(ForGetRequest forGetRequest)
        {
            var result = ForGetService.GetForAsk(forGetRequest);
            if (result.Item1 == false)
            {
                return (false, default(T), result.Item3);
            }
            return (true, result.Item2.JsonToObject<T>(), result.Item3);
        }


        /// <summary>
        /// Post请求
        /// </summary>
        /// <param name="forGetRequest"></param>
        public static (bool, String, string) PostForAsk(ForGetRequest forGetRequest)
        {
            var result = RequestResult(forGetRequest, DataNacosVars.Post);
            if (result.StatusCode != HttpStatusCode.OK)
            {
                return (false, "", result.Html);
            }
            return (true, result.Html, "成功");
        }

        /// <summary>
        /// Post请求
        /// </summary>
        /// <param name="forGetRequest"></param>
        public static (bool, T, string) PostForAsk<T>(ForGetRequest forGetRequest)
        {
            var result = ForGetService.PostForAsk(forGetRequest);
            if (result.Item1 == false)
            {
                return (false, default(T), result.Item3);
            }
            return (true, result.Item2.JsonToObject<T>(), result.Item3);
        }

        /// <summary>
        /// Put请求
        /// </summary>
        /// <param name="forGetRequest"></param>
        public static (bool, String, string) PutForAsk(ForGetRequest forGetRequest)
        {
            var result = RequestResult(forGetRequest, DataNacosVars.Put);
            if (result.StatusCode != HttpStatusCode.OK)
            {
                return (false, "", result.Html);
            }
            return (true, result.Html, "成功");
        }

        /// <summary>
        /// Put请求
        /// </summary>
        /// <param name="forGetRequest"></param>
        public static (bool, T, string) PutForAsk<T>(ForGetRequest forGetRequest)
        {
            var result = ForGetService.PutForAsk(forGetRequest);
            if (result.Item1 == false)
            {
                return (false, default(T), result.Item3);
            }
            return (true, result.Item2.JsonToObject<T>(), result.Item3);
        }

        /// <summary>
        /// Delete请求
        /// </summary>
        /// <param name="forGetRequest"></param>
        public static (bool, String, string) DeleteForAsk(ForGetRequest forGetRequest)
        {
            var result = RequestResult(forGetRequest, DataNacosVars.Delete);
            if (result.StatusCode != HttpStatusCode.OK)
            {
                return (false, "", result.Html);
            }
            return (true, result.Html, "成功");
        }

        /// <summary>
        /// Delete请求
        /// </summary>
        /// <param name="forGetRequest"></param>
        public static (bool, T, string) DeleteForAsk<T>(ForGetRequest forGetRequest)
        {
            var result = ForGetService.DeleteForAsk(forGetRequest);
            if (result.Item1 == false)
            {
                return (false, default(T), result.Item3);
            }
            return (true, result.Item2.JsonToObject<T>(), result.Item3);
        }

        /// <summary>
        /// 公共调用
        /// </summary>
        /// <param name="forGetRequest"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private static HttpResult RequestResult(ForGetRequest forGetRequest, string method)
        {
            NacosListInstanceHosts infoInstanceHosts = NacosUtil.SelectOneHealthyInstance(new NacosListInstanceRequest() { serviceName = forGetRequest.serverName, groupName = forGetRequest.groupName, namespaceId = forGetRequest.nameSpaceId });
            if (infoInstanceHosts == null)
            {
                throw new Exception("找不到服务");
            }
            string urls = $"{forGetRequest.agree}://{infoInstanceHosts.ip}:{infoInstanceHosts.port}{forGetRequest.urls}";
            StringBuilder sbParame = new StringBuilder();
            if (forGetRequest.parames != null)
            {
                if (forGetRequest.parames.GetType() == typeof(String))
                {
                    sbParame.Append(forGetRequest.parames);
                }
                else
                {
                    if (forGetRequest.contentType.ToLower() != ForGetContentType.jsonData.ToLower())
                    {
                        sbParame.Append(QualityEntry.ProjectKeyOfValue(forGetRequest.parames));
                    }
                    else
                    {
                        sbParame.Append(forGetRequest.parames.ToJson());
                    }
                }
            }
            HttpUtil httpExtension = new HttpUtil();
            var result = httpExtension.GetHtml(new HttpItem()
            {
                URL = urls,
                Postdata = sbParame.ToString(),
                ContentType = forGetRequest.contentType,
                Timeout = forGetRequest.defaultTimeOut,
                Header= forGetRequest.header,
                Method = method
            }); 
            return result;
        }

    }
}
