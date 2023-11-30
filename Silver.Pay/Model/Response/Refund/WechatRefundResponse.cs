using System;
using System.Collections.Generic;
using System.Text;

namespace Silver.Pay.Model.Response.Refund
{
    public class WechatRefundResponse
    {

        /// <summary>
        /// 微信支付退款单号
        /// </summary>
        public string refund_id { get; set; }
        /// <summary>
        /// 商户退款单号
        /// </summary>
        public string out_refund_no { get; set; }
        /// <summary>
        /// 微信支付订单号	
        /// </summary>
        public string transaction_id { get; set; }
        /// <summary>
        /// 商户订单号
        /// </summary>
        public string out_trade_no { get; set; }
        /// <summary>
        /// 退款渠道 枚举值：
        /// ORIGINAL：原路退款
        /// BALANCE：退回到余额
        /// OTHER_BALANCE：原账户异常退到其他余额账户
        /// OTHER_BANKCARD：原银行卡异常退到其他银行卡
        /// </summary>
        public string channel { get; set; }
        /// <summary>
        /// 退款入账账户
        /// </summary>
        public string user_received_account { get; set; }
        /// <summary>
        /// 退款成功时间
        /// </summary>
        public string success_time { get; set; }
        /// <summary>
        /// 退款创建时间
        /// </summary>
        public string create_time { get; set; }
        /// <summary>
        /// 退款状态
        /// SUCCESS：退款成功
        /// CLOSED：退款关闭
        /// PROCESSING：退款处理中
        /// ABNORMAL：退款异常
        /// </summary>
        public string status { get; set; }
        /// <summary>
        /// 资金账户
        /// </summary>
        public string funds_account { get; set; }
        /// <summary>
        /// -金额信息
        /// </summary>
        public WechatRefundAmount amount { get; set; }
        /// <summary>
        /// 退款出资账户及金额
        /// </summary>
        public List<WechatRefundPromotionDetailItem> promotion_detail { get; set; }

    }

    public class RefundFromItem
    {
        /// <summary>
        /// 出资账户类型  AVAILABLE : 可用余额   UNAVAILABLE : 不可用余额
        /// </summary>
        public string account { get; set; }
        /// <summary>
        /// 出资金额
        /// </summary>
        public int amount { get; set; }
    }

    public class WechatRefundAmount
    {
        /// <summary>
        /// 订单金额
        /// </summary>
        public int total { get; set; }
        /// <summary>
        /// 退款金额
        /// </summary>
        public int refund { get; set; }
        /// <summary>
        /// 退款出资账户及金额
        /// </summary>
        public List<RefundFromItem> from { get; set; }
        /// <summary>
        /// 用户支付金额
        /// </summary>
        public int payer_total { get; set; }
        /// <summary>
        /// 用户退款金额	
        /// </summary>
        public int payer_refund { get; set; }
        /// <summary>
        /// 应结退款金额
        /// </summary>
        public int settlement_refund { get; set; }
        /// <summary>
        /// 应结订单金额
        /// </summary>
        public int settlement_total { get; set; }
        /// <summary>
        /// 优惠退款金额
        /// </summary>
        public int discount_refund { get; set; }
        /// <summary>
        /// 退款币种
        /// </summary>
        public string currency { get; set; }
    }

    public class WechatRefundGoodsdetailItem
    {
        /// <summary>
        /// 
        /// </summary>
        public string merchant_goods_id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string wechatpay_goods_id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string goods_name { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int unit_price { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int refund_amount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int refund_quantity { get; set; }
    }

    public class WechatRefundPromotionDetailItem
    {
        /// <summary>
        /// 
        /// </summary>
        public string promotion_id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string scope { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string type { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int amount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int refund_amount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<WechatRefundGoodsdetailItem> goods_detail { get; set; }
    }

}
