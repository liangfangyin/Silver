using Essensoft.Paylink.WeChatPay;
using Essensoft.Paylink.WeChatPay.V3;
using Essensoft.Paylink.WeChatPay.V3.Notify;
using System.Threading.Tasks;

namespace Silver.Pay.WeChatPay
{
    public class WeChatNotify
    {

        private readonly IWeChatPayNotifyClient _client;
        private readonly WeChatPayOptions _optionsAccessor;

        public WeChatNotify(IWeChatPayNotifyClient client, WeChatPayOptions optionsAccessor)
        { 
            _client = client;
            _optionsAccessor = optionsAccessor;
        }

        #region 微信结果通知

        /// <summary>
        /// 支付结果通知
        /// </summary>
        /// <param name="headers"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        public async Task<WeChatPayTransactionsNotify> Transactions(WeChatPayHeaders headers, string body)
        {
            return await _client.ExecuteAsync<WeChatPayTransactionsNotify>(headers, body, _optionsAccessor);
        }

        /// <summary>
        /// 退款结果通知
        /// </summary>
        public async Task<WeChatPayRefundDomesticRefundsNotify> Refund(WeChatPayHeaders headers, string body)
        {
            return await _client.ExecuteAsync<WeChatPayRefundDomesticRefundsNotify>(headers, body, _optionsAccessor);
        }

        #endregion

        #region 微信支付分

        /// <summary>
        /// 开启/解除授权服务回调通知
        /// </summary>
        public async Task<WeChatPayScoreUserOpenOrCloseNotify> Permissions(WeChatPayHeaders headers, string body)
        {
            return await _client.ExecuteAsync<WeChatPayScoreUserOpenOrCloseNotify>(headers, body, _optionsAccessor);
        }

        /// <summary>
        /// 确认订单回调通知
        /// </summary> 
        public async Task<WeChatPayScoreUserConfirmNotify> OrderConfirm(WeChatPayHeaders headers, string body)
        {
            return await _client.ExecuteAsync<WeChatPayScoreUserConfirmNotify>(headers, body, _optionsAccessor); 
        }

        /// <summary>
        /// 订单支付成功回调通知
        /// </summary> 
        public async Task<WeChatPayScoreUserPaidNotify> OrderPaid(WeChatPayHeaders headers, string body)
        {
            return await _client.ExecuteAsync<WeChatPayScoreUserPaidNotify>(headers, body, _optionsAccessor); 
        }

        /// <summary>
        /// 订单确认 或 支付成功 回调通知
        /// </summary> 
        public async Task<WeChatPayScoreUserPaidNotify> OrderConfirmOrPaid(WeChatPayHeaders headers, string body)
        {
            return await _client.ExecuteAsync<WeChatPayScoreUserPaidNotify>(headers, body, _optionsAccessor); 
        }

        #endregion


    }
}
