using IoTClient.Clients.Modbus;
using System;

namespace Silver.Net.IoT
{
    /// <summary>
    /// Modbus Tcp协议客户端
    /// </summary>
    public class ModbusTcpExtension : IDisposable
    {
        private ModbusTcpClient client;

        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="ip">IP地址</param>
        /// <param name="port">端口</param>
        /// <param name="timeout">超时时间</param>
        /// <returns></returns>
        public ModbusTcpClient GetInstance(string ip="127.0.0.1", int port=1234, int timeout = 1500)
        {
            client = new ModbusTcpClient(ip, port, timeout);
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
