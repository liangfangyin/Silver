using Newtonsoft.Json;
using TouchSocket.Core;
using TouchSocket.Sockets;

namespace Silver.Net.Tcp
{
    public class TcpClients
    {

        /// <summary>
        /// 成功连接到服务器
        /// </summary>
        public event ConnectedHand ConnectedEvent;
        public delegate void ConnectedHand(ITcpClient client, TouchSocketEventArgs e);

        /// <summary>
        /// 从服务器断开连接，当连接不成功时不会触发
        /// </summary>
        public event DisconnectedHand DisconnectedEvent;
        public delegate void DisconnectedHand(ITcpClientBase client, TouchSocketEventArgs e);

        /// <summary>
        /// 接收数据
        /// </summary>
        public event ReceivedHand ReceivedEvent;
        public delegate void ReceivedHand(TcpClient client, byte[] buffer);
         
        public TcpClient tcpClient = new TcpClient();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Address">地址</param>
        /// <param name="TryOut">重连次数  0：不重试  -1：无限重连，大于0，链接次数</param>
        /// <param name="TimeOut">连接超时时间</param>
        /// <param name="BufferLength">缓存长度</param>
        public TcpClients(string Address = "127.0.0.1:7789", int TryOut = -1, int TimeOut = 1000, int BufferLength = 1024 * 10)
        {
            //成功连接到服务器
            tcpClient.Connected += (client, e) =>
            {
                if (ConnectedEvent != null)
                {
                    ConnectedEvent(client, e);
                }
            };
            //从服务器断开连接，当连接不成功时不会触发。
            tcpClient.Disconnected += (client, e) =>
            {
                if (DisconnectedEvent != null)
                {
                    DisconnectedEvent(client, e);
                }
            };
            //接收数据
            tcpClient.Received += (client, byteBlock, requestInfo) =>
            {
                if (ReceivedEvent != null)
                {
                    ReceivedEvent(client, byteBlock.Buffer);
                }
            };

            //声明配置
            TouchSocketConfig config = new TouchSocketConfig();
            if (TryOut == 0)
            {
                config.UsePlugin();
                config.ConfigurePlugins(t =>
                {
                    t.UseReconnection(0, false, TimeOut);
                });
            }
            else
            {
                config.UsePlugin();
                config.ConfigurePlugins(t =>
                {
                    t.UseReconnection(TryOut, true, TimeOut);
                });
            }
            config.SetRemoteIPHost(new IPHost(Address)).UsePlugin().SetBufferLength(BufferLength);

            //载入配置
            tcpClient.Setup(config);
            tcpClient.Connect();
        }
         
        /// <summary>
        /// 发送信息
        /// </summary>
        /// <param name="messAge"></param>
        public void Send(string messAge)
        {
            tcpClient.Send(messAge);
        }

        /// <summary>
        /// 发送信息-异步
        /// </summary>
        /// <param name="messAge"></param>
        public void SendAsync(string messAge)
        {
            tcpClient.SendAsync(messAge);
        }

        /// <summary>
        /// 发送信息JSON
        /// </summary>
        /// <param name="messAge"></param>
        public void SendJson(object messAge)
        {
            tcpClient.Send(JsonConvert.SerializeObject(messAge));
        }

        /// <summary>
        /// 发送信息JSON-异步
        /// </summary>
        /// <param name="messAge"></param>
        public void SendJsonAsync(object messAge)
        {
            tcpClient.SendAsync(JsonConvert.SerializeObject(messAge));
        }

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="messAge"></param>
        public void Send(byte[] messAge)
        {
            tcpClient.Send(messAge);
        }

        /// <summary>
        /// 发送数据-异步
        /// </summary>
        /// <param name="messAge"></param>
        public void SendAsync(byte[] messAge)
        {
            tcpClient.SendAsync(messAge);
        }

        /// <summary>
        /// 关闭
        /// </summary>
        public void Close()
        { 
            tcpClient.Close();
        } 

    }
}
