namespace Silver.Pay.Model.Response.Query
{
    public class WechatQueryResponse
    {

        /// <summary>
        /// 支付金额
        /// </summary>
        public WechatAmount amount { get; set; } = new WechatAmount();

        /// <summary>
        /// 应用ID
        /// </summary>
        public string appid { get; set; } = "";
        /// <summary>
        /// 附加数据
        /// </summary>
        public string attach { get; set; } = "";
        /// <summary>
        /// 付款银行
        /// </summary>
        public string bank_type { get; set; } = "";
        /// <summary>
        /// 直连商户号
        /// </summary>
        public string mchid { get; set; } = "";
        /// <summary>
        /// 商户订单号
        /// </summary>
        public string out_trade_no { get; set; } = "";
        /// <summary>
        /// 支付者
        /// </summary>
        public WechatPayer payer { get; set; } = new WechatPayer();

        /// <summary>
        /// 支付完成时间
        /// </summary>
        public string success_time { get; set; } = "";
        /// <summary>
        /// 交易状态
        /// </summary>
        public string trade_state { get; set; } = "";
        /// <summary>
        /// 支付成功
        /// </summary>
        public string trade_state_desc { get; set; } = "";
        /// <summary>
        /// 交易类型
        /// </summary>
        public string trade_type { get; set; } = "";
        /// <summary>
        /// 微信支付订单号
        /// </summary>
        public string transaction_id { get; set; } = "";

    }


    public class WechatAmount
    {
        /// <summary>
        /// 货币类型 CNY：人民币，境内商户号仅支持人民币。
        /// </summary>
        public string currency { get; set; } = "";
        /// <summary>
        /// 用户支付币种
        /// </summary>
        public string payer_currency { get; set; } = "";
        /// <summary>
        /// 用户支付金额
        /// </summary>
        public int payer_total { get; set; }
        /// <summary>
        /// 总金额
        /// </summary>
        public int total { get; set; }
    }

    public class WechatPayer
    {
        /// <summary>
        /// 用户在直连商户appid下的唯一标识。
        /// </summary>
        public string openid { get; set; } = "";
    }

}
