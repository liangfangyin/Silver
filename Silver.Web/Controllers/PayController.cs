using Essensoft.Paylink.Alipay;
using Essensoft.Paylink.WeChatPay;
using Essensoft.Paylink.WeChatPay.V3;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Silver.Pay.Model.Request;
using Silver.Pay.Payment;
using Silver.Web.Model;
using Silver.WeChat;

namespace Silver.Web.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class PayController : ApiControllerBase
    { 
        private IPayment payment;
        public PayController(IAlipayNotifyClient notifyAlipayClient, IWeChatPayNotifyClient notifyWeChatClient, IAlipayClient aliPay, IOptions<AlipayOptions> aliPayOptions, IWeChatPayClient weiChat, Essensoft.Paylink.WeChatPay.V2.IWeChatPayClient weiChatV2, IOptions<WeChatPayOptions> weiChatOptions)
        {
            this.payment = new Payment( notifyAlipayClient, notifyWeChatClient, aliPay, aliPayOptions.Value, weiChat, weiChatV2, weiChatOptions.Value);
        }

        [HttpPost]
        public async Task<ResponseModel> WeChat([FromBody] PayRequest request)
        {
            WeChatClient chatClient = new WeChatClient("wx507c3250835e3cb2", "a536c638cb259138e2586a75b576363d");
            var result = await chatClient.ExecuteSnsOAuth2AccessTokenAsync(request.OutTradeNo);
            if (!result.Item1)
            {
                json.code = ErrorCodetaticVars.DataFailure;
                json.msg = result.Item3;
                return json;
            }
            var resultUser = await chatClient.ExecuteCgibinUserInfoAsync(result.Item2.OpenId);


         
            return json;
        }

        [HttpPost]
        public async Task<ResponseModel> Create([FromBody] PayRequest request)
        {
            var result = await payment.Pay(request);
            json.code = result.IsSuccess ? ErrorCodetaticVars.Succcess : ErrorCodetaticVars.DataFailure;
            json.msg = result.Message;
            json.data = new
            {
                info = result.Result
            };
            return json;
        }

        /// <summary>
        /// 订单查询
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost(Name = "Query")]
        public async Task<IActionResult> Query([FromBody] QueryRequest request)
        {
            var result = await payment.Query(request);
            return Ok(new { info = result });
        }

        /// <summary>
        /// 退款
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ResponseModel> Refund([FromBody] RefundRequest request)
        {
            var result = await payment.Refund(request);
            json.code = result.IsSuccess ? ErrorCodetaticVars.Succcess : ErrorCodetaticVars.DataFailure;
            json.msg = result.Message;
            json.data = new
            {
                info = result.Result
            };
            return json;
        }

        [HttpPost]
        public async Task<ResponseModel> RefundQuery([FromBody] RefundQueryRequest request)
        {
            var result = await payment.RefundQuery(request);
            json.code = result.IsSuccess ? ErrorCodetaticVars.Succcess : ErrorCodetaticVars.DataFailure;
            json.msg = result.Message;
            json.data = new
            {
                info = result.Result
            };
            return json;
        }

        [HttpPost]
        public async Task<ResponseModel> Close([FromBody] CloseRequest request)
        {
            var result = await payment.Close(request);
            json.code = result.IsSuccess ? ErrorCodetaticVars.Succcess : ErrorCodetaticVars.DataFailure;
            json.msg = result.Message;
            json.data = new
            {
                info = result.Result
            };
            return json;
        }




    }
}
