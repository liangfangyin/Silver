using Essensoft.Paylink.WeChatPay;
using Silver.Pay.Model.Enum;

namespace Silver.Pay.Model.Response.Create
{

    public class PayCreateResponse : PayResult
    {
        /// <summary>
        /// 是否成功
        /// </summary>
        public bool IsSuccess { get; set; }

        /// <summary>
        /// 提示
        /// </summary>
        public string Message { get; set; } = "";

        /// <summary>
        /// 二维码
        /// AliPayPreCreate
        /// </summary>
        public string Qrcode { get; set; } = "";

        /// <summary>
        /// 用户标识
        /// </summary> 
        public string OpenId { get; set; } = "";

        /// <summary>
        /// 用户账户
        /// </summary>
        public string OpenName { get; set; } = "";

        /// <summary>
        /// 金额
        /// </summary>
        public decimal TotalFee { get; set; }

        /// <summary>
        /// 交易单号
        /// </summary>
        public string TransactionId { get; set; } = "";

        /// <summary>
        /// 商户订单号
        /// </summary>
        public string OutTradeNo { get; set; } = "";

        /// <summary>
        /// 回话ID
        /// </summary>
        public string PrepayId { get; set; } = "";

        /// <summary>
        /// 支付方式
        /// </summary>
        public PayTypeEnum PayType { get; set; }

        /// <summary>
        /// 支付模型
        /// </summary>
        public PayIdEnum PayId { get; set; } 

    }
     

    public class AliPayPreCreateResponse
    {
        public string Qrcode { get; set; } = "";

    }

    public class WeChatAppResponse
    {
        public WeChatPayDictionary Parameter { get; set; }

        public string PrepayId { get; set; }

    }

    public class WeChatJSAPIResponse
    {
        public WeChatPayDictionary Parameter { get; set; }

        public string PrepayId { get; set; }

    }

    public class WeChatQrCodeResponse
    {
        public string Qrcode { get; set; } = "";

    }

    public class WeChatMiniProgramResponse
    {
        public WeChatPayDictionary Parameter { get; set; }

        public string PrepayId { get; set; }

    }

}
