using System;
using System.Collections.Generic;
using System.Text;

namespace Silver.Pay.Model.Response.Refund
{
    public class AlipayRefundQueryResponse
    {

        public AlipayTradeFastpayRefundQueryResponse alipay_trade_fastpay_refund_query_response { get; set; } = new AlipayTradeFastpayRefundQueryResponse();

        public class AlipayTradeFastpayRefundQueryResponse
        {

            /// <summary>
            /// 支付宝交易号
            /// </summary>
            public string trade_no { get; set; }
            /// <summary>
            /// 商户订单号
            /// </summary>
            public string out_trade_no { get; set; }

            /// <summary>
            /// 本笔退款对应的退款请求号
            /// </summary>
            public string out_request_no { get; set; }

            /// <summary>
            /// 该笔退款所对应的交易的订单金额
            /// </summary>
            public decimal total_amount { get; set; }

            /// <summary>
            /// 本次退款请求，对应的退款金额
            /// </summary>
            public decimal refund_amount { get; set; }

            /// <summary> 
            /// 退款状态。枚举值：
            /// REFUND_SUCCESS 退款处理成功；
            /// </summary>
            public string refund_status { get; set; }

            /// <summary>
            /// 退款时间。
            /// </summary>
            public string gmt_refund_pay { get; set; }

            /// <summary>
            /// 本次商户实际退回金额；。
            /// </summary>
            public string send_back_fee { get; set; }

        }



    }
     

}
