using Microsoft.Extensions.Configuration;

namespace Silver.Ocelot.Nacos.Model
{
    public class NacosConfigFactory
    {
        public NacosConfigFactory(IConfiguration configuration, string nacosUrl="")
        {
            NacosConfigFactory.ServerAddresses = configuration.GetSection("nacos:ServerAddresses").Value;
            if (!string.IsNullOrEmpty(nacosUrl))
            {
                NacosConfigFactory.ServerAddresses = nacosUrl;
            }
            NacosConfigFactory.DefaultTimeOut = Convert.ToInt32(configuration.GetSection("nacos:DefaultTimeOut").Value);
            NacosConfigFactory.ListenInterval = Convert.ToInt32(configuration.GetSection("nacos:ListenInterval").Value);
            NacosConfigFactory.ServiceName = configuration.GetSection("nacos:ServiceName").Value;
            NacosConfigFactory.GroupName = configuration.GetSection("nacos:GroupName").Value;
            NacosConfigFactory.Namespace = configuration.GetSection("nacos:Namespace").Value;

            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    Nacos.ListGetNacos = Nacos.ListGetNacos.Where(t => (DateTime.Now - t.NowDate).TotalSeconds <= 5).ToList();
                    Thread.Sleep(500);
                }
            });
        }

        public static string ServerAddresses { get; set; } = "";

        public static int DefaultTimeOut { get; set; }

        public static int ListenInterval { get; set; }

        public static string ServiceName { get; set; } = "";

        public static string GroupName { get; set; } = "";

        public static string Namespace { get; set; } = "";


    }
}
