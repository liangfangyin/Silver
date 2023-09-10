using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace Silver.RabbitMQ
{
    public class RabbitMQSetting
    {

        /// <summary>
        /// 注册队列
        /// </summary>
        public static List<string> ListRabbitMQ = new List<string>();

        /// <summary>
        /// RabbitMQ 链接
        /// </summary>
        public static IConnection connection { get; set; }

        /// <summary>
        /// RabbitMQ 渠道
        /// </summary>
        public static IModel channel { get; set; }



    }
}
