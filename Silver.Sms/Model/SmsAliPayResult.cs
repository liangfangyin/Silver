using System;
using System.Collections.Generic;
using System.Text;

namespace Peak.Lib.Sms.Model
{
    public class SmsAliPayResult
    {
        /// <summary>
        /// 提示语
        /// </summary>
        public string Message { get; set; } = "";

        /// <summary>
        /// 请求ID
        /// </summary>
        public string RequestId { get; set; } = "";

        /// <summary>
        /// 错误码
        /// </summary>
        public string Code { get; set; } = "";

        /// <summary>
        /// 返回码
        /// </summary>
        public string BizId { get; set; } = "";

        /// <summary>
        /// 是否成功
        /// </summary>
        public bool IsSuccess { get; set; } = true;

    }
}
