using IoTClient.Clients.Modbus;
using System;
using System.IO.Ports;

namespace Silver.Net.IoT
{
    /// <summary>
    /// ModbusRtu协议客户端
    /// </summary>
    public class ModbusRtuExtension : IDisposable
    {

        private ModbusRtuClient client;

        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="portName">COM端口名称</param>
        /// <param name="baudRate">波特率</param>
        /// <param name="dataBits">数据位</param>
        /// <param name="stopBits">停止位</param>
        /// <param name="parity">奇偶校验</param>
        /// <param name="timeout">超时时间（毫秒）</param>
        public ModbusRtuClient GetInstance(string portName="COM1", int baudRate=1, int dataBits=8, StopBits stopBits= StopBits.One, Parity parity= Parity.None, int timeout = 1500)
        {
            client = new ModbusRtuClient(portName, baudRate, dataBits, stopBits, parity, timeout);
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
