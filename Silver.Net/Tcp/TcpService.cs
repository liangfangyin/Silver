using System;
using System.Collections.Generic;
using TouchSocket.Core;
using TouchSocket.Sockets;

namespace Silver.Net.Tcp
{
    public class TcpServices
    {
       
        /// <summary>
        /// 有客户端正在连接
        /// </summary>
        public event ConnectingHand ConnectingEvent;
        public delegate void ConnectingHand(SocketClient client);

        /// <summary>
        /// 有客户端连接
        /// </summary>
        public event ConnectedHand ConnectedEvent;
        public delegate void ConnectedHand(SocketClient client, TouchSocketEventArgs e);

        /// <summary>
        /// 有客户端断开连接
        /// </summary>
        public event DisconnectedHand DisconnectedEvent;
        public delegate void DisconnectedHand(SocketClient client, TouchSocketEventArgs e);

        /// <summary>
        /// 从客户端收到信息
        /// </summary>
        public event ReceivedHand ReceivedEvent;
        public delegate void ReceivedHand(SocketClient client, byte[] buffer);

        public TcpService service = new TcpService();
        public List<IPHost> listIpHost = new List<IPHost>();
        public int threadCount = 10;
        public int maxCount = 10000;


        /// <summary>
        /// 初始化TCP服务
        /// </summary>
        /// <param name="Ports">IP:端口   多个用因为逗号隔开  127.0.0.1:6379,6380,127.0.0.1:6987</param>
        /// <param name="ThreadCount">多线程数</param>
        /// <param name="MaxCount">最大链接数</param>
        /// <exception cref="Exception"></exception>
        public TcpServices(string Ports,int ThreadCount=10, int MaxCount=10000)
        {
            this.maxCount = MaxCount;
            this.threadCount = ThreadCount;
            this.listIpHost = new List<IPHost>();
            if (string.IsNullOrEmpty(Ports))
            {
                throw new Exception("端口不能为空");
            }
            foreach (string post in Ports.Split(","))
            {
                listIpHost.Add(new IPHost(post));
            } 
            //有客户端正在连接
            service.Connecting += (client, e) =>
            {
                if (ConnectingEvent != null)
                {
                    ConnectingEvent(client);
                }
            };
            //有客户端连接
            service.Connected += (client, e) =>
            {
                if (ConnectedEvent != null)
                {
                    ConnectedEvent(client, e);
                }
            };
            //有客户端断开连接
            service.Disconnected += (client, e) =>
            {
                if (DisconnectedEvent != null)
                {
                    DisconnectedEvent(client, e);
                }
            };
            //从客户端收到信息
            service.Received += (client, byteBlock, requestInfo) =>
            {
                if (ReceivedEvent != null)
                {
                    ReceivedEvent(client, byteBlock.Buffer);
                }
                //从客户端收到信息
                //string mes = Encoding.UTF8.GetString(byteBlock.Buffer, 0, byteBlock.Len);
                //Console.WriteLine($"已从{client.ID}接收到信息：{mes}");

                //client.Send(mes);//将收到的信息直接返回给发送方

                //client.Send("id",mes);//将收到的信息返回给特定ID的客户端

                //var ids = service.GetIDs();
                //foreach (var clientId in ids)//将收到的信息返回给在线的所有客户端。
                //{
                //    if (clientId != client.ID)//不给自己发
                //    {
                //        service.Send(clientId, mes);
                //    }
                //}
            };
        }

        /// <summary>
        /// 启动
        /// </summary>
        public void StartAt()
        {
            service.Setup(new TouchSocketConfig()//载入配置     
                    .SetListenIPHosts(listIpHost.ToArray())//同时监听两个地址
                    .SetMaxCount(maxCount)
                    .SetThreadCount(threadCount)
                    .ConfigurePlugins(a =>
                    {
                        //a.Add();//此处可以添加插件
                    })
                    .ConfigureContainer(a =>
                    {

                    }))
                    .Start();//启动
        }


        public void Close()
        {
            service.Dispose();
            service.Stop();
        }


    }
}
