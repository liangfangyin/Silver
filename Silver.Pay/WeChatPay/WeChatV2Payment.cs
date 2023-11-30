using Essensoft.Paylink.WeChatPay.V2;
using Essensoft.Paylink.WeChatPay.V2.Request;
using Silver.Basic;
using Silver.Pay.Model.Request;
using Silver.Pay.Model.Response.Create;

namespace Silver.Pay.WeChatPay
{
    public class WeChatV2Payment
    {
        private readonly IWeChatPayClient _client;
        private readonly Essensoft.Paylink.WeChatPay.WeChatPayOptions _optionsAccessor;
        public WeChatV2Payment(IWeChatPayClient client, Essensoft.Paylink.WeChatPay.WeChatPayOptions optionsAccessor)
        {
            _client = client;
            _optionsAccessor = optionsAccessor;
        }

        /// <summary>
        /// 商户扫描客户支付码
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        public async Task<PayCreateResponse> MicroPay(WeChatPayMicroPay viewModel)
        {
            PayCreateResponse result = new PayCreateResponse();
            var request = new WeChatPayMicroPayRequest
            {
                Body = viewModel.Body,
                OutTradeNo = viewModel.OutTradeNo,
                TotalFee = Convert.ToInt32(viewModel.TotalFee.ToDecimal() * 100),
                SpBillCreateIp = viewModel.SpBillCreateIp,
                AuthCode = viewModel.AuthCode
            };
            var response = await _client.ExecuteAsync(request, _optionsAccessor);
            result.IsSuccess = (response.ResultCode == "SUCCESS" && response.ReturnCode == "SUCCESS");
            if(response.ErrCode != null&& response.ResultCode == "FAIL")
            {
                if (response.ErrCode.ToUpper()== "USERPAYING")
                {
                    result.IsSuccess = true;
                }
            }
            result.Message = response.ErrCodeDes;
            result.Result = response.Body;
            var resultMicro = response.ToJson().JsonToObject<MicroPayResponse>();
            result.OpenId = resultMicro.OpenId;
            result.TotalFee = viewModel.TotalFee.ToDecimal();
            result.TransactionId=resultMicro.TransactionId;
            result.OutTradeNo = viewModel.OutTradeNo;
            result.PrepayId = ""; 
            return result;
        }

    }
}
