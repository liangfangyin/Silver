using Silver.Pay.Model.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace Silver.Pay.Model.Request
{
    public class QueryRequest
    {

        /// <summary>
        /// 支付方式 
        /// </summary>
        public PayTypeEnum PayType { get; set; }

        /// <summary>
        /// 商户订单号
        /// </summary>
        public string OutTradeNo { get; set; } = "";

        /// <summary>
        /// 交易号
        /// </summary>
        public string TransactionId { get; set; } = "";

    }
}
