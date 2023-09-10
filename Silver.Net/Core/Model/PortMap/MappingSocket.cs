using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Silver.Net.Model
{
    public class MappingSocket
    {
        /// <summary>
        /// 发送流量
        /// </summary>
        public long send { get; set; }

        /// <summary>
        /// 接收流量
        /// </summary>
        public long record { get; set; }

        public Socket serverSocket { get; set; }

        public Thread myThread { get; set; }

        /// <summary>
        /// 本地IP
        /// </summary>
        public string localIP { get; set; } = "0.0.0.0";

        /// <summary>
        /// 本地端口
        /// </summary>
        public int localPort { get; set; } = 8002;

        /// <summary>
        /// 云端IP
        /// </summary>
        public string CloneIP { get; set; } = "183.157.203.100";

        /// <summary>
        /// 云端端口
        /// </summary>
        public int ClonePort { get; set; } = 8001;

        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsStart { get; set; } = false;

    }
}
