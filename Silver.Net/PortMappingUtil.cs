using Silver.Net.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Threading;

namespace Silver.Net
{
    /// <summary>
    /// 端口映射
    /// </summary>
    public class PortMappingUtil
    {
        public static Dictionary<int, MappingSocket> dicMapping = new Dictionary<int, MappingSocket>();
        private Mapping _mapping = new Mapping();

        public void AddMapping(Mapping item)
        {
            this._mapping = item;
            MappingSocket infoMapping = new MappingSocket();
            if (dicMapping.ContainsKey(item.localPort))
            {
                KillProcess(item.localPort);
                infoMapping = dicMapping[item.localPort];
                infoMapping.myThread.Interrupt();
                infoMapping.serverSocket.Close();
            }
            try
            {
                IPAddress ip = IPAddress.Parse(_mapping.localIP);
                Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                serverSocket.ReceiveTimeout = 10000;
                serverSocket.SendTimeout = 10000;
                serverSocket.Bind(new IPEndPoint(ip, _mapping.localPort));
                serverSocket.Listen(0);
                Thread myThread = new Thread(ListenClient);
                myThread.IsBackground = true;
                myThread.Start(serverSocket);
                infoMapping.IsStart = true;
                infoMapping.myThread = myThread;
                infoMapping.serverSocket = serverSocket;
            }
            catch
            {
                infoMapping.IsStart = false;
            }
            infoMapping.CloneIP = item.CloneIP;
            infoMapping.ClonePort = item.ClonePort;
            infoMapping.localIP = item.localIP;
            infoMapping.localPort = item.localPort;
            if (dicMapping.ContainsKey(item.localPort))
            {
                dicMapping[item.localPort] = infoMapping;
            }
            else
            {
                dicMapping.Add(item.localPort, infoMapping);
            }
        }

        /// <summary>
        /// 监听客户端
        /// </summary>
        /// <param name="obj"></param>
        private void ListenClient(object obj)
        {
            Socket serverSocket = (Socket)obj;
            IPAddress ip = IPAddress.Parse(_mapping.CloneIP);
            while (true)
            {
                try
                {
                    Socket tcp1 = serverSocket.Accept();
                    Socket tcp2 = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    tcp2.Connect(new IPEndPoint(ip, _mapping.ClonePort));
                    //目标主机返回数据
                    ThreadPool.QueueUserWorkItem(new WaitCallback(SwapMsg), new thSock
                    {
                        tcp1 = tcp2,
                        tcp2 = tcp1
                    });
                    //中间主机请求数据
                    ThreadPool.QueueUserWorkItem(new WaitCallback(SwapMsg), new thSock
                    {
                        tcp1 = tcp1,
                        tcp2 = tcp2
                    });
                }
                catch (Exception ex)
                {
                    if (serverSocket.Connected)
                    {
                        serverSocket.Close();
                    }
                    break;
                }
            }
        }

        /// <summary>
        /// 两个 tcp 连接 交换数据，一发一收
        /// </summary>
        /// <param name="obj"></param>
        public void SwapMsg(object obj)
        {
            thSock mSocket = (thSock)obj;
            var infoMapping = dicMapping[_mapping.localPort];
            while (true)
            {
                try
                {
                    byte[] result = new byte[1024];
                    int resultCount = mSocket.tcp2.Receive(result, result.Length, SocketFlags.None);
                    if (resultCount == 0)
                    {
                        if (mSocket.tcp1.Connected)
                        {
                            mSocket.tcp1.Close();
                        }
                        if (mSocket.tcp2.Connected)
                        {
                            mSocket.tcp2.Close();
                        }
                    }
                    int sendCount = mSocket.tcp1.Send(result, resultCount, SocketFlags.None);
                    infoMapping.record += resultCount;
                    infoMapping.send += sendCount;
                    dicMapping[_mapping.localPort] = infoMapping;
                }
                catch (Exception ex)
                {
                    if (mSocket.tcp1.Connected)
                    {
                        mSocket.tcp1.Close();
                    }
                    if (mSocket.tcp2.Connected)
                    {
                        mSocket.tcp2.Close();
                    }
                    break;
                }
            }
        }

        /// <summary>
        /// 查询运行端口进程，并且结束
        /// </summary>
        /// <param name="port"></param>
        public void KillProcess(int port) //调用方法，传参
        {
            try
            {
                Process pro = new Process();
                // 设置命令行、参数
                pro.StartInfo.FileName = "cmd.exe";
                pro.StartInfo.UseShellExecute = false;
                pro.StartInfo.RedirectStandardInput = true;
                pro.StartInfo.RedirectStandardOutput = true;
                pro.StartInfo.RedirectStandardError = true;
                pro.StartInfo.CreateNoWindow = true;
                // 启动CMD
                pro.Start();
                // 运行端口检查命令
                pro.StandardInput.WriteLine("netstat -ano");
                pro.StandardInput.WriteLine("exit");

                // 获取结果
                Regex reg = new Regex(@"\s ", RegexOptions.Compiled);
                string line = null;
                while ((line = pro.StandardOutput.ReadLine()) != null)
                {
                    line = line.Trim();
                    if (line.StartsWith("TCP", StringComparison.OrdinalIgnoreCase))
                    {
                        line = reg.Replace(line, ",");

                        string[] arr = line.Split(',');
                        if (arr[2].EndsWith(":" + port))
                        {
                            Console.WriteLine("8002端口的进程ID：{0}", arr[17]);
                            try
                            {
                                Process thisproc = Process.GetProcessById(Convert.ToInt32(arr[17]));
                                thisproc.Close();
                            }
                            catch { }
                        }

                    }
                }
            }
            catch { }
        }


    }
}
