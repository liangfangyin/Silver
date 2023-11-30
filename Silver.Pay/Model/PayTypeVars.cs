using System;
using System.Collections.Generic;
using System.Text;

namespace Silver.Pay.Model
{
    public class PayTypeVars
    {

        /// <summary>
        /// 微信
        /// </summary>
        public static int WeChat { get; set; } = 2;

        /// <summary>
        /// 支付宝
        /// </summary>
        public static int AliPay { get; set; } = 3;

        /// <summary>
        /// 数字货币
        /// </summary>
        public static int Digital { get; set; } = 5;

    }
}
