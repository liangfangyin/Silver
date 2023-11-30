using Silver.Pay.Model.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace Silver.Pay.Model.Request
{
    public class CloseRequest
    {
        /// <summary>
        /// 支付方式
        ///  
        /// </summary>
        public PayTypeEnum Payment { get; set; }

        /// <summary>
        /// 微信、支付宝
        /// 支付商户单号
        /// </summary>
        public string OutTradeNo { get; set; } = "";

        /// <summary>
        /// 支付宝
        /// 异步通知地址
        /// </summary>
        public string NotifyUrl { get; set; } = "";

        /// <summary>
        /// 支付宝
        /// 支付交易号
        /// </summary>
        public string TransactionId { get; set; } = "";

    }
}
