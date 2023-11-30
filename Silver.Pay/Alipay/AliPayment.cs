using Essensoft.Paylink.Alipay;
using Essensoft.Paylink.Alipay.Domain;
using Essensoft.Paylink.Alipay.Request;
using Silver.Basic;
using Silver.Pay.Model;
using Silver.Pay.Model.Request;
using Silver.Pay.Model.Response.Create;

namespace Silver.Pay.Alipay
{
    public class AliPayment
    {
        private IAlipayClient _client;
        private AlipayOptions _optionsAccessor;
        public AliPayment(IAlipayClient client, AlipayOptions optionsAccessor)
        {
            _client = client;
            _optionsAccessor = optionsAccessor;
        }
         
        /// <summary>
        /// 当面付-扫码支付
        /// </summary> 
        public async Task<PayCreateResponse> PreCreate(AlipayTradePreCreateViewModel viewModel)
        {
            PayCreateResponse result = new PayCreateResponse();
            var model = new AlipayTradePrecreateModel
            {
                OutTradeNo = viewModel.OutTradeNo,
                Subject = viewModel.Subject,
                TotalAmount = viewModel.TotalAmount,
                Body = viewModel.Body, 
            };
            var req = new AlipayTradePrecreateRequest();
            req.SetBizModel(model);
            req.SetNotifyUrl(viewModel.NotifyUrl);
            var response = await _client.CertificateExecuteAsync(req, _optionsAccessor);
            result.IsSuccess = !response.IsError;
            result.Message = response.Msg;
            result.Result = response.Body;
            if (!response.IsError)
            { 
                result.Qrcode = response.QrCode;
                result.TotalFee = Convert.ToDecimal(viewModel.TotalAmount); 
                result.OutTradeNo = viewModel.OutTradeNo;
            }
            return result; 
        }

        /// <summary>
        /// 当面付-二维码/条码/声波支付
        /// </summary>
        public async Task<PayCreateResponse> Pay(AlipayTradePayViewModel viewModel)
        {
            PayCreateResponse result = new PayCreateResponse();
            var model = new AlipayTradePayModel
            {
                OutTradeNo = viewModel.OutTradeNo,
                Subject = viewModel.Subject,
                Scene = viewModel.Scene,
                AuthCode = viewModel.AuthCode,
                TotalAmount = viewModel.TotalAmount,
                Body = viewModel.Body
            };
            var req = new AlipayTradePayRequest();
            req.SetBizModel(model);
            var response = await _client.CertificateExecuteAsync(req, _optionsAccessor);
            result.IsSuccess = !response.IsError;
            result.Message = response.Msg;
            result.Result = response.Body;
            var resultTradePay = response.Body.JsonToObject<AlipayTradePayResponse>();
            if (resultTradePay.alipay_trade_pay_response.code == "10000")
            {
                result.OpenId = resultTradePay.alipay_trade_pay_response.buyer_user_id;
                result.OpenName = resultTradePay.alipay_trade_pay_response.buyer_logon_id;
                result.TotalFee = Convert.ToDecimal(resultTradePay.alipay_trade_pay_response.total_amount);
                result.TransactionId = resultTradePay.alipay_trade_pay_response.trade_no;
                result.OutTradeNo = viewModel.OutTradeNo;
            }
            else
            {
                result.IsSuccess = false;
                result.Message = resultTradePay.alipay_trade_pay_response.sub_msg;
            }
            return result; 
        }

        /// <summary>
        /// APP支付
        /// </summary>
        public async Task<PayCreateResponse> AppPay(AlipayTradeAppPayViewModel viewModel)
        {
            PayCreateResponse result = new PayCreateResponse();
            var model = new AlipayTradeAppPayModel
            {
                OutTradeNo = viewModel.OutTradeNo,
                Subject = viewModel.Subject,
                ProductCode = viewModel.ProductCode,
                TotalAmount = viewModel.TotalAmount,
                Body = viewModel.Body
            };
            var req = new AlipayTradeAppPayRequest();
            req.SetBizModel(model);
            req.SetNotifyUrl(viewModel.NotifyUrl);
            var response = await _client.SdkExecuteAsync(req, _optionsAccessor);
            result.IsSuccess = !response.IsError;
            result.Message = response.Msg;
            result.Result = response.Body;
            if (!response.IsError)
            { 
                result.TotalFee = Convert.ToDecimal(viewModel.TotalAmount);
                result.OutTradeNo = viewModel.OutTradeNo;
            }
            return result;
        }

        /// <summary>
        /// 电脑网站支付
        /// </summary>
        /// <param name="viewModel"></param>
        public async Task<PayCreateResponse> PagePay(AlipayTradePagePayViewModel viewModel)
        {
            PayCreateResponse result = new PayCreateResponse();
            var model = new AlipayTradePagePayModel
            {
                Body = viewModel.Body,
                Subject = viewModel.Subject,
                TotalAmount = viewModel.TotalAmount,
                OutTradeNo = viewModel.OutTradeNo,
                ProductCode = viewModel.ProductCode
            };
            var req = new AlipayTradePagePayRequest();
            req.SetBizModel(model);
            req.SetNotifyUrl(viewModel.NotifyUrl);
            req.SetReturnUrl(viewModel.ReturnUrl); 
            var response = await _client.PageExecuteAsync(req, _optionsAccessor);
            result.IsSuccess = !response.IsError;
            result.Message = response.Msg;
            result.Result = response.Body;
            if (!response.IsError)
            {
                result.TotalFee = Convert.ToDecimal(viewModel.TotalAmount);
                result.OutTradeNo = viewModel.OutTradeNo;
            }
            return result;
        }

        /// <summary>
        /// 手机网站支付
        /// </summary>
        public async Task<PayCreateResponse> WapPay(AlipayTradeWapPayViewModel viewMode)
        {
            PayCreateResponse result = new PayCreateResponse();
            var model = new AlipayTradeWapPayModel
            {
                Body = viewMode.Body,
                Subject = viewMode.Subject,
                TotalAmount = viewMode.TotalAmount,
                OutTradeNo = viewMode.OutTradeNo,
                ProductCode = viewMode.ProductCode
            };
            var req = new AlipayTradeWapPayRequest();
            req.SetBizModel(model);
            req.SetNotifyUrl(viewMode.NotifyUrl);
            req.SetReturnUrl(viewMode.ReturnUrl);
            var response = await _client.PageExecuteAsync(req, _optionsAccessor);
            result.IsSuccess = !response.IsError;
            result.Message = response.Msg;
            result.Result = response.Body;
            if (!response.IsError)
            {
                result.TotalFee = Convert.ToDecimal(viewMode.TotalAmount);
                result.OutTradeNo = viewMode.OutTradeNo;
            }
            return result;
        }

        /// <summary>
        /// 交易查询
        /// </summary>
        public async Task<PayResult> Query(AlipayTradeQueryViewModel viewMode)
        {
            PayResult result = new PayResult();
            var model = new AlipayTradeQueryModel
            {
                OutTradeNo = viewMode.OutTradeNo,
                TradeNo = viewMode.TradeNo
            };
            var req = new AlipayTradeQueryRequest();
            req.SetBizModel(model);
            var response = await _client.CertificateExecuteAsync(req, _optionsAccessor); 
            result.IsSuccess = !response.IsError;
            result.Message = response.Msg;
            result.Result = (((AlipayResponse)response).Body);
            return result;
        }

        /// <summary>
        /// 交易退款
        /// </summary>
        public async Task<PayResult> Refund(AlipayTradeRefundViewModel viewMode)
        {
            PayResult result = new PayResult();
            var model = new AlipayTradeRefundModel
            {
                OutTradeNo = viewMode.OutTradeNo,
                TradeNo = viewMode.TradeNo,
                RefundAmount = viewMode.RefundAmount,
                OutRequestNo = viewMode.OutRequestNo,
                RefundReason = viewMode.RefundReason
            };
            var req = new AlipayTradeRefundRequest();
            req.SetBizModel(model);
            var response = await _client.CertificateExecuteAsync(req, _optionsAccessor);
            result.IsSuccess = !response.IsError;
            result.Message = response.Msg;
            result.Result = response.Body;
            return result;
        }

        /// <summary>
        /// 退款查询
        /// </summary>
        public async Task<PayResult> RefundQuery(AlipayTradeRefundQueryViewModel viewMode)
        {
            PayResult result = new PayResult();
            var model = new AlipayTradeFastpayRefundQueryModel
            {
                OutTradeNo = viewMode.OutTradeNo,
                TradeNo = viewMode.TradeNo,
                OutRequestNo = viewMode.OutRequestNo
            };
            var req = new AlipayTradeFastpayRefundQueryRequest();
            req.SetBizModel(model);
            var response = await _client.CertificateExecuteAsync(req, _optionsAccessor);
            result.IsSuccess = !response.IsError;
            result.Message = response.Msg;
            result.Result = response.Body;
            return result;
        }

        /// <summary>
        /// 交易关闭
        /// </summary>
        public async Task<PayResult> Close(AlipayTradeCloseViewModel viewMode)
        {
            PayResult result = new PayResult();
            var model = new AlipayTradeCloseModel
            {
                OutTradeNo = viewMode.OutTradeNo,
                TradeNo = viewMode.TradeNo,
            };
            var req = new AlipayTradeCloseRequest();
            req.SetBizModel(model);
            req.SetNotifyUrl(viewMode.NotifyUrl);
            var response = await _client.CertificateExecuteAsync(req, _optionsAccessor);
            result.IsSuccess = !response.IsError;
            result.Message = response.Msg;
            result.Result = response.Body;
            return result;
        }

        /// <summary>
        /// 统一转账
        /// </summary>
        public async Task<PayResult> Transfer(AlipayTransferViewModel viewMode)
        {
            PayResult result = new PayResult();
            var model = new AlipayFundTransUniTransferModel
            {
                OutBizNo = viewMode.OutBizNo,
                TransAmount = viewMode.TransAmount,
                ProductCode = viewMode.ProductCode,
                BizScene = viewMode.BizScene,
                PayeeInfo = new Participant { Identity = viewMode.PayeeIdentity, IdentityType = viewMode.PayeeIdentityType, Name = viewMode.PayeeName },
                Remark = viewMode.Remark
            };
            var req = new AlipayFundTransUniTransferRequest();
            req.SetBizModel(model);
            var response = await _client.CertificateExecuteAsync(req, _optionsAccessor);
            result.IsSuccess = !response.IsError;
            result.Message = response.Msg;
            result.Result = response.Body;
            return result;
        }

        /// <summary>
        /// 查询统一转账订单
        /// </summary>
        public async Task<PayResult> TransQuery(AlipayTransQueryViewModel viewMode)
        {
            PayResult result = new PayResult();
            var model = new AlipayFundTransCommonQueryModel
            {
                OutBizNo = viewMode.OutBizNo,
                OrderId = viewMode.OrderId
            };
            var req = new AlipayFundTransCommonQueryRequest();
            req.SetBizModel(model);
            var response = await _client.CertificateExecuteAsync(req, _optionsAccessor);
            result.IsSuccess = !response.IsError;
            result.Message = response.Msg;
            result.Result = response.Body;
            return result;
        }

        /// <summary>
        /// 余额查询
        /// </summary>
        public async Task<PayResult> AccountQuery(AlipayAccountQueryViewModel viewModel)
        {
            PayResult result = new PayResult();
            var model = new AlipayFundAccountQueryModel
            {
                AlipayUserId = viewModel.AlipayUserId,
                AccountType = viewModel.AccountType
            };
            var req = new AlipayFundAccountQueryRequest();
            req.SetBizModel(model);
            var response = await _client.CertificateExecuteAsync(req, _optionsAccessor);
            result.IsSuccess = !response.IsError;
            result.Message = response.Msg;
            result.Result = response.Body;
            return result;
        }



    }
}
