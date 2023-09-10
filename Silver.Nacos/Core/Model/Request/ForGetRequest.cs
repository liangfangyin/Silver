using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Silver.Nacos.Core.Model.Request
{
    public class ForGetRequest
    {
        /// <summary>
        /// 服务名称 -必填
        /// </summary>
        public string serverName { get; set; } = "";

        /// <summary>
        /// 分组名-选填
        /// </summary>
        public string groupName { get; set; } = "DEFAULT_GROUP";

        /// <summary>
        /// 命名空间-选填
        /// </summary>
        public string nameSpaceId { get; set; } = "";

        /// <summary>
        /// 接口地址-必填，如 /v1/member/login
        /// </summary>
        public string urls { get; set; } = "";

        /// <summary>
        /// 参数-选填
        /// </summary>
        public object parames { get; set; } = null;

        /// <summary>
        /// 是否json提交数据-选填
        /// </summary>
        public bool isJson { get; set; } = false;

        /// <summary>
        /// 请求协议
        /// </summary>
        public string agree { get; set; } = "http";

        /// <summary>
        /// 调用超时时间
        /// </summary>
        public int defaultTimeOut { get; set; } = 5000;

        /// <summary>
        /// 头-选填
        /// </summary>
        public WebHeaderCollection header { get; set; } = new WebHeaderCollection();

        /// <summary>
        /// 请求格式-选填
        /// </summary>
        public string contentType { get; set; } = ForGetContentType.jsonData;

    }

    /// <summary>
    /// 请求方式
    /// </summary>
    public class ForGetContentType
    {
        /// <summary>
        /// form 表单
        /// </summary>
        public static string formUrlencoded = "application/x-www-form-urlencoded;charset=utf-8";

        /// <summary>
        /// 表单
        /// </summary>
        public static string formData = "multipart/form-data";

        /// <summary>
        /// json方式
        /// </summary>
        public static string jsonData = "application/json;charset=utf-8";

        /// <summary>
        /// xml方式
        /// </summary>
        public static string xmlData = "text/xml";


    }


}
