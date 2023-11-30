using System;
using System.Collections.Generic;
using System.Text;

namespace Silver.Pay.Model.Response.Create
{
    public class AlipayTradePayResponse
    {
        /// <summary>
        /// 
        /// </summary>
        public Alipay_trade_pay_response alipay_trade_pay_response { get; set; } = new Alipay_trade_pay_response();
        /// <summary>
        /// 
        /// </summary>
        public string alipay_cert_sn { get; set; } = "";
        /// <summary>
        /// 
        /// </summary>
        public string sign { get; set; } = "";

        public class Fund_bill_listItem
        {
            /// <summary>
            /// 
            /// </summary>
            public string amount { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string fund_channel { get; set; }
        }

        public class Alipay_trade_pay_response
        {
            /// <summary>
            /// 
            /// </summary>
            public string code { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string msg { get; set; }

            public string sub_msg { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public string buyer_logon_id { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string buyer_pay_amount { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string buyer_user_id { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string buyer_user_type { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public List<Fund_bill_listItem> fund_bill_list { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string gmt_payment { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string invoice_amount { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string out_trade_no { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string point_amount { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string receipt_amount { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string total_amount { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string trade_no { get; set; }
        }

    }
}
