using System;
using System.Collections.Generic;
using System.Text;

namespace Silver.Pay.Model.Response.Refund
{
    public class AlipayRefundResponse
    {

        public AlipayTradeRefundResponse alipay_trade_refund_response { get; set; } = new AlipayTradeRefundResponse();


        public class AlipayTradeRefundResponse
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
            /// 用户的登录id
            /// </summary>
            public string buyer_logon_id { get; set; }
            /// <summary>
            /// 本次退款是否发生了资金变化
            /// </summary>
            public string fund_change { get; set; }
            /// <summary>
            /// 退款总金额。
            /// </summary>
            public double refund_fee { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public List<Refund_detail_item_listItem> refund_detail_item_list { get; set; }
            /// <summary>
            /// 望湘园联洋店
            /// </summary>
            public string store_name { get; set; }
            /// <summary>
            /// 买家在支付宝的用户id
            /// </summary>
            public string buyer_user_id { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string send_back_fee { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string refund_hyb_amount { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public List<Refund_charge_info_listItem> refund_charge_info_list { get; set; }
        }


       

    }

    public class Refund_charge_info_listItem
    {
        /// <summary>
        /// 实退费用
        /// </summary>
        public double refund_charge_fee { get; set; }
        /// <summary>
        /// 签约费率
        /// </summary>
        public string switch_fee_rate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string charge_type { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<Refund_sub_fee_detail_listItem> refund_sub_fee_detail_list { get; set; }
    }

    public class Refund_detail_item_listItem
    {
        /// <summary>
        /// 
        /// </summary>
        public string fund_channel { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public double amount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public double real_amount { get; set; }
        /// <summary>
        /// 签约费率
        /// </summary>
        public string fund_type { get; set; }
    }

    public class Refund_sub_fee_detail_listItem
    {
        /// <summary>
        /// 实退费用
        /// </summary>
        public double refund_charge_fee { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string switch_fee_rate { get; set; }
    }


}
