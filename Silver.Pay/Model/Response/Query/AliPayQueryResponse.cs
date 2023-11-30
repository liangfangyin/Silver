using System;
using System.Collections.Generic;
using System.Text;

namespace Silver.Pay.Model.Response.Query
{
    public class AliPayQueryResponse
    {

        public AliPayQeryItem alipay_trade_query_response { get; set; }


      
    }

    public class AliPayQeryItem
    {

        /// <summary>
        /// 支付宝交易号
        /// </summary>
        public string trade_no { get; set; }
        /// <summary>
        /// 商家订单号
        /// </summary>
        public string out_trade_no { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string buyer_open_id { get; set; }
        /// <summary>
        /// 买家支付宝账号
        /// </summary>
        public string buyer_logon_id { get; set; }
        /// <summary>
        /// 交易状态：
        /// WAIT_BUYER_PAY（交易创建，等待买家付款）、
        /// TRADE_CLOSED（未付款交易超时关闭，或支付完成后全额退款）、
        /// TRADE_SUCCESS（交易支付成功）、
        /// TRADE_FINISHED（交易结束，不可退款）
        /// </summary>
        public string trade_status { get; set; }
        /// <summary>
        /// 交易的订单金额，单位为元
        /// </summary>
        public double total_amount { get; set; }
        /// <summary>
        /// 标价币种
        /// </summary>
        public string trans_currency { get; set; }
        /// <summary>
        /// 订单结算币种
        /// </summary>
        public string settle_currency { get; set; }
        /// <summary>
        /// 结算币种订单金额
        /// </summary>
        public double settle_amount { get; set; }
        /// <summary>
        /// 订单支付币种
        /// </summary>
        public int pay_currency { get; set; }
        /// <summary>
        /// 支付币种订单金额
        /// </summary>
        public string pay_amount { get; set; }
        /// <summary>
        /// 结算币种兑换标价币种汇率
        /// </summary>
        public string settle_trans_rate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string trans_pay_rate { get; set; }
        /// <summary>
        /// 标价币种兑换支付币种汇率
        /// </summary>
        public string alipay_store_id { get; set; }
        /// <summary>
        /// 买家实付金额
        /// </summary>
        public double buyer_pay_amount { get; set; }
        /// <summary>
        /// 积分支付的金额
        /// </summary>
        public double point_amount { get; set; }
        /// <summary>
        /// 交易中用户支付的可开具发票的金额
        /// </summary>
        public double invoice_amount { get; set; }
        /// <summary>
        /// 本次交易打款给卖家的时间
        /// </summary>
        public string send_pay_date { get; set; }
        /// <summary>
        /// 实收金额，单位为元
        /// </summary>
        public string receipt_amount { get; set; }
        /// <summary>
        /// 商户门店编号
        /// </summary>
        public string store_id { get; set; }
        /// <summary>
        /// 商户机具终端编号
        /// </summary>
        public string terminal_id { get; set; }
        /// <summary>
        /// 交易支付使用的资金渠道。
        /// </summary>
        public List<Fund_bill_listItem> fund_bill_list { get; set; }
        /// <summary>
        /// 请求交易支付中的商户店铺的名称
        /// </summary>
        public string store_name { get; set; }
        /// <summary>
        /// 买家在支付宝的用户id
        /// </summary>
        public string buyer_user_id { get; set; }
        /// <summary>
        /// [{"goods_id":"STANDARD1026181538","goods_name":"雪碧","discount_amount":"100.00","voucher_id":"2015102600073002039000002D5O"}]
        /// </summary>
        public string discount_goods_detail { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string industry_sepc_detail { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string industry_sepc_detail_gov { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string industry_sepc_detail_acc { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<Voucher_detail_listItem> voucher_detail_list { get; set; }
        /// <summary>
        /// 该笔交易针对收款方的收费金额； 只在银行间联交易场景下返回该信息；
        /// </summary>
        public string charge_amount { get; set; }
        /// <summary>
        /// 费率活动标识。
        /// </summary>
        public string charge_flags { get; set; }
        /// <summary>
        /// 支付清算编号，用于清算对账使用；
        /// </summary>
        public string settlement_id { get; set; }
        /// <summary>
        /// 返回的交易结算信息，包含分账、补差等信息。
        /// </summary>
        public Trade_settle_info trade_settle_info { get; set; }
        /// <summary>
        /// 预授权支付模式
        /// </summary>
        public string auth_trade_pay_mode { get; set; }
        /// <summary>
        /// 买家用户类型。CORPORATE:企业用户；PRIVATE:个人用户。
        /// </summary>
        public string buyer_user_type { get; set; }
        /// <summary>
        /// 商家优惠金额
        /// </summary>
        public string mdiscount_amount { get; set; }
        /// <summary>
        /// 平台优惠金额
        /// </summary>
        public string discount_amount { get; set; }
        /// <summary>
        /// 菜鸟网络有限公司
        /// </summary>
        public string buyer_user_name { get; set; }
        /// <summary>
        /// 订单标题；
        /// </summary>
        public string subject { get; set; }
        /// <summary>
        /// 订单描述
        /// </summary>
        public string body { get; set; }
        /// <summary>
        /// 间连商户在支付宝端的商户编号
        /// </summary>
        public string alipay_sub_merchant_id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ext_infos { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string passback_params { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Hb_fq_pay_info hb_fq_pay_info { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string receipt_currency_type { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string credit_pay_mode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string credit_biz_order_id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Enterprise_pay_info enterprise_pay_info { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string hyb_amount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Bkagent_resp_info bkagent_resp_info { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<Charge_info_listItem> charge_info_list { get; set; }

    }



    public class Fund_bill_listItem
    {
        /// <summary>
        /// 
        /// </summary>
        public string fund_channel { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string bank_code { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public double amount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public double real_amount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string fund_type { get; set; }
    }

    public class Other_contribute_detailItem
    {
        /// <summary>
        /// 
        /// </summary>
        public string contribute_type { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public double contribute_amount { get; set; }
    }

    public class Voucher_detail_listItem
    {
        /// <summary>
        /// 
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// XX超市5折优惠
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string type { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public double amount { get; set; }
 
        /// <summary>
        /// 学生专用优惠
        /// </summary>
        public string memo { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string template_id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<Other_contribute_detailItem> other_contribute_detail { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public double purchase_buyer_contribute { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public double purchase_merchant_contribute { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public double purchase_ant_contribute { get; set; }
    }

    public class Trade_settle_detail_listItem
    {
        /// <summary>
        /// 
        /// </summary>
        public string operation_type { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string operation_serial_no { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string operation_dt { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string trans_out { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string trans_in { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public double amount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ori_trans_out { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ori_trans_in { get; set; }
    }

    public class Trade_settle_info
    {
        /// <summary>
        /// 
        /// </summary>
        public List<Trade_settle_detail_listItem> trade_settle_detail_list { get; set; }
    }

    public class Hb_fq_pay_info
    {
        /// <summary>
        /// 
        /// </summary>
        public string user_install_num { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string fq_amount { get; set; }
    }

    public class Enterprise_pay_info
    {
        /// <summary>
        /// 
        /// </summary>
        public string is_use_enterprise_pay { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public double invoice_amount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string biz_info { get; set; }
    }

    public class Bkagent_resp_info
    {
        /// <summary>
        /// 
        /// </summary>
        public string bindtrx_id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string bindclrissr_id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string bindpyeracctbk_id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string bkpyeruser_code { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string estter_location { get; set; }
    }

    public class Sub_fee_detail_listItem
    {
        /// <summary>
        /// 
        /// </summary>
        public double charge_fee { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public double original_charge_fee { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string switch_fee_rate { get; set; }
    }

    public class Charge_info_listItem
    {
        /// <summary>
        /// 
        /// </summary>
        public double charge_fee { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public double original_charge_fee { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string switch_fee_rate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string is_rating_on_trade_receiver { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string is_rating_on_switch { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string charge_type { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<Sub_fee_detail_listItem> sub_fee_detail_list { get; set; }
    }




}
