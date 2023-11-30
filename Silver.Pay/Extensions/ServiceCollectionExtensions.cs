using Essensoft.Paylink.Alipay;
using Essensoft.Paylink.WeChatPay;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Silver.Pay.Extensions
{
    public static class ServiceCollectionExtensions
    {

        public static void AddPay(this IServiceCollection services, IConfiguration Configuration)
        {
            // 添加Paylink依赖注入
            services.AddAlipay();
            services.AddWeChatPay();
        }

    }
}
