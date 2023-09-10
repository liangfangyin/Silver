using System;
using System.Collections.Generic;
using System.Text;

namespace Silver.ThirdApi.Model.Currency
{
    /// <summary>
    /// 根据IP获取地址
    /// </summary>
    public class ipNewV3
    {
        public RequestData parame { get; set; } = new RequestData();

        public string apiUrl = "http://apis.juhe.cn/ip/ipNewV3";

        public RequestMethod Method { get; set; } = RequestMethod.GET;

        public Respone respone { get; set; } = new Respone();

        public class RequestData
        {
            /// <summary>
            /// IP地址
            /// </summary>
            public string ip { get; set; }

            public string key { get; set; } = "574b61f9a6d1b97d2c91d8b6f881cb58";
        }

        public class Respone: ResponeResult
        { 
            public ResponeData result { get; set; }

            public class ResponeData
            { 
                public string Country { get; set; }
                public string Province { get; set; }
                public string City { get; set; }
                public string District { get; set; }
                public string Isp { get; set; } 

            }

        }

    }
}
