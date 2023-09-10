using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Silver.Nacos.Core.Model.Respone;

namespace Silver.Nacos.Core
{
    public class Balancer
    {
        private static readonly string _uniqueKey = "nacos-sdk-csharp";

        public static NacosListInstanceHosts GetHostByRandomWeight(List<NacosListInstanceHosts> hosts)
        {
            if (hosts == null || !hosts.Any()) return null;

            List<Pair<NacosListInstanceHosts>> hostsWithWeight = new List<Pair<NacosListInstanceHosts>>();
            foreach (var host in hosts)
            {
                if (host.valid)
                {
                    hostsWithWeight.Add(new Pair<NacosListInstanceHosts>(host, host.weight));
                }
            }

            Chooser<string, NacosListInstanceHosts> vipChooser = new Chooser<string, NacosListInstanceHosts>(_uniqueKey);
            vipChooser.Refresh(hostsWithWeight);
            return vipChooser.RandomWithWeight();
        }

        public static NacosListInstanceHosts GetHostByRandom(List<NacosListInstanceHosts> hosts)
        {
            if (hosts == null || !hosts.Any()) return null;

            List<Pair<NacosListInstanceHosts>> hostsWithWeight = new List<Pair<NacosListInstanceHosts>>();
            foreach (var host in hosts)
            {
                if (host.valid)
                {
                    hostsWithWeight.Add(new Pair<NacosListInstanceHosts>(host, host.weight));
                }
            }

            Chooser<string, NacosListInstanceHosts> vipChooser = new Chooser<string, NacosListInstanceHosts>(_uniqueKey);
            vipChooser.Refresh(hostsWithWeight);
            return vipChooser.Random();
        }
    }
}
