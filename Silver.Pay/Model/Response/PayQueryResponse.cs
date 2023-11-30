using System;
using System.Collections.Generic;
using System.Text;
using Silver.Pay.Model.Response.Query;

namespace Silver.Pay.Model.Response
{
    public class PayQueryResponse
    {
        public string Payment { get; set; } = "";

        /// <summary>
        /// 金额(元)
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// 应用ID
        /// </summary>
        public string Appid { get; set; } = "";

        /// <summary>
        /// 附加数据
        /// </summary>
        public string Attach { get; set; } = "";

        /// <summary>
        /// 直连商户号
        /// </summary>
        public string Mchid { get; set; } = "";

        /// <summary>
        /// 商户订单号
        /// </summary>
        public string outTradeNo { get; set; } = "";

        /// <summary>
        /// 用户appid下的唯一标识。
        /// </summary>
        public string Openid { get; set; } = "";

        /// <summary>
        /// 支付完成时间
        /// </summary>
        public string SuccessTime { get; set; } = "";

        /// <summary>
        /// 交易状态
        /// SUCCESS：支付成功
        /// REFUND：转入退款
        /// NOTPAY：未支付
        /// CLOSED：已关闭
        /// REVOKED：已撤销（仅付款码支付会返回）
        /// USERPAYING：用户支付中（仅付款码支付会返回）
        /// PAYERROR：支付失败（仅付款码支付会返回）
        /// </summary>
        public string TradeState { get; set; } = "";

        /// <summary>
        /// 交易类型
        /// </summary>
        public string TradeType { get; set; } = "";

        /// <summary>
        /// 支付订单号
        /// </summary>
        public string TransactionId { get; set; } = "";

        /// <summary>
        /// 
        /// </summary>
        public string Wechat { get; set; }


        public string AliPay { get; set; }


    }
}
