using IoTClient.Clients.PLC;
using IoTClient.Common.Enums;
using System;

namespace Silver.Net.IoT
{
    /// <summary>
    /// 西门子客户端
    /// </summary>
    public class SiemensExtension : IDisposable
    {

        private SiemensClient client;

        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="ip">IP地址</param>
        /// <param name="port">端口</param>
        /// <param name="timeout">超时时间</param>
        /// <returns></returns>
        public SiemensClient GetInstance(SiemensVersion version= SiemensVersion.None, string ip = "127.0.0.1", int port = 1234, int timeout = 1500)
        {
            byte slot = 0;
            byte rack = 0;
            client = new SiemensClient(version, ip, port, slot, rack, timeout);
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
