using Silver.Pay.Model.Enum;

namespace Silver.Pay.Model.Request
{
    /// <summary>
    /// 支付请求参数
    /// </summary>
    public class PayRequest
    {
        /// <summary>
        /// 支付类型：wechat   alipay
        /// </summary>
        public PayTypeEnum PayType { get; set; } = PayTypeEnum.WeChat;

        /// <summary>
        /// 支付方式：
        /// AliPayPreCreate：当面付-扫码支付
        /// AliPayPay：当面付-二维码/条码/声波支付
        /// AliPayAPP：APP支付
        /// AliPayWeb：电脑网站支付
        /// AliPayWap：手机网站支付
        /// WeChatMicro：商户扫描客户支付码
        /// WeChatAPP：APP支付-App下单API
        /// WeChatPub：公众号支付-JSAPI下单
        /// WeChatQrCode：扫码支付-Native下单API
        /// WeChatH5：H5支付-H5下单API
        /// WeChatMiniProgram：小程序支付-JSAPI下单
        /// </summary>
        public PayIdEnum PayId { get; set; }

        /// <summary>
        /// 商户订单号
        /// AliPayPreCreate
        /// AliPayPay
        /// AliPayAPP
        /// AliPayWeb
        /// AliPayWap
        /// WeChatMicro
        /// WeChatAPP
        /// WeChatPub
        /// WeChatQrCode
        /// WeChatH5
        /// WeChatMiniProgram
        /// </summary>
        public string OutTradeNo { get; set; } = "";

        /// <summary>
        /// 标题
        /// AliPayPreCreate
        /// AliPayPay
        /// AliPayAPP
        /// AliPayWeb
        /// AliPayWap
        /// </summary>
        public string Subject { get; set; } = "";

        /// <summary>
        /// 订单描述
        /// AliPayPreCreate
        /// AliPayPay
        /// AliPayAPP
        /// AliPayWeb
        /// AliPayWap
        /// WeChatMicro
        /// WeChatAPP
        /// WeChatPub
        /// WeChatQrCode
        /// WeChatH5
        /// WeChatMiniProgram
        /// </summary>
        public string Body { get; set; } = "";

        /// <summary>
        /// 订单总金额 (元)
        /// AliPayPreCreate
        /// AliPayPay
        /// AliPayAPP
        /// AliPayWeb
        /// AliPayWap
        /// WeChatMicro
        /// WeChatAPP
        /// WeChatPub
        /// WeChatQrCode
        /// WeChatH5
        /// WeChatMiniProgram
        /// </summary>
        public decimal TotalAmount { get; set; }

        /// <summary>
        /// 异步通知地址
        /// AliPayPreCreate
        /// AliPayAPP
        /// AliPayWeb
        /// AliPayWap
        /// WeChatAPP
        /// WeChatPub
        /// WeChatQrCode
        /// WeChatH5
        /// WeChatMiniProgram
        /// </summary>
        public string NotifyUrl { get; set; } = "";

        /// <summary>
        /// 同步通知地址
        /// AliPayWeb
        /// AliPayWap
        /// </summary>
        public string ReturnUrl { get; set; } = "";

        /// <summary>
        /// 支付授权码 【扫码专用】
        /// AliPayPay
        /// WeChatMicro
        /// WeChatPub
        /// WeChatQrCode
        /// </summary>
        public string AuthCode { get; set; } = "";

        /// <summary>
        /// IP地址
        /// WeChatMicro 
        /// WeChatH5
        /// </summary>
        public string IP { get; set; } = "";

        /// <summary>
        /// 微信openid  【微信】
        /// WeChatPub
        /// WeChatMiniProgram
        /// </summary>
        public string OpenId { get; set; } = "";


    }
}
