using System;
using System.Collections.Generic;
using System.Text;

namespace Silver.Nacos.Core.Model
{
    public class DataNacosVars
    {

        public static string ContentType = "application/x-www-form-urlencoded; charset=utf-8";

        public static string Post = "POST";

        public static string Get = "GET";

        public static string Put = "PUT";

        public static string Delete = "DELETE";

        public static string GetApiUrl(string urls)
        {
            return $"{NacosUtil.nacosRegisterInstanceRequest.serverAddresses}{urls}";
        }


    }
}
