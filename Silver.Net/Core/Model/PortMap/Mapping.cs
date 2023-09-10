using System;
using System.Collections.Generic;
using System.Text;

namespace Silver.Net.Model
{
    public class Mapping
    {
        /// <summary>
        /// 本地IP
        /// </summary>
        public string localIP { get; set; } = "0.0.0.0";

        /// <summary>
        /// 本地端口
        /// </summary>
        public int localPort { get; set; } = 8001;

        /// <summary>
        /// 云端IP
        /// </summary>
        public string CloneIP { get; set; } = "183.157.203.100";

        /// <summary>
        /// 云端端口
        /// </summary>
        public int ClonePort { get; set; } = 8001;
    }
}
