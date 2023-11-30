using Essensoft.Paylink.Alipay;
using Essensoft.Paylink.Alipay.Notify;
using Essensoft.Paylink.WeChatPay;
using Essensoft.Paylink.WeChatPay.V3;
using Essensoft.Paylink.WeChatPay.V3.Notify;
using Silver.Basic;
using Silver.Pay.Alipay;
using Silver.Pay.Model;
using Silver.Pay.Model.Enum;
using Silver.Pay.Model.Request;
using Silver.Pay.Model.Response;
using Silver.Pay.Model.Response.Create;
using Silver.Pay.Model.Response.Query;
using Silver.Pay.Model.Response.Refund;
using Silver.Pay.WeChatPay;

namespace Silver.Pay.Payment
{
    public class Payment : IPayment
    {
        private AliPayment aliPayment;
        private WeChatPayment weChatPayment;
        private WeChatV2Payment weChatPaymentV2;
        private AlipayNotifys alipayNotifys;
        private WeChatNotify weChatNotify;

        public Payment(IAlipayNotifyClient notifyAlipayClient, IWeChatPayNotifyClient notifyWeChatClient, IAlipayClient aliPay, AlipayOptions aliPayOptions, IWeChatPayClient weiChat, Essensoft.Paylink.WeChatPay.V2.IWeChatPayClient weiChatV2, WeChatPayOptions weiChatOptions)
        {
            if (weiChat != null && weiChatOptions != null)
            {
                weChatPayment = new WeChatPayment(weiChat, weiChatOptions);
                weChatNotify = new WeChatNotify(notifyWeChatClient, weiChatOptions);
            }
            if (weiChatV2 != null && weiChatOptions != null)
            {
                weChatPaymentV2 = new WeChatV2Payment(weiChatV2, weiChatOptions);
            }
            if (aliPay != null && aliPayOptions != null)
            {
                aliPayment = new AliPayment(aliPay, aliPayOptions);
                alipayNotifys = new AlipayNotifys(notifyAlipayClient, aliPayOptions);
            }
        }

        /// <summary>
        /// 支付
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<PayCreateResponse> Pay(PayRequest request)
        {
            PayCreateResponse result = new PayCreateResponse();
            result.PayType = request.PayType;
            result.PayId = request.PayId;
            result.TotalFee = request.TotalAmount;
            result.OutTradeNo = request.OutTradeNo;
            if (request.PayId == PayIdEnum.AliPayPreCreate)
            {
                var requestPay = new AlipayTradePreCreateViewModel()
                {
                    OutTradeNo = request.OutTradeNo,
                    Subject = request.Subject,
                    TotalAmount = request.TotalAmount.ToString(),
                    Body = request.Body,
                    NotifyUrl = request.NotifyUrl
                };
                result = await aliPayment.PreCreate(requestPay);
            }
            else if (request.PayId == PayIdEnum.AliPayPay)
            {
                var requestPay = new AlipayTradePayViewModel()
                {
                    OutTradeNo = request.OutTradeNo,
                    Subject = request.Subject,
                    Scene = "bar_code",
                    AuthCode = request.AuthCode,
                    TotalAmount = request.TotalAmount.ToString(),
                    Body = request.Body
                };
                result = await aliPayment.Pay(requestPay);
            }
            else if (request.PayId == PayIdEnum.AliPayAPP)
            {
                var requestPay = new AlipayTradeAppPayViewModel()
                {
                    OutTradeNo = request.OutTradeNo,
                    Subject = request.Subject,
                    ProductCode = "QUICK_MSECURITY_PAY",
                    TotalAmount = request.TotalAmount.ToString(),
                    Body = request.Body,
                    NotifyUrl = request.NotifyUrl
                };
                result = await aliPayment.AppPay(requestPay);
            }
            else if (request.PayId == PayIdEnum.AliPayWeb)
            {
                var requestPay = new AlipayTradePagePayViewModel()
                {
                    Body = request.Body,
                    Subject = request.Subject,
                    TotalAmount = request.TotalAmount.ToString(),
                    OutTradeNo = request.OutTradeNo,
                    ProductCode = "FAST_INSTANT_TRADE_PAY",
                    NotifyUrl = request.NotifyUrl,
                    ReturnUrl = request.ReturnUrl
                };
                result = await aliPayment.PagePay(requestPay);
            }
            else if (request.PayId == PayIdEnum.AliPayWap)
            {
                var requestPay = new AlipayTradeWapPayViewModel()
                {
                    Body = request.Body,
                    Subject = request.Subject,
                    TotalAmount = request.TotalAmount.ToString(),
                    OutTradeNo = request.OutTradeNo,
                    ProductCode = "QUICK_WAP_WAY",
                    NotifyUrl = request.NotifyUrl,
                    ReturnUrl = request.ReturnUrl
                };
                result = await aliPayment.WapPay(requestPay);
            }
            else if (request.PayId == PayIdEnum.WeChatMicro)
            {
                var requestPay = new WeChatPayMicroPay()
                {
                    Body = request.Body,
                    OutTradeNo = request.OutTradeNo,
                    TotalFee = request.TotalAmount.ToString(),
                    SpBillCreateIp = request.IP,
                    AuthCode = request.AuthCode
                };
                result = await weChatPaymentV2.MicroPay(requestPay);
            }
            else if (request.PayId == PayIdEnum.WeChatAPP)
            {
                var requestPay = new WeChatPayAppPayV3ViewModel()
                {
                    Total = request.TotalAmount.ToString(),
                    Currency = "CNY",
                    Description = request.Body,
                    NotifyUrl = request.NotifyUrl,
                    OutTradeNo = request.OutTradeNo,
                };
                result = await weChatPayment.AppPay(requestPay);
            }
            else if (request.PayId == PayIdEnum.WeChatPub)
            {
                var requestPay = new WeChatPayPubPayV3ViewModel()
                {
                    Total = request.TotalAmount.ToString(),
                    Currency = "CNY",
                    Description = request.Body,
                    NotifyUrl = request.NotifyUrl,
                    OutTradeNo = request.OutTradeNo,
                    OpenId = request.OpenId
                };
                result = await weChatPayment.JSAPIPay(requestPay);
            }
            else if (request.PayId == PayIdEnum.WeChatQrCode)
            {
                var requestPay = new WeChatPayQrCodePayV3ViewModel()
                {
                    Total = request.TotalAmount.ToString(),
                    Currency = "CNY",
                    Description = request.Body,
                    NotifyUrl = request.NotifyUrl,
                    OutTradeNo = request.OutTradeNo,
                };
                result = await weChatPayment.QrCodePay(requestPay);
            }
            else if (request.PayId == PayIdEnum.WeChatH5)
            {
                var requestPay = new WeChatPayH5PayV3ViewModel()
                {
                    Total = request.TotalAmount.ToString(),
                    Currency = "CNY",
                    Description = request.Body,
                    NotifyUrl = request.NotifyUrl,
                    OutTradeNo = request.OutTradeNo,
                    IP = request.IP
                };
                result = await weChatPayment.H5Pay(requestPay);
            }
            else if (request.PayId == PayIdEnum.WeChatMiniProgram)
            {
                var requestPay = new WeChatPayPubPayV3ViewModel()
                {
                    Total = request.TotalAmount.ToString(),
                    Currency = "CNY",
                    Description = request.Body,
                    NotifyUrl = request.NotifyUrl,
                    OutTradeNo = request.OutTradeNo,
                    OpenId = request.OpenId
                };
                result = await weChatPayment.MiniProgramPay(requestPay);
            }
            return result;
        }

        /// <summary>
        /// 订单查询
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<PayQueryResult> Query(QueryRequest request)
        {
            PayResult result = new PayResult();
            PayQueryResult payQueryResult = new PayQueryResult();
            PayQueryResponse payQueryResponse = new PayQueryResponse();
            if (request.PayType== PayTypeEnum.WeChat)
            {
                if (request.OutTradeNo.IsNotEmpty())
                {
                    result = await weChatPayment.QueryByOutTradeNo(new WeChatPayQueryByOutTradeNoViewModel() { OutTradeNo = request.OutTradeNo });
                }
                else if (request.TransactionId.IsNotEmpty())
                {
                    result = await weChatPayment.QueryByTransactionId(new WeChatPayQueryByTransactionIdViewModel() { TransactionId = request.TransactionId });
                } 
                if (result.IsSuccess)
                {
                    WechatQueryResponse payQuery =  StringUtil.JsonToObject<WechatQueryResponse>(result.Result);
                    payQueryResponse.Amount = Convert.ToDecimal(payQuery.amount.payer_total / 100);
                    payQueryResponse.Appid = payQuery.appid;
                    payQueryResponse.Attach = payQuery.attach;
                    payQueryResponse.Mchid = payQuery.mchid;
                    payQueryResponse.outTradeNo = payQuery.out_trade_no;
                    payQueryResponse.Openid = payQuery.payer.openid;
                    payQueryResponse.SuccessTime = payQuery.success_time;
                    payQueryResponse.TradeState = payQuery.trade_state;
                    payQueryResponse.TradeType = payQuery.trade_type;
                    payQueryResponse.TransactionId = payQuery.transaction_id;
                    payQueryResponse.Payment = "wechat";
                }
                payQueryResponse.Wechat = result.Result;
            }
            else if (request.PayType == PayTypeEnum.Alipay)
            {
                result = await aliPayment.Query(new AlipayTradeQueryViewModel() { OutTradeNo = request.OutTradeNo, TradeNo = request.TransactionId });
                if (result.IsSuccess)
                {
                    AliPayQueryResponse payQuery = StringUtil.JsonToObject<AliPayQueryResponse>(result.Result);
                    payQueryResponse.Amount = Convert.ToDecimal(payQuery.alipay_trade_query_response.total_amount);
                    payQueryResponse.Appid = payQuery.alipay_trade_query_response.buyer_user_id;
                    payQueryResponse.Attach = payQuery.alipay_trade_query_response.ext_infos;
                    payQueryResponse.Mchid = payQuery.alipay_trade_query_response.store_name;
                    payQueryResponse.outTradeNo = payQuery.alipay_trade_query_response.out_trade_no;
                    payQueryResponse.Openid = payQuery.alipay_trade_query_response.buyer_logon_id;
                    payQueryResponse.SuccessTime = payQuery.alipay_trade_query_response.send_pay_date;
                    if (payQuery.alipay_trade_query_response.trade_status.ToUpper() == "WAIT_BUYER_PAY")
                    {
                        payQueryResponse.TradeState = "NOTPAY";
                    }
                    else if (payQuery.alipay_trade_query_response.trade_status.ToUpper() == "TRADE_CLOSED")
                    {
                        payQueryResponse.TradeState = "CLOSED";
                    }
                    else if (payQuery.alipay_trade_query_response.trade_status.ToUpper() == "TRADE_SUCCESS")
                    {
                        payQueryResponse.TradeState = "SUCCESS";
                    }
                    else if (payQuery.alipay_trade_query_response.trade_status.ToUpper() == "TRADE_FINISHED")
                    {
                        payQueryResponse.TradeState = "SUCCESS";
                    }
                    payQueryResponse.TradeType = "";
                    payQueryResponse.TransactionId = payQuery.alipay_trade_query_response.trade_no;
                    payQueryResponse.Payment = "alipay";
                }
                payQueryResponse.AliPay = result.Result;
            }
            payQueryResult.StatusCode = result.StatusCode;
            payQueryResult.IsSuccess = result.IsSuccess;
            payQueryResult.Message = result.Message;
            payQueryResult.Result = payQueryResponse;
            return payQueryResult;
        }

        /// <summary>
        /// 退款
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<PayRefundResponse> Refund(RefundRequest request)
        {
            PayRefundResponse refundResponse = new PayRefundResponse();
            PayResult result = new PayResult();
            if (request.PayType == PayTypeEnum.WeChat)
            {
                WeChatPayV3RefundViewModel viewModel = new WeChatPayV3RefundViewModel();
                viewModel.OutRefundNo = request.OutRefundNo;
                viewModel.OutTradeNo = request.OutTradeNo;
                viewModel.TransactionId = request.TransactionId;
                viewModel.NotifyUrl = request.NotifyUrl;
                viewModel.Reason = request.Reason;
                viewModel.RefundAmount = Convert.ToInt32(request.RefundAmount * 100);
                viewModel.TotalAmount = Convert.ToInt32(request.TotalAmount * 100);
                viewModel.Currency = request.Currency;
                result = await weChatPayment.Refund(viewModel);
                if (result.IsSuccess)
                {
                    WechatRefundResponse resultRefund = result.Result.JsonToObject<WechatRefundResponse>();
                    refundResponse.Result.RefundId = resultRefund.refund_id;
                    refundResponse.Result.OutRefundNo = resultRefund.out_refund_no;
                    refundResponse.Result.TransactionId = resultRefund.transaction_id;
                    refundResponse.Result.OutTradeNo = resultRefund.out_trade_no;
                    refundResponse.Result.UserReceivedAccount = resultRefund.user_received_account;
                    refundResponse.Result.Status = resultRefund.status;
                    refundResponse.Result.Total = Convert.ToDecimal(resultRefund.amount.total / Convert.ToDecimal(100));
                    refundResponse.Result.Refund = Convert.ToDecimal(resultRefund.amount.refund / Convert.ToDecimal(100));
                    refundResponse.Result.PayerTotal = Convert.ToDecimal(resultRefund.amount.payer_total / Convert.ToDecimal(100));
                    refundResponse.Result.PayerRefund = Convert.ToDecimal(resultRefund.amount.refund/ Convert.ToDecimal(100));
                }
                refundResponse.Result.Wechat = result.Result;
            }
            else if (request.PayType == PayTypeEnum.Alipay)
            {
                AlipayTradeRefundViewModel viewMode = new AlipayTradeRefundViewModel();
                viewMode.OutTradeNo = request.OutTradeNo;
                viewMode.TradeNo = request.TransactionId;
                viewMode.RefundAmount = request.RefundAmount.ToString();
                viewMode.OutRequestNo = request.OutRefundNo;
                viewMode.RefundReason = request.Reason;
                result = await aliPayment.Refund(viewMode);
                if (result.IsSuccess)
                {
                    AlipayRefundResponse resultRefund = result.Result.JsonToObject<AlipayRefundResponse>();
                    refundResponse.Result.RefundId = "";
                    refundResponse.Result.OutRefundNo = "";
                    refundResponse.Result.TransactionId = resultRefund.alipay_trade_refund_response.trade_no;
                    refundResponse.Result.OutTradeNo = resultRefund.alipay_trade_refund_response.out_trade_no;
                    refundResponse.Result.UserReceivedAccount = resultRefund.alipay_trade_refund_response.buyer_user_id;
                    refundResponse.Result.Status = "PROCESSING";
                    refundResponse.Result.Total = 0;
                    refundResponse.Result.Refund = Convert.ToDecimal(resultRefund.alipay_trade_refund_response.refund_fee);
                    refundResponse.Result.PayerTotal = 0;
                    refundResponse.Result.PayerRefund = Convert.ToDecimal(resultRefund.alipay_trade_refund_response.refund_fee);
                }
                refundResponse.Result.AliPay = result.Result;
            }
            refundResponse.StatusCode = result.StatusCode;
            refundResponse.IsSuccess = result.IsSuccess;
            refundResponse.Message = result.Message;
            return refundResponse;
        }
         
        /// <summary>
        /// 退款查询
        /// </summary>
        /// <returns></returns>
        public async Task<PayRefundResponse> RefundQuery(RefundQueryRequest request)
        {
            PayRefundResponse refundResponse = new PayRefundResponse();
            PayResult result = new PayResult();
            if (request.Payment == PayTypeEnum.WeChat)
            {
                result = await weChatPayment.RefundQuery(new WeChatPayV3RefundQueryViewModel() { OutRefundNo = request.OutRefundNo });
                if (result.IsSuccess)
                {
                    WechatRefundResponse resultRefund = result.Result.JsonToObject<WechatRefundResponse>();
                    refundResponse.Result.RefundId = resultRefund.refund_id;
                    refundResponse.Result.OutRefundNo = resultRefund.out_refund_no;
                    refundResponse.Result.TransactionId = resultRefund.transaction_id;
                    refundResponse.Result.OutTradeNo = resultRefund.out_trade_no;
                    refundResponse.Result.UserReceivedAccount = resultRefund.user_received_account;
                    refundResponse.Result.Status = resultRefund.status;
                    refundResponse.Result.Total = Convert.ToDecimal(resultRefund.amount.total / Convert.ToDecimal(100));
                    refundResponse.Result.Refund = Convert.ToDecimal(resultRefund.amount.refund / Convert.ToDecimal(100));
                    refundResponse.Result.PayerTotal = Convert.ToDecimal(resultRefund.amount.payer_total / Convert.ToDecimal(100));
                    refundResponse.Result.PayerRefund = Convert.ToDecimal(resultRefund.amount.refund / Convert.ToDecimal(100));
                }
                refundResponse.Result.Wechat = result.Result;
            }
            else if (request.Payment == PayTypeEnum.Alipay)
            {
                result = await aliPayment.RefundQuery(new AlipayTradeRefundQueryViewModel() { OutRequestNo = request.OutRefundNo, OutTradeNo = request.OutTradeNo, TradeNo = request.TransactionId });
                if (result.IsSuccess)
                {
                    AlipayRefundQueryResponse resultRefund = result.Result.JsonToObject<AlipayRefundQueryResponse>();
                    refundResponse.Result.RefundId = "";
                    refundResponse.Result.OutRefundNo = resultRefund.alipay_trade_fastpay_refund_query_response.out_request_no;
                    refundResponse.Result.TransactionId = resultRefund.alipay_trade_fastpay_refund_query_response.trade_no;
                    refundResponse.Result.OutTradeNo = resultRefund.alipay_trade_fastpay_refund_query_response.out_trade_no;
                    refundResponse.Result.UserReceivedAccount ="";
                    refundResponse.Result.Status = "PROCESSING";
                    if (resultRefund.alipay_trade_fastpay_refund_query_response.refund_status == "REFUND_SUCCESS")
                    {
                        refundResponse.Result.Status = "SUCCESS";
                    }
                    refundResponse.Result.Total = 0;
                    refundResponse.Result.Refund = Convert.ToDecimal(resultRefund.alipay_trade_fastpay_refund_query_response.refund_amount);
                    refundResponse.Result.PayerTotal = Convert.ToDecimal(resultRefund.alipay_trade_fastpay_refund_query_response.total_amount);
                    refundResponse.Result.PayerRefund = Convert.ToDecimal(resultRefund.alipay_trade_fastpay_refund_query_response.refund_amount);
                }
                refundResponse.Result.AliPay = result.Result;
            }
            refundResponse.StatusCode = result.StatusCode;
            refundResponse.IsSuccess = result.IsSuccess;
            refundResponse.Message = result.Message;
            return refundResponse;
        }

        /// <summary>
        /// 关闭订单
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<PayResult> Close(CloseRequest request)
        {
            PayResult result = new PayResult();
            if (request.Payment == PayTypeEnum.WeChat)
            {
                result = await weChatPayment.OutTradeNoClose(new WeChatPayOutTradeNoCloseViewModel() { OutTradeNo = request.OutTradeNo });
            }
            else if (request.Payment == PayTypeEnum.Alipay)
            {
                result = await aliPayment.Close(new AlipayTradeCloseViewModel() { NotifyUrl = request.NotifyUrl, OutTradeNo = request.OutTradeNo, TradeNo = request.TransactionId });
            }
            return result;
        }

        /// <summary>
        /// 扫码支付异步通知
        /// </summary>
        /// <param name="Request"></param>
        /// <returns></returns>
        public async Task<AlipayTradePrecreateNotify> AliPayPrecreate(IDictionary<string, string> Request)
        {
            return await alipayNotifys.Precreate(Request);
        }

        /// <summary>
        /// APP支付异步通知
        /// </summary>
        /// <param name="Request"></param>
        /// <returns></returns>
        public async Task<AlipayTradeAppPayNotify> AliPayAppPay(IDictionary<string, string> Request)
        {
            return await alipayNotifys.AppPay(Request);
        }

        /// <summary>
        /// 电脑网站支付异步通知
        /// </summary>
        /// <param name="Request"></param>
        /// <returns></returns>
        public async Task<AlipayTradePagePayNotify> AliPayPagePay(IDictionary<string, string> Request)
        {
            return await alipayNotifys.PagePay(Request);
        }

        /// <summary>
        /// 手机网站支付异步通知
        /// </summary>
        /// <param name="Request"></param>
        /// <returns></returns>
        public async Task<AlipayTradeWapPayNotify> AliPayWapPay(IDictionary<string, string> Request)
        {
            return await alipayNotifys.WapPay(Request);
        }

        /// <summary>
        /// 交易关闭异步通知
        /// </summary>
        /// <param name="Request"></param>
        /// <returns></returns>
        public async Task<AlipayTradeCloseNotify> AliPayClose(IDictionary<string, string> Request)
        {
            return await alipayNotifys.Close(Request);
        }

        /// <summary>
        /// 支付结果通知
        /// </summary>
        /// <param name="headers"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        public async Task<WeChatPayTransactionsNotify> WeChatTransactions(WeChatPayHeaders headers, string body)
        {
            return await weChatNotify.Transactions(headers,body);
        }

        /// <summary>
        /// 退款结果通知
        /// </summary>
        /// <param name="headers"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        public async Task<WeChatPayRefundDomesticRefundsNotify> WeChatRefund(WeChatPayHeaders headers, string body)
        {
            return await weChatNotify.Refund(headers, body);
        }


    }
}
