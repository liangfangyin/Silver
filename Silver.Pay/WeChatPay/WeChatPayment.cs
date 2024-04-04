using Essensoft.Paylink.Security;
using Essensoft.Paylink.WeChatPay;
using Essensoft.Paylink.WeChatPay.V3;
using Essensoft.Paylink.WeChatPay.V3.Domain;
using Essensoft.Paylink.WeChatPay.V3.Request;
using Silver.Basic;
using Silver.Pay.Extensions;
using Silver.Pay.Model;
using Silver.Pay.Model.Request;
using Silver.Pay.Model.Response.Create;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Silver.Pay.WeChatPay
{
    public class WeChatPayment
    {
        private readonly IWeChatPayClient _client;
        private readonly WeChatPayOptions _optionsAccessor;
        public WeChatPayment(IWeChatPayClient client, WeChatPayOptions optionsAccessor)
        {
            _client = client;
            _optionsAccessor = optionsAccessor;
        }

        #region  微信支付
         
        /// <summary>
        /// APP支付-App下单API
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        public async Task<PayCreateResponse> AppPay(WeChatPayAppPayV3ViewModel viewModel)
        {
            PayCreateResponse result = new PayCreateResponse();
            var model = new WeChatPayTransactionsAppBodyModel
            {
                AppId = _optionsAccessor.AppId,
                MchId = _optionsAccessor.MchId,
                Amount = new Amount { Total = Convert.ToInt32(viewModel.Total.ToDecimal()*100), Currency = viewModel.Currency },
                Description = viewModel.Description,
                NotifyUrl = viewModel.NotifyUrl,
                OutTradeNo = viewModel.OutTradeNo,
            };
            if (viewModel.GoodsDetail.Count > 0)
            {
                model.Detail = new Detail()
                {
                    CostPrice = Convert.ToInt32(viewModel.Total.ToDecimal() * 100),
                    GoodsDetail = viewModel.GoodsDetail
                };
            }
            var request = new WeChatPayTransactionsAppRequest();
            request.SetBodyModel(model);
            var response = await _client.ExecuteAsync(request, _optionsAccessor);
            result.IsSuccess = !response.IsError;
            result.StatusCode = ((int)response.StatusCode);
            result.Message = response.Message;
            result.Result = response.Body;
            if (!response.IsError)
            {
                var req = new WeChatPayAppSdkRequest
                {
                    PrepayId = response.PrepayId
                };
                var parameter = await _client.ExecuteAsync(req, _optionsAccessor); 
                result.OpenId = "";
                result.TotalFee = viewModel.Total.ToDecimal();
                result.TransactionId = "";
                result.OutTradeNo = viewModel.OutTradeNo;
                result.PrepayId = response.PrepayId;
            }
            return result;
        }

        /// <summary>
        /// 公众号支付-JSAPI下单
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        public async Task<PayCreateResponse> JSAPIPay(WeChatPayPubPayV3ViewModel viewModel)
        {
            PayCreateResponse result = new PayCreateResponse();
            var model = new WeChatPayTransactionsJsApiBodyModel
            {
                AppId = _optionsAccessor.AppId,
                MchId = _optionsAccessor.MchId,
                Amount = new Amount { Total = Convert.ToInt32(viewModel.Total.ToDecimal() * 100), Currency = viewModel.Currency},
                Description = viewModel.Description,
                NotifyUrl = viewModel.NotifyUrl,
                OutTradeNo = viewModel.OutTradeNo,
                Payer = new PayerInfo { OpenId = viewModel.OpenId }, 
            };
            if (viewModel.GoodsDetail.Count > 0)
            {
                model.Detail = new Detail()
                {
                    CostPrice = Convert.ToInt32(viewModel.Total.ToDecimal() * 100),
                    GoodsDetail = viewModel.GoodsDetail
                };
            }
            var request = new WeChatPayTransactionsJsApiRequest();
            request.SetBodyModel(model); 
            var response = await _client.ExecuteAsync(request, _optionsAccessor);
            result.IsSuccess = !response.IsError;
            result.StatusCode = ((int)response.StatusCode);
            result.Message = response.Message;
            result.Result = response.Body;
            if (!response.IsError)
            {
                WeChatPayDictionary sortedTxtParams = new WeChatPayDictionary();
                if (!string.IsNullOrEmpty(_optionsAccessor.SubAppId))
                {
                    sortedTxtParams.Add("appId", _optionsAccessor.SubAppId);
                }
                else
                {
                    sortedTxtParams.Add("appId", _optionsAccessor.AppId);
                }
                sortedTxtParams.Add("timeStamp", WeChatPayUtility.GetTimeStamp());
                sortedTxtParams.Add("nonceStr", WeChatPayUtility.GenerateNonceStr());
                sortedTxtParams.Add("package", "prepay_id=" + response.PrepayId);
                sortedTxtParams.Add("signType", "RSA");
                string paySign = "";
                string data = BuildSignatureSourceData(sortedTxtParams);
                var Certificate2 = new X509Certificate2(_optionsAccessor.Certificate, _optionsAccessor.CertificatePassword, X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.Exportable | X509KeyStorageFlags.PersistKeySet);
                using (RSA RSAPrivateKey = Certificate2.GetRSAPrivateKey())
                {
                    paySign = SHA256WithRSA.Sign(RSAPrivateKey, data);
                }
                sortedTxtParams.Add("paySign", paySign);
                result.Result = (new { package = sortedTxtParams.GetValue("package"), appId = sortedTxtParams.GetValue("appId"), timeStamp = sortedTxtParams.GetValue("timeStamp"), nonceStr = sortedTxtParams.GetValue("nonceStr"), signType = sortedTxtParams.GetValue("signType"), paySign = sortedTxtParams.GetValue("paySign") }).ToJson();
            }
            return result;
        }

        /// <summary>
        /// 扫码支付-Native下单API
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        public async Task<PayCreateResponse> QrCodePay(WeChatPayQrCodePayV3ViewModel viewModel)
        {
            PayCreateResponse result = new PayCreateResponse();
            var model = new WeChatPayTransactionsNativeBodyModel
            {
                AppId = _optionsAccessor.AppId,
                MchId = _optionsAccessor.MchId,
                Amount = new Amount { Total = Convert.ToInt32(viewModel.Total.ToDecimal() * 100), Currency = viewModel.Currency },
                Description = viewModel.Description,
                NotifyUrl = viewModel.NotifyUrl,
                OutTradeNo = viewModel.OutTradeNo, 
            };
            if (viewModel.GoodsDetail.Count > 0)
            {
                model.Detail = new Detail()
                {
                    CostPrice = Convert.ToInt32(viewModel.Total.ToDecimal() * 100),
                    GoodsDetail = viewModel.GoodsDetail
                };
            }
            var request = new WeChatPayTransactionsNativeRequest();
            request.SetBodyModel(model);
            var response = await _client.ExecuteAsync(request, _optionsAccessor);
            result.IsSuccess = !response.IsError;
            result.StatusCode = ((int)response.StatusCode);
            result.Message = response.Message;
            result.Result = response.Body;
            if (!response.IsError)
            { 
                result.Qrcode = response.CodeUrl;
                result.OpenId = "";
                result.TotalFee = viewModel.Total.ToDecimal();
                result.TransactionId = "";
                result.OutTradeNo = viewModel.OutTradeNo;
                result.PrepayId = "";
            }
            return result;
        }

        /// <summary>
        /// H5支付-H5下单API
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        public async Task<PayCreateResponse> H5Pay(WeChatPayH5PayV3ViewModel viewModel)
        {
            PayCreateResponse result = new PayCreateResponse();
            var model = new WeChatPayTransactionsH5BodyModel
            {
                AppId = _optionsAccessor.AppId,
                MchId = _optionsAccessor.MchId,
                Amount = new Amount { Total = Convert.ToInt32(viewModel.Total.ToDecimal() * 100), Currency = viewModel.Currency },
                Description = viewModel.Description,
                NotifyUrl = viewModel.NotifyUrl,
                OutTradeNo = viewModel.OutTradeNo,
                SceneInfo = new SceneInfo { PayerClientIp = viewModel.IP, H5Info = new H5Info { Type = "Wap" } }
            };
            if (viewModel.GoodsDetail.Count > 0)
            {
                model.Detail = new Detail()
                {
                    CostPrice = Convert.ToInt32(viewModel.Total.ToDecimal() * 100),
                    GoodsDetail = viewModel.GoodsDetail
                };
            }
            var request = new WeChatPayTransactionsH5Request();
            request.SetBodyModel(model);
            var response = await _client.ExecuteAsync(request, _optionsAccessor);
            result.IsSuccess = !response.IsError;
            result.StatusCode = ((int)response.StatusCode);
            result.Message = response.Message;
            result.Result = response.Body; 
            result.OpenId = "";
            result.TotalFee = viewModel.Total.ToDecimal();
            result.TransactionId = "";
            result.OutTradeNo = viewModel.OutTradeNo;
            result.PrepayId = "";
            return result;
        }

        /// <summary>
        /// 小程序支付-JSAPI下单
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        public async Task<PayCreateResponse> MiniProgramPay(WeChatPayPubPayV3ViewModel viewModel)
        {
            PayCreateResponse result = new PayCreateResponse();
            var model = new WeChatPayTransactionsJsApiBodyModel
            {
                AppId = _optionsAccessor.AppId,
                MchId = _optionsAccessor.MchId,
                Amount = new Amount { Total = Convert.ToInt32(viewModel.Total.ToDecimal() * 100), Currency = viewModel.Currency },
                Description = viewModel.Description,
                NotifyUrl = viewModel.NotifyUrl,
                OutTradeNo = viewModel.OutTradeNo,
                Payer = new PayerInfo { OpenId = viewModel.OpenId }, 
            };
            if (viewModel.GoodsDetail.Count > 0)
            {
                model.Detail = new Detail()
                {
                    CostPrice = Convert.ToInt32(viewModel.Total.ToDecimal() * 100),
                    GoodsDetail = viewModel.GoodsDetail
                };
            }
            var request = new WeChatPayTransactionsJsApiRequest();
            request.SetBodyModel(model);
            var response = await _client.ExecuteAsync(request, _optionsAccessor);
            result.IsSuccess = !response.IsError;
            result.StatusCode = ((int)response.StatusCode);
            result.Message = response.Message;
            result.Result = response.Body; 
            if (!response.IsError)
            {
                WeChatPayDictionary sortedTxtParams = new WeChatPayDictionary();
                if (!string.IsNullOrEmpty(_optionsAccessor.SubAppId))
                {
                    sortedTxtParams.Add("appId", _optionsAccessor.SubAppId);
                }
                else
                {
                    sortedTxtParams.Add("appId", _optionsAccessor.AppId);
                }
                sortedTxtParams.Add("timeStamp", WeChatPayUtility.GetTimeStamp());
                sortedTxtParams.Add("nonceStr", WeChatPayUtility.GenerateNonceStr());
                sortedTxtParams.Add("package", "prepay_id=" + response.PrepayId);
                sortedTxtParams.Add("signType", "RSA"); 
                string paySign = "";
                string data = BuildSignatureSourceData(sortedTxtParams);
                var Certificate2 = new X509Certificate2(_optionsAccessor.Certificate, _optionsAccessor.CertificatePassword, X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.Exportable | X509KeyStorageFlags.PersistKeySet);
                using (RSA RSAPrivateKey = Certificate2.GetRSAPrivateKey())
                {
                    paySign = SHA256WithRSA.Sign(RSAPrivateKey, data);
                }
                sortedTxtParams.Add("paySign", paySign);
                result.Result = (new { package = sortedTxtParams.GetValue("package"), appId = sortedTxtParams.GetValue("appId"), timeStamp = sortedTxtParams.GetValue("timeStamp"), nonceStr = sortedTxtParams.GetValue("nonceStr"), signType = sortedTxtParams.GetValue("signType"), paySign = sortedTxtParams.GetValue("paySign") }).ToJson();
            }
            return result;
        }

        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="sortedTxtParams"></param>
        /// <returns></returns>
        private static string BuildSignatureSourceData(WeChatPayDictionary sortedTxtParams)
        {
            return $"{sortedTxtParams.GetValue("appId")}\n{sortedTxtParams.GetValue("timeStamp")}\n{sortedTxtParams.GetValue("nonceStr")}\n{sortedTxtParams.GetValue("package")}\n";
        }
         
        /// <summary>
        /// 微信支付订单号查询
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        public async Task<PayResult> QueryByTransactionId(WeChatPayQueryByTransactionIdViewModel viewModel)
        {
            PayResult result = new PayResult();
            var model = new WeChatPayTransactionsIdQueryModel
            {
                MchId = _optionsAccessor.MchId,
            };
            var request = new WeChatPayTransactionsIdRequest
            {
                TransactionId = viewModel.TransactionId
            };
            request.SetQueryModel(model);
            var response = await _client.ExecuteAsync(request, _optionsAccessor);
            if (((int)response.StatusCode) == 200 && (response.Code == "SUCCESS" || response.Code == null))
            {
                result.IsSuccess = true;
            }
            else
            {
                result.IsSuccess = false;
            }
            result.StatusCode = ((int)response.StatusCode);
            result.Message = response.Message;
            result.Result = response.Body;
            return result;
        }

        /// <summary>
        /// 商户订单号查询
        /// </summary>
        /// <param name="viewModel"></param> 
        public async Task<PayResult> QueryByOutTradeNo(WeChatPayQueryByOutTradeNoViewModel viewModel)
        {
            PayResult result = new PayResult();
            var model = new WeChatPayTransactionsOutTradeNoQueryModel
            {
                MchId = _optionsAccessor.MchId,
            };
            var request = new WeChatPayTransactionsOutTradeNoRequest
            {
                OutTradeNo = viewModel.OutTradeNo,
            };
            request.SetQueryModel(model);
            var response = await _client.ExecuteAsync(request, _optionsAccessor);
            if (((int)response.StatusCode) == 200 && (response.Code == "SUCCESS" || response.Code == null))
            {
                result.IsSuccess = true;
            }
            else
            {
                result.IsSuccess = false;
            }
            result.StatusCode = ((int)response.StatusCode);
            result.Message = response.Message;
            result.Result = response.Body;
            return result;
        }

        /// <summary>
        /// 关闭订单
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        public async Task<PayResult> OutTradeNoClose(WeChatPayOutTradeNoCloseViewModel viewModel)
        {
            PayResult result = new PayResult();
            var model = new WeChatPayTransactionsOutTradeNoCloseBodyModel
            {
                MchId = _optionsAccessor.MchId,
            };
            var request = new WeChatPayTransactionsOutTradeNoCloseRequest
            {
                OutTradeNo = viewModel.OutTradeNo,
            };
            request.SetBodyModel(model);
            var response = await _client.ExecuteAsync(request, _optionsAccessor);
            if (((int)response.StatusCode) == 200 && (response.Code == "SUCCESS" || response.Code == null))
            {
                result.IsSuccess = true;
            }
            else
            {
                result.IsSuccess = false;
            }
            result.StatusCode = ((int)response.StatusCode);
            result.Message = response.Message;
            result.Result = response.Body;
            return result;
        }

        /// <summary>
        /// 退款申请
        /// </summary>
        /// <param name="viewModel"></param>
        public async Task<PayResult> Refund(WeChatPayV3RefundViewModel viewModel)
        {
            PayResult result = new PayResult();
            var model = new WeChatPayRefundDomesticRefundsBodyModel()
            {
                TransactionId = viewModel.TransactionId,
                OutTradeNo = viewModel.OutTradeNo,
                OutRefundNo = viewModel.OutRefundNo,
                NotifyUrl = viewModel.NotifyUrl,
                Reason = viewModel.Reason,
                Amount = new RefundAmount { Refund = viewModel.RefundAmount, Total = viewModel.TotalAmount, Currency = viewModel.Currency }
            };
            var request = new WeChatPayRefundDomesticRefundsRequest();
            request.SetBodyModel(model);
            var response = await _client.ExecuteAsync(request, _optionsAccessor);
            if (((int)response.StatusCode) == 200 && (response.Code == "SUCCESS" || response.Code == null))
            {
                result.IsSuccess = true;
            }
            else
            {
                result.IsSuccess = false;
            }
            result.StatusCode = ((int)response.StatusCode);
            result.Message = response.Message;
            result.Result = response.Body;
            return result;
        }

        /// <summary>
        /// 查询单笔退款
        /// </summary>
        /// <param name="viewModel"></param>
        public async Task<PayResult> RefundQuery(WeChatPayV3RefundQueryViewModel viewModel)
        {
            PayResult result = new PayResult();
            var request = new WeChatPayRefundDomesticRefundsOutRefundNoRequest
            {
                OutRefundNo = viewModel.OutRefundNo
            };
            var response = await _client.ExecuteAsync(request, _optionsAccessor);
            if (((int)response.StatusCode) == 200 && (response.Code == "SUCCESS" || response.Code == null))
            {
                result.IsSuccess = true;
            }
            else
            {
                result.IsSuccess = false;
            }
            result.StatusCode = ((int)response.StatusCode);
            result.Message = response.Message;
            result.Result = response.Body;
            return result;
        }

        /// <summary>
        /// 申请交易账单
        /// </summary>
        /// <param name="viewModel"></param>=
        public async Task<PayResult> TradeBill(WeChatPayTradeBillViewModel viewModel)
        {
            PayResult result = new PayResult();
            var model = new WeChatPayBillTradeBillQueryModel
            {
                BillDate = viewModel.BillDate
            };
            var request = new WeChatPayBillTradeBillRequest();
            request.SetQueryModel(model);
            var response = await _client.ExecuteAsync(request, _optionsAccessor);
            if (((int)response.StatusCode) == 200 && (response.Code == "SUCCESS" || response.Code == null))
            {
                result.IsSuccess = true;
            }
            else
            {
                result.IsSuccess = false;
            }
            result.StatusCode = ((int)response.StatusCode);
            result.Message = response.Message;
            result.Result = response.Body;
            return result;
        }


        /// <summary>
        /// 申请资金账单
        /// </summary>
        /// <param name="viewModel"></param>
        public async Task<PayResult> FundflowBill(WeChatPayFundflowBillViewModel viewModel)
        {
            PayResult result = new PayResult();
            var model = new WeChatPayBillFundflowBillQueryModel
            {
                BillDate = viewModel.BillDate
            };

            var request = new WeChatPayBillFundflowBillRequest();
            request.SetQueryModel(model);
            var response = await _client.ExecuteAsync(request, _optionsAccessor);
            if (((int)response.StatusCode) == 200 && (response.Code == "SUCCESS" || response.Code == null))
            {
                result.IsSuccess = true;
            }
            else
            {
                result.IsSuccess = false;
            }
            result.StatusCode = ((int)response.StatusCode);
            result.Message = response.Message;
            result.Result = response.Body;
            return result;
        }


        /// <summary>
        /// 下载账单
        /// </summary>
        /// <param name="viewModel"></param>
        public async Task<PayResult> BillDownload(WeChatPayBillDownloadViewModel viewModel)
        {
            PayResult result = new PayResult();
            var request = new WeChatPayBillDownloadRequest();
            request.SetRequestUrl(viewModel.DownloadUrl);
            var response = await _client.ExecuteAsync(request, _optionsAccessor);
            if (((int)response.StatusCode) == 200 && (response.Code == "SUCCESS" || response.Code == null))
            {
                result.IsSuccess = true;
            }
            else
            {
                result.IsSuccess = false;
            }
            result.StatusCode = ((int)response.StatusCode);
            result.Message = response.Message;
            result.Result = response.Body;
            return result;
        }
         

        #endregion

        #region 微信支付分

        /// <summary>
        /// 支付分-创建支付分订单
        /// </summary>
        public async Task<PayResult> ServiceOrder(WeChatPayScoreServiceOrderViewModel viewModel)
        {
            PayResult result = new PayResult();
            var model = new WeChatPayScoreServiceOrderBodyModel
            {
                AppId = _optionsAccessor.AppId,
                ServiceId = viewModel.ServiceId,
                OutOrderNo = viewModel.OutOrderNo,
                ServiceIntroduction = viewModel.ServiceIntroduction,
                TimeRange = new TimeRange
                {
                    StartTime = viewModel.StartTime,
                    EndTime = viewModel.EndTime
                },
                RiskFund = new RiskFund
                {
                    Name = viewModel.RiskFundName,
                    Amount = viewModel.RiskFundAmount
                },
                NotifyUrl = viewModel.NotifyUrl,
                OpenId = viewModel.OpenId
            };
            var request = new WeChatPayScoreServiceOrderRequest();
            request.SetBodyModel(model);
            var response = await _client.ExecuteAsync(request, _optionsAccessor);
            if (((int)response.StatusCode) == 200 && (response.Code == "SUCCESS" || response.Code == null))
            {
                result.IsSuccess = true;
            }
            else
            {
                result.IsSuccess = false;
            }
            result.StatusCode = ((int)response.StatusCode);
            result.Message = response.Message;
            result.Result = response.Body;
            return result;
        }

        /// <summary>
        /// 支付分-查询支付分订单
        /// </summary>
        public async Task<PayResult> ServiceOrderQuery(WeChatPayScoreServiceOrderQueryViewModel viewModel)
        {
            PayResult result = new PayResult();
            var model = new WeChatPayScoreServiceOrderQueryModel
            {
                AppId = _optionsAccessor.AppId,
                ServiceId = viewModel.ServiceId,
                OutOrderNo = viewModel.OutOrderNo,
                QueryId = viewModel.QueryId
            };
            var request = new WeChatPayScoreServiceOrderQueryRequest();
            request.SetQueryModel(model);
            var response = await _client.ExecuteAsync(request, _optionsAccessor);
            if (((int)response.StatusCode) == 200 && (response.Code == "SUCCESS" || response.Code == null))
            {
                result.IsSuccess = true;
            }
            else
            {
                result.IsSuccess = false;
            }
            result.StatusCode = ((int)response.StatusCode);
            result.Message = response.Message;
            result.Result = response.Body;
            return result;
        }
         
        /// <summary>
        /// 支付分-取消支付分订单
        /// </summary> 
        public async Task<PayResult> ServiceOrderCancel(WeChatPayScoreServiceOrderCancelViewModel viewModel)
        {
            PayResult result = new PayResult();
            var model = new WeChatPayScoreServiceOrderOutOrderNoCancelBodyModel
            {
                AppId = _optionsAccessor.AppId,
                ServiceId = viewModel.ServiceId,
                Reason = viewModel.Reason
            };
            var request = new WeChatPayScoreServiceOrderOutOrderNoCancelRequest
            {
                OutOrderNo = viewModel.OutOrderNo
            };
            request.SetBodyModel(model);
            var response = await _client.ExecuteAsync(request, _optionsAccessor);
            if (((int)response.StatusCode) == 200 && (response.Code == "SUCCESS" || response.Code == null))
            {
                result.IsSuccess = true;
            }
            else
            {
                result.IsSuccess = false;
            }
            result.StatusCode = ((int)response.StatusCode);
            result.Message = response.Message;
            result.Result = response.Body;
            return result;
        }
         
        /// <summary>
        /// 支付分-修改支付分订单金额
        /// </summary> 
        public async Task<PayResult> ServiceOrderModify(WeChatPayScoreServiceOrderModifyViewModel viewModel)
        {
            PayResult result = new PayResult();
            var model = new WeChatPayScoreServiceOrderOutOrderNoModifyBodyModel
            {
                AppId = _optionsAccessor.AppId,
                ServiceId = viewModel.ServiceId,
                PostPayments = new List<PostPayment> {
                   new PostPayment{
                       Name = viewModel.Name,
                       Amount = viewModel.Amount,
                       Count = viewModel.Count
                   }
                },
                TotalAmount = viewModel.TotalAmount,
                Reason = viewModel.Reason
            };
            var request = new WeChatPayScoreServiceOrderOutOrderNoModifyRequest
            {
                OutOrderNo = viewModel.OutOrderNo
            };
            request.SetBodyModel(model);
            var response = await _client.ExecuteAsync(request, _optionsAccessor);
            if (((int)response.StatusCode) == 200 && (response.Code == "SUCCESS" || response.Code == null))
            {
                result.IsSuccess = true;
            }
            else
            {
                result.IsSuccess = false;
            }
            result.StatusCode = ((int)response.StatusCode);
            result.Message = response.Message;
            result.Result = response.Body;
            return result;
        }
         
        /// <summary>
        /// 支付分-完结支付分订单
        /// </summary> 
        public async Task<PayResult> ServiceOrderComplete(WeChatPayScoreServiceOrderCompleteViewModel viewModel)
        {
            PayResult result = new PayResult();
            var model = new WeChatPayScoreServiceOrderOutOrderNoCompleteBodyModel
            {
                AppId = _optionsAccessor.AppId,
                ServiceId = viewModel.ServiceId,
                PostPayments = new List<PostPayment>
                {
                   new PostPayment
                   {
                       Name = viewModel.Name,
                       Amount = viewModel.Amount,
                       Count = viewModel.Count
                   }
                },
                TotalAmount = viewModel.TotalAmount
            };
            var request = new WeChatPayScoreServiceOrderOutOrderNoCompleteRequest
            {
                OutOrderNo = viewModel.OutOrderNo
            };
            request.SetBodyModel(model);
            var response = await _client.ExecuteAsync(request, _optionsAccessor);
            if (((int)response.StatusCode) == 200 && (response.Code == "SUCCESS" || response.Code == null))
            {
                result.IsSuccess = true;
            }
            else
            {
                result.IsSuccess = false;
            }
            result.StatusCode = ((int)response.StatusCode);
            result.Message = response.Message;
            result.Result = response.Body;
            return result;
        }
         
        /// <summary>
        /// 支付分-商户发起催收扣款
        /// </summary> 
        public async Task<PayResult> ServiceOrderPay(WeChatPayScoreServiceOrderPayViewModel viewModel)
        {
            PayResult result = new PayResult();
            var model = new WeChatPayScoreServiceOrderPayBodyModel
            {
                AppId = _optionsAccessor.AppId,
                ServiceId = viewModel.ServiceId,
            };
            var request = new WeChatPayScoreServiceOrderPayRequest
            {
                OutOrderNo = viewModel.OutOrderNo
            };
            request.SetBodyModel(model);
            var response = await _client.ExecuteAsync(request, _optionsAccessor);
            if (((int)response.StatusCode) == 200 && (response.Code == "SUCCESS" || response.Code == null))
            {
                result.IsSuccess = true;
            }
            else
            {
                result.IsSuccess = false;
            }
            result.StatusCode = ((int)response.StatusCode);
            result.Message = response.Message;
            result.Result = response.Body;
            return result;
        }
         
        /// <summary>
        /// 支付分-同步服务订单信息
        /// </summary> 
        public async Task<PayResult> ServiceOrderSync(WeChatPayScoreServiceOrderSyncViewModel viewModel)
        {
            PayResult result = new PayResult();
            var model = new WeChatPayScoreServiceOrderSyncBodyModel
            {
                AppId = _optionsAccessor.AppId,
                ServiceId = viewModel.ServiceId,
                Type = viewModel.Type,
                Detail = new SyncDetail
                {
                    PaidTime = viewModel.PaidTime
                }
            };
            var request = new WeChatPayScoreServiceOrderSyncRequest
            {
                OutOrderNo = viewModel.OutOrderNo
            };
            request.SetBodyModel(model);
            var response = await _client.ExecuteAsync(request, _optionsAccessor);
            if (((int)response.StatusCode) == 200 && (response.Code == "SUCCESS" || response.Code == null))
            {
                result.IsSuccess = true;
            }
            else
            {
                result.IsSuccess = false;
            }
            result.StatusCode = ((int)response.StatusCode);
            result.Message = response.Message;
            result.Result = response.Body;
            return result;
        }
         
        /// <summary>
        /// 支付分-创单结单合并
        /// </summary> 
        public async Task<PayResult> ServiceOrderDirectComplete(WeChatPayScoreServiceOrderDirectCompleteViewModel viewModel)
        {
            PayResult result = new PayResult();
            var model = new WeChatPayScoreServiceOrderDirectCompleteBodyModel
            {
                AppId = _optionsAccessor.AppId,
                ServiceId = viewModel.ServiceId,
                OutOrderNo = viewModel.OutOrderNo,
                ServiceIntroduction = viewModel.ServiceIntroduction,
                PostPayments = new List<PostPayment> {
                   new PostPayment{
                       Name = viewModel.PostPaymentName,
                       Amount = viewModel.PostPaymentAmount,
                       Description = viewModel.PostPaymentDescription,
                       Count = viewModel.PostPaymentCount
                   }
                },
                TimeRange = new TimeRange
                {
                    StartTime = viewModel.StartTime,
                    EndTime = viewModel.EndTime
                },
                TotalAmount = viewModel.TotalAmount,
                NotifyUrl = viewModel.NotifyUrl,
                OpenId = viewModel.OpenId
            };
            var request = new WeChatPayScoreServiceOrderDirectCompleteRequest();
            request.SetBodyModel(model);
            var response = await _client.ExecuteAsync(request, _optionsAccessor);
            if (((int)response.StatusCode) == 200 && (response.Code == "SUCCESS" || response.Code == null))
            {
                result.IsSuccess = true;
            }
            else
            {
                result.IsSuccess = false;
            }
            result.StatusCode = ((int)response.StatusCode);
            result.Message = response.Message;
            result.Result = response.Body;
            return result;
        }
         
        /// <summary>
        /// 支付分-商户预授权
        /// </summary> 
        public async Task<PayResult> Permissions(PermissionsViewModel viewModel)
        {
            PayResult result = new PayResult();
            var model = new WeChatPayScorePermissionsBodyModel
            {
                AppId = _optionsAccessor.AppId,
                ServiceId = viewModel.ServiceId,
                AuthorizationCode = viewModel.AuthorizationCode,
                NotifyUrl = viewModel.NotifyUrl
            };
            var request = new WeChatPayScorePermissionsRequest();
            request.SetBodyModel(model);
            var response = await _client.ExecuteAsync(request, _optionsAccessor);
            if (((int)response.StatusCode) == 200 && (response.Code == "SUCCESS" || response.Code == null))
            {
                result.IsSuccess = true;
            }
            else
            {
                result.IsSuccess = false;
            }
            result.StatusCode = ((int)response.StatusCode);
            result.Message = response.Message;
            result.Result = response.Body;
            return result;
        }
         
        /// <summary>
        /// 支付分-查询用户授权记录（授权协议号）
        /// </summary> 
        public async Task<PayResult> PermissionsQueryForAuthCode(PermissionsQueryForAuthCodeViewModel viewModel)
        {
            PayResult result = new PayResult();
            var model = new WeChatPayScorePermissionsQueryForAuthCodeQueryModel
            {
                ServiceId = viewModel.ServiceId,
            };

            var request = new WeChatPayScorePermissionsQueryForAuthCodeRequest
            {
                AuthorizationCode = viewModel.AuthorizationCode
            };
            request.SetQueryModel(model);
            var response = await _client.ExecuteAsync(request, _optionsAccessor);
            if (((int)response.StatusCode) == 200 && (response.Code == "SUCCESS" || response.Code == null))
            {
                result.IsSuccess = true;
            }
            else
            {
                result.IsSuccess = false;
            }
            result.StatusCode = ((int)response.StatusCode);
            result.Message = response.Message;
            result.Result = response.Body;
            return result;
        }
         
        /// <summary>
        /// 支付分-解除用户授权关系（授权协议号）
        /// </summary> 
        public async Task<PayResult> PermissionsTerminateForAuthCode(PermissionsTerminateForAuthCodeViewModel viewModel)
        {
            PayResult result = new PayResult();
            var model = new WeChatPayScorePermissionsTerminateForAuthCodeBodyModel
            {
                ServiceId = viewModel.ServiceId,
                Reason = viewModel.Reason
            };
            var request = new WeChatPayScorePermissionsTerminateForAuthCodeRequest
            {
                AuthorizationCode = viewModel.AuthorizationCode
            };
            request.SetBodyModel(model);
            var response = await _client.ExecuteAsync(request, _optionsAccessor);
            if (((int)response.StatusCode) == 200 && (response.Code == "SUCCESS" || response.Code == null))
            {
                result.IsSuccess = true;
            }
            else
            {
                result.IsSuccess = false;
            }
            result.StatusCode = ((int)response.StatusCode);
            result.Message = response.Message;
            result.Result = response.Body;
            return result;
        }
         
        /// <summary>
        /// 支付分-查询用户授权记录（openid）
        /// </summary> 
        public async Task<PayResult> PermissionsQueryForOpenId(PermissionsQueryForOpenIdViewModel viewModel)
        {
            PayResult result = new PayResult();
            var model = new WeChatPayScorePermissionsQueryForOpenIdQueryModel
            {
                AppId = _optionsAccessor.AppId,
                ServiceId = viewModel.ServiceId,
            };
            var request = new WeChatPayScorePermissionsQueryForOpenIdRequest
            {
                OpenId = viewModel.OpenId
            };
            request.SetQueryModel(model);
            var response = await _client.ExecuteAsync(request, _optionsAccessor);
            if (((int)response.StatusCode) == 200 && (response.Code == "SUCCESS" || response.Code == null))
            {
                result.IsSuccess = true;
            }
            else
            {
                result.IsSuccess = false;
            }
            result.StatusCode = ((int)response.StatusCode);
            result.Message = response.Message;
            result.Result = response.Body;
            return result;
        }
         
        /// <summary>
        /// 支付分-解除用户授权关系（OpenId）
        /// </summary> 
        public async Task<PayResult> PermissionsTerminateForOpenId(PermissionsTerminateForOpenIdViewModel viewModel)
        {
            PayResult result = new PayResult();
            var model = new WeChatPayScorePermissionsTerminateForOpenIdBodyModel
            {
                AppId = _optionsAccessor.AppId,
                ServiceId = viewModel.ServiceId,
                Reason = viewModel.Reason
            };
            var request = new WeChatPayScorePermissionsTerminateForOpenIdRequest
            {
                OpenId = viewModel.OpenId
            };
            request.SetBodyModel(model);
            var response = await _client.ExecuteAsync(request, _optionsAccessor);
            if (((int)response.StatusCode) == 200 && (response.Code == "SUCCESS" || response.Code == null))
            {
                result.IsSuccess = true;
            }
            else
            {
                result.IsSuccess = false;
            }
            result.StatusCode = ((int)response.StatusCode);
            result.Message = response.Message;
            result.Result = response.Body;
            return result;
        }

        #endregion

    }
}
