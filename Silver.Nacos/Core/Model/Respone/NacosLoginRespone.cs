using System;
using System.Collections.Generic;
using System.Text;

namespace Silver.Nacos.Core.Model.Respone
{
    public class NacosLoginRespone
    {

        /// <summary>
        /// Token
        /// </summary>
        public string accessToken { get; set; } = "";

        /// <summary>
        /// 有效时间
        /// </summary>
        public int tokenTtl { get; set; }

        /// <summary>
        /// 是否管理员
        /// </summary>
        public bool globalAdmin { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string username { get; set; } = "";

    }
}
