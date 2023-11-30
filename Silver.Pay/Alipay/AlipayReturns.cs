using Essensoft.Paylink.Alipay;
using Essensoft.Paylink.Alipay.Notify;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Silver.Pay.Alipay
{
    public class AlipayReturns
    {

        private readonly IAlipayNotifyClient _client;
        private readonly AlipayOptions _optionsAccessor;

        public AlipayReturns(IAlipayNotifyClient client, AlipayOptions optionsAccessor)
        { 
            _client = client;
            _optionsAccessor = optionsAccessor;
        }

        /// <summary>
        /// 电脑网站支付 - 同步跳转
        /// </summary>
        public async Task<AlipayNotify> PagePay(IDictionary<string, string> Request)
        {
            return await _client.ExecuteAsync<AlipayTradePagePayReturn>(Request, _optionsAccessor);
        }

        /// <summary>
        /// 手机网站支付 - 同步跳转
        /// </summary>
        public async Task<AlipayNotify> WapPay(IDictionary<string, string> Request)
        {
            return await _client.ExecuteAsync<AlipayTradeWapPayReturn>(Request, _optionsAccessor); 
        }

    }
}
