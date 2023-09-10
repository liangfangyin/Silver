using Nacos.V2;
using Nacos.V2.Common;
using Nacos.V2.Naming.Dtos;
using Ocelot.Infrastructure.Extensions;
using Ocelot.ServiceDiscovery.Providers;
using Ocelot.Values;
using Silver.Ocelot.Nacos.Model;
using Service = Ocelot.Values.Service;

namespace Silver.Ocelot.Nacos
{
    public class Nacos : IServiceDiscoveryProvider
    {
        private readonly string _serviceName;
        private readonly string _groupName;
        private readonly INacosNamingService _service;
        private const string VersionPrefix = "version-";
        public static List<NacosCacheService> ListGetNacos = new List<NacosCacheService>();

        public Nacos(string serviceName, INacosNamingService service)
        {
            var groupedName = serviceName.Split(Constants.SERVICE_INFO_SPLITER);
            _serviceName = groupedName.Length == 1 ? groupedName[0] : groupedName[1];
            _groupName = NacosConfigFactory.GroupName;
            _service = service;
        }

        public async Task<List<Service>> Get()
        {
            var listService = ListGetNacos.Where(t => t.ServiceName == _serviceName && t.GroupName == _groupName && (DateTime.Now - t.NowDate).TotalSeconds <= 5).ToList();
            if (ListGetNacos.Count > 0)
            {
                try
                {
                    return listService.FirstOrDefault().Services;
                }
                catch { }
            }
            var services = new List<Service>();
            var instances = await _service.GetAllInstances(_serviceName, _groupName);
            if (instances != null && instances.Any())
            {
                foreach (var instance in instances)
                {
                    services.Add(BuildService(instance));
                }
            }
            Nacos.ListGetNacos.Add(new NacosCacheService() { Services = services, GroupName = _groupName, ServiceName = _serviceName });
            return await Task.FromResult(services);
        }

        private Service BuildService(Instance instance)
        {
            var tags = GetTags(instance.Metadata);
            return new Service(instance.ServiceName, new ServiceHostAndPort(instance.Ip, instance.Port), instance.InstanceId, GetVersionFromStrings(tags), tags);
        }

        private List<string> GetTags(Dictionary<string, string> pairs)
        {
            var list = new List<string>();
            foreach (var key in pairs.Keys)
            {
                list.Add($"{key}-{pairs[key]}");
            }
            return list;
        }

        private string GetVersionFromStrings(IEnumerable<string> strings)
        {
            return strings
                ?.FirstOrDefault(x => x.StartsWith(VersionPrefix, StringComparison.Ordinal))
                .TrimStart(VersionPrefix);
        }
    }
}
