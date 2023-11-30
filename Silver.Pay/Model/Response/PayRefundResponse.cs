using Silver.Pay.Model.Response.Refund;
using System;
using System.Collections.Generic;
using System.Text;

namespace Silver.Pay.Model.Response
{
    public class PayRefundResponse
    {
        public int StatusCode { get; set; }

        public bool IsSuccess { get; set; } = true;

        public string Message { get; set; } = "";

        public PayRefundCO Result { get; set; } = new PayRefundCO();

        public class PayRefundCO
        {
            /// <summary>
            /// 支付退款单号
            /// </summary>
            public string RefundId { get; set; }

            /// <summary>
            /// 商户退款单号
            /// </summary>
            public string OutRefundNo { get; set; }

            /// <summary>
            /// 支付订单号	
            /// </summary>
            public string TransactionId { get; set; }

            /// <summary>
            /// 商户订单号
            /// </summary>
            public string OutTradeNo { get; set; }

            /// <summary>
            /// 退款入账账户
            /// </summary>
            public string UserReceivedAccount { get; set; }

            /// <summary>
            /// 退款状态
            /// SUCCESS：退款成功
            /// CLOSED：退款关闭
            /// PROCESSING：退款处理中
            /// ABNORMAL：退款异常
            /// </summary>
            public string Status { get; set; }

            /// <summary>
            /// 订单金额
            /// </summary>
            public decimal Total { get; set; }

            /// <summary>
            /// 退款金额
            /// </summary>
            public decimal Refund { get; set; }

            /// <summary>
            /// 用户支付金额
            /// </summary>
            public decimal PayerTotal { get; set; }

            /// <summary>
            /// 用户退款金额	
            /// </summary>
            public decimal PayerRefund { get; set; }


            public object Wechat { get; set; }

            public object AliPay { get; set; }
        }

    }
}
