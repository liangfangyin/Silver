using Newtonsoft.Json;
using System.Net;
using System.Net.Sockets;
using TouchSocket.Core;
using TouchSocket.Sockets;

namespace Silver.Net.Udp
{
    public class UdpClientServer
    {
        /// <summary>
        /// 接收数据
        /// </summary>
        public event ReceivedHand ReceivedEvent;
        public delegate void ReceivedHand(EndPoint client, byte[] buffer);


        UdpSession udpSession = new UdpSession();
        public UdpClientServer(string Address= "127.0.0.1:7789")
        { 
            udpSession.Received += (endpoint, byteBlock, requestInfo) =>
            {
                if (ReceivedEvent != null)
                {
                    ReceivedEvent(endpoint, byteBlock.Buffer);
                }
            }; 
            udpSession.Setup(new TouchSocketConfig()
                .SetBindIPHost(new IPHost(Address))
                .SetUdpDataHandlingAdapter(() => new UdpPackageAdapter()));//加载配置
            udpSession.Start();//启动
              
        }


        /// <summary>
        /// 发送信息
        /// </summary>
        /// <param name="messAge"></param>
        public void Send(string messAge)
        {
            udpSession.Send(messAge);
        }

        /// <summary>
        /// 发送信息-异步
        /// </summary>
        /// <param name="messAge"></param>
        public void SendAsync(string messAge)
        {
            udpSession.SendAsync(messAge);
        }

        /// <summary>
        /// 发送信息JSON
        /// </summary>
        /// <param name="messAge"></param>
        public void SendJson(object messAge)
        {
            udpSession.Send(JsonConvert.SerializeObject(messAge));
        }

        /// <summary>
        /// 发送信息JSON-异步
        /// </summary>
        /// <param name="messAge"></param>
        public void SendJsonAsync(object messAge)
        {
            udpSession.SendAsync(JsonConvert.SerializeObject(messAge));
        }

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="messAge"></param>
        public void Send(byte[] messAge)
        {
            udpSession.Send(messAge);
        }

        /// <summary>
        /// 发送数据-异步
        /// </summary>
        /// <param name="messAge"></param>
        public void SendAsync(byte[] messAge)
        {
            udpSession.SendAsync(messAge);
        }
         
        /// <summary>
        /// 关闭
        /// </summary>
        public void Close()
        { 
            udpSession.Dispose();
            udpSession.Stop();
        }
         
    }
}
