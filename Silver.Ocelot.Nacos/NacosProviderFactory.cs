using Microsoft.Extensions.DependencyInjection;
using Nacos.V2;
using Ocelot.ServiceDiscovery;

namespace Silver.Ocelot.Nacos
{
    public static class NacosProviderFactory
    {
        public static ServiceDiscoveryFinderDelegate Get = (provider, config, route) =>
        {
            var service = provider.GetService<INacosNamingService>();
            if (config.Type?.ToLower() == "nacos" && service != null)
            {
                return new Nacos(route.ServiceName, service);
            }
            return null;
        };
    }
}
