using System;
using System.Collections.Generic;
using System.Text;

namespace Silver.Nacos.Core.Model.Request
{
    public class NacosLoginRequest
    {

        /// <summary>
        /// 用户名
        /// </summary>
        public string username { get; set; } = "";

        /// <summary>
        /// 密码
        /// </summary>
        public string password { get; set; } = "";


    }
}
