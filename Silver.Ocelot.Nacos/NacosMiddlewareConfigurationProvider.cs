using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Ocelot.Configuration.Repository;
using Ocelot.Middleware;

namespace Silver.Ocelot.Nacos
{
    public class NacosMiddlewareConfigurationProvider
    {
        public static OcelotMiddlewareConfigurationDelegate Get = builder =>
        {
            var internalConfigRepo = builder.ApplicationServices.GetService<IInternalConfigurationRepository>();
            var config = internalConfigRepo.Get();

            var hostLifetime = builder.ApplicationServices.GetService<IHostApplicationLifetime>();

            return Task.CompletedTask;
        };
    }
}
