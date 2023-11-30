using Essensoft.Paylink.Alipay;
using Essensoft.Paylink.Alipay.Notify;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Silver.Pay.Alipay
{
    public class AlipayNotifys
    {

        private readonly IAlipayNotifyClient _client;
        private readonly AlipayOptions _optionsAccessor;

        public AlipayNotifys(IAlipayNotifyClient client, AlipayOptions optionsAccessor)
        {
            _client = client;
            _optionsAccessor = optionsAccessor;
        }


        /// <summary>
        /// 应用网关
        /// </summary>
        /// <returns></returns>
        public async Task<AlipayNotify> Gateway(IDictionary<string, string> Request, string msg_method)
        {
            switch (msg_method)
            {
                // 资金单据状态变更通知
                case "alipay.fund.trans.order.changed":
                    {
                        return await _client.CertificateExecuteAsync<AlipayFundTransOrderChangedNotify>(Request, _optionsAccessor);
                    }
                // 第三方应用授权取消消息
                case "alipay.open.auth.appauth.cancelled":
                    {
                        return await _client.CertificateExecuteAsync<AlipayOpenAuthAppauthCancelledNotify>(Request, _optionsAccessor);
                    }
                // 用户授权取消消息
                case "alipay.open.auth.userauth.cancelled":
                    {
                        return await _client.CertificateExecuteAsync<AlipayOpenAuthUserauthCancelledNotify>(Request, _optionsAccessor);
                    }
                // 小程序审核通过通知
                case "alipay.open.mini.version.audit.passed":
                    {
                        return await _client.CertificateExecuteAsync<AlipayOpenMiniVersionAuditPassedNotify>(Request, _optionsAccessor);
                    }
                // 小程序审核驳回通知
                case "alipay.open.mini.version.audit.rejected":
                    {
                        return await _client.CertificateExecuteAsync<AlipayOpenMiniVersionAuditRejectedNotify>(Request, _optionsAccessor);
                    }
                // 收单退款冲退完成通知
                case "alipay.trade.refund.depositback.completed":
                    {
                        return await _client.CertificateExecuteAsync<AlipayTradeRefundDepositbackCompletedNotify>(Request, _optionsAccessor);
                    }
                // 收单资金结算到银行账户，结算退票的异步通知
                case "alipay.trade.settle.dishonoured":
                    {
                        return await _client.CertificateExecuteAsync<AlipayTradeSettleDishonouredNotify>(Request, _optionsAccessor);
                    }
                // 收单资金结算到银行账户，结算失败的异步通知
                case "alipay.trade.settle.fail":
                    {
                        return await _client.CertificateExecuteAsync<AlipayTradeSettleFailNotify>(Request, _optionsAccessor);
                    }
                // 收单资金结算到银行账户，结算成功的异步通知
                case "alipay.trade.settle.success":
                    {
                        return await _client.CertificateExecuteAsync<AlipayTradeSettleSuccessNotify>(Request, _optionsAccessor);
                    }
                // 身份认证记录消息
                case "alipay.user.certify.open.notify.completed":
                    {
                        return await _client.CertificateExecuteAsync<AlipayUserCertifyOpenNotifyCompletedNotify>(Request, _optionsAccessor);
                    }
                default:
                    return null;
            }
        }

        /// <summary>
        /// 扫码支付异步通知
        /// </summary>
        public async Task<AlipayTradePrecreateNotify> Precreate(IDictionary<string, string> Request)
        {
            return await _client.CertificateExecuteAsync<AlipayTradePrecreateNotify>(Request, _optionsAccessor);
        }

        /// <summary>
        /// APP支付异步通知
        /// </summary> 
        public async Task<AlipayTradeAppPayNotify> AppPay(IDictionary<string, string> Request)
        {
            return await _client.CertificateExecuteAsync<AlipayTradeAppPayNotify>(Request, _optionsAccessor);
        }

        /// <summary>
        /// 电脑网站支付异步通知
        /// </summary> 
        public async Task<AlipayTradePagePayNotify> PagePay(IDictionary<string, string> Request)
        {
            return await _client.CertificateExecuteAsync<AlipayTradePagePayNotify>(Request, _optionsAccessor);
        }

        /// <summary>
        /// 手机网站支付异步通知
        /// </summary> 
        public async Task<AlipayTradeWapPayNotify> WapPay(IDictionary<string, string> Request)
        {
            return await _client.CertificateExecuteAsync<AlipayTradeWapPayNotify>(Request, _optionsAccessor);
        }

        /// <summary>
        /// 交易关闭异步通知
        /// </summary> 
        public async Task<AlipayTradeCloseNotify> Close(IDictionary<string, string> Request)
        {
            return await _client.CertificateExecuteAsync<AlipayTradeCloseNotify>(Request, _optionsAccessor);
        }


    }
}
