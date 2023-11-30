using Silver.Pay.Model.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Xml.Linq;

namespace Silver.Pay.Model.Request
{
    public class RefundRequest
    {
        /// <summary>
        /// 支付方式 
        /// </summary>
        public PayTypeEnum PayType { get; set; } 

        /// <summary>
        /// 退款单号
        /// </summary>
        public string OutRefundNo { get; set; } = "";

        /// <summary>
        /// 支付交易号
        /// </summary>
        public string TransactionId { get; set; } = "";

        /// <summary>
        /// 支付商户单号
        /// </summary>
        public string OutTradeNo { get; set; } = "";

        /// <summary>
        /// 异步通知地址
        /// </summary>
        public string NotifyUrl { get; set; } = "";

        /// <summary>
        /// 退款金额
        /// </summary>
        public double RefundAmount { get; set; } 

        /// <summary>
        /// 订单总金额
        /// </summary>
        public double TotalAmount { get; set; } 

        /// <summary>
        /// 货币类型  CNY：人民币
        /// </summary>
        public string Currency { get; set; } = "CNY";

        /// <summary>
        /// 退款原因
        /// </summary>
        public string Reason { get; set; } = "";

    }
}
