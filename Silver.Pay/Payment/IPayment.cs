using Essensoft.Paylink.Alipay.Notify;
using Essensoft.Paylink.WeChatPay.V3.Notify;
using Essensoft.Paylink.WeChatPay.V3;
using Silver.Pay.Model;
using Silver.Pay.Model.Request;
using Silver.Pay.Model.Response;
using Silver.Pay.Model.Response.Create;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Silver.Pay.Payment
{
    public interface IPayment
    {
        /// <summary>
        /// 支付
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<PayCreateResponse> Pay(PayRequest request);

        /// <summary>
        /// 订单详情
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<PayQueryResult> Query(QueryRequest request);

        /// <summary>
        /// 退款
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<PayRefundResponse> Refund(RefundRequest request);

        /// <summary>
        /// 退款查询
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<PayRefundResponse> RefundQuery(RefundQueryRequest request);

        /// <summary>
        /// 取消订单
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<PayResult> Close(CloseRequest request);

        /// <summary>
        /// 扫码支付异步通知
        /// </summary>
        /// <param name="Request"></param>
        /// <returns></returns>
        Task<AlipayTradePrecreateNotify> AliPayPrecreate(IDictionary<string, string> Request);

        /// <summary>
        /// APP支付异步通知
        /// </summary>
        /// <param name="Request"></param>
        /// <returns></returns>
        Task<AlipayTradeAppPayNotify> AliPayAppPay(IDictionary<string, string> Request);

        /// <summary>
        /// 电脑网站支付异步通知
        /// </summary>
        /// <param name="Request"></param>
        /// <returns></returns>
        Task<AlipayTradePagePayNotify> AliPayPagePay(IDictionary<string, string> Request);

        /// <summary>
        /// 手机网站支付异步通知
        /// </summary>
        /// <param name="Request"></param>
        /// <returns></returns>
        Task<AlipayTradeWapPayNotify> AliPayWapPay(IDictionary<string, string> Request);

        /// <summary>
        /// 交易关闭异步通知
        /// </summary>
        /// <param name="Request"></param>
        /// <returns></returns>
        Task<AlipayTradeCloseNotify> AliPayClose(IDictionary<string, string> Request);

        /// <summary>
        /// 支付结果通知
        /// </summary>
        /// <param name="headers"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        Task<WeChatPayTransactionsNotify> WeChatTransactions(WeChatPayHeaders headers, string body);

        /// <summary>
        /// 退款结果通知
        /// </summary>
        /// <param name="headers"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        Task<WeChatPayRefundDomesticRefundsNotify> WeChatRefund(WeChatPayHeaders headers, string body);

    }
}
