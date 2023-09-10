using Microsoft.Extensions.DependencyInjection;
using Nacos.AspNetCore.V2;
using Ocelot.DependencyInjection;
using Silver.Ocelot.Nacos.Model;

namespace Silver.Ocelot.Nacos
{
    public static class NacosOcelotUtil
    {
        /// <summary>
        /// Nacos注册
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IOcelotBuilder AddNacos(this IOcelotBuilder builder)
        {
            builder.Services.AddSingleton(new NacosConfigFactory(builder.Configuration));
            builder.Services.AddNacosAspNet(builder.Configuration);
            builder.Services.AddSingleton(NacosProviderFactory.Get);
            builder.Services.AddSingleton(NacosMiddlewareConfigurationProvider.Get);
            return builder;
        }

        /// <summary>
        /// Nacos注册
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="nacosUrl"></param>
        /// <returns></returns>
        public static IOcelotBuilder AddNacos(this IOcelotBuilder builder,string nacosUrl)
        { 
            builder.Services.AddSingleton(new NacosConfigFactory(builder.Configuration, nacosUrl));
            builder.Services.AddNacosAspNet(x =>
            {
                x.ServerAddresses = new List<string> { nacosUrl };
                x.EndPoint = "";
                x.Namespace = builder.Configuration.GetSection("nacos:Namespace").Value;
                x.ServiceName = builder.Configuration.GetSection("nacos:ServiceName").Value;
                x.GroupName = builder.Configuration.GetSection("nacos:GroupName").Value;
                x.ClusterName = builder.Configuration.GetSection("nacos:ClusterName").Value;
                x.UserName = builder.Configuration.GetSection("nacos:UserName").Value;
                x.Password = builder.Configuration.GetSection("nacos:Password").Value;
                x.Weight = Convert.ToDouble(builder.Configuration.GetSection("nacos:Weight").Value);
                x.RegisterEnabled = true;
                x.InstanceEnabled = true;
                x.Ephemeral = true;
                x.Secure = false;
                x.NamingUseRpc = false;
                x.DefaultTimeOut = Convert.ToInt32(builder.Configuration.GetSection("nacos:DefaultTimeOut").Value);
                x.ListenInterval = Convert.ToInt32(builder.Configuration.GetSection("nacos:ListenInterval").Value);
            });
            builder.Services.AddSingleton(NacosProviderFactory.Get);
            builder.Services.AddSingleton(NacosMiddlewareConfigurationProvider.Get);
            return builder;
        }

    }
}