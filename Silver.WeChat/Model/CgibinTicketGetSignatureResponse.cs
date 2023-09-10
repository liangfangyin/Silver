using System;
using System.Collections.Generic;
using System.Text;

namespace Silver.WeChat.Model
{
    public class CgibinTicketGetSignatureResponse
    {

        /// <summary>
        /// 时间戳
        /// </summary>
        public long Timestamp { get; set; }

        /// <summary>
        /// 随机数
        /// </summary>
        public string NonceStr { get; set; } = "";

        /// <summary>
        /// 签名
        /// </summary>
        public string Signature { get; set; } = "";

        /// <summary>
        /// 是否成果
        /// </summary>
        public bool IsSuccess { get; set; } = false;

        /// <summary>
        /// 错误信息
        /// </summary>
        public string ErrorMessage { get; set; } = "";

        /// <summary>
        ///  
        /// </summary>
        public string AppId { get; set; } = "";

    }
}
