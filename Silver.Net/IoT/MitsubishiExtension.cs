using IoTClient.Clients.PLC;
using IoTClient.Enums;
using System;

namespace Silver.Net.IoT
{
    /// <summary>
    /// 三菱plc 客户端
    /// </summary>
    public class MitsubishiExtension : IDisposable
    {

        private MitsubishiClient client;

        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="ip">IP地址</param>
        /// <param name="port">端口</param>
        /// <param name="timeout">超时时间</param>
        /// <returns></returns>
        public MitsubishiClient GetInstance(string ip = "127.0.0.1", int port = 1234, int timeout = 1500)
        {
            client = new MitsubishiClient(MitsubishiVersion.Qna_3E, ip, port, timeout);
            client.Open();
            return client;
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            if (client != null)
            {
                client.Close();
            }
        }

    }
}
