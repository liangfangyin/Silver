using System;
using System.Collections.Generic;
using System.Text;

namespace Silver.Pay.Model
{
    public class PayModeVars
    {

        /// <summary>
        /// 卡
        /// </summary>
        public static int Card { get; set; } = 1;

        /// <summary>
        /// 现金
        /// </summary>
        public static int Cash { get; set; } = 2;

        /// <summary>
        /// 数字货币 301~399
        /// </summary>
        public static int Digital { get; set; } = 3;

        /// <summary>
        /// 支付宝-扫码 
        /// </summary>
        public static int AliPay_PreCreate { get; set; } = 4;

        /// <summary>
        /// 支付宝-二维码
        /// </summary>
        public static int AliPay_Pay { get; set; } = 5;

        /// <summary>
        /// 支付宝-APP
        /// </summary>
        public static int AliPay_App { get; set; } = 6;

        /// <summary>
        /// 支付宝-电脑H5
        /// </summary>
        public static int AliPay_Page { get; set; } = 7;

        /// <summary>
        /// 支付宝-手机H5
        /// </summary>
        public static int AliPay_Wap { get; set; } = 8;

        /// <summary>
        /// 支付宝-人脸
        /// </summary>
        public static int AliPay_Face { get; set; } = 14;

        /// <summary>
        /// 微信-APP
        /// </summary>
        public static int Wechat_App { get; set; } = 9;

        /// <summary>
        /// 微信-公众号支付
        /// </summary>
        public static int Wechat_JSAPI { get; set; } = 10;

        /// <summary>
        /// 微信-扫码
        /// </summary>
        public static int Wechat_QrCode { get; set; } = 11;

        /// <summary>
        /// 微信-H5
        /// </summary>
        public static int Wechat_H5 { get; set; } = 12;

        /// <summary>
        /// 微信-小程序
        /// </summary>
        public static int Wechat_MiniProgram { get; set; } = 13;

        /// <summary>
        /// 微信-人脸
        /// </summary>
        public static int Wechat_Face { get; set; } = 15;

        /// <summary>
        /// 微信-扫描客户端
        /// </summary>
        public static int Wechat_Micro { get; set; } = 16;

    }
}
