using System;
using System.Collections.Generic;
using System.Text;

namespace Silver.Nacos.Core.Model.Respone
{
    public class NacosListInstanceRespone
    {
        public string dom { get; set; } = "";

        public int cacheMillis { get; set; }

        public bool useSpecifiedURL { get; set; }

        public string checksum { get; set; } = "";

        public long lastRefTime { get; set; }

        public string env { get; set; } = "";

        public string clusters { get; set; } = "";

        public bool valid { get; set; }

        public List<NacosListInstanceHosts> hosts { get; set; } = new List<NacosListInstanceHosts>();

    }

    public class NacosListInstanceHosts
    {
        public string instanceId { get; set; } = "";

        public string ip { get; set; }

        public int port { get; set; }

        public double weight { get; set; }

        public bool healthy { get; set; }

        public bool enabled { get; set; }

        public bool ephemeral { get; set; }

        public string clusterName { get; set; } = "";

        public string serviceName { get; set; } = "";

        public bool valid { get; set; }

        public bool marked { get; set; }

        public object metadata { get; set; }

        public int instanceHeartBeatInterval { get; set; }


    }


}
