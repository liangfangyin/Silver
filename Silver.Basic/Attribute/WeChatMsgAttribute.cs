using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Silver.Basic.Core;

namespace Silver.Basic.Attribute
{
    public class WeChatMsgAttribute : RegularExpressionAttribute
    {
        public WeChatMsgAttribute() : base(ExtRegularString.WeChatMsg)
        {
            ErrorMessage = "微信消息模板ID格式不正确";
        }

        public WeChatMsgAttribute(string msg) : base(ExtRegularString.WeChatMsg)
        {
            ErrorMessage = msg;
        }

    }
}
