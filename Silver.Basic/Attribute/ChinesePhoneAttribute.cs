using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Silver.Basic.Core;

namespace Silver.Basic.Attribute
{
    public class ChinesePhoneAttribute : RegularExpressionAttribute
    {
        public ChinesePhoneAttribute() : base(ExtRegularString.ChinesePhone)
        {
            ErrorMessage = "固定电话格式不正确";
        }

        public ChinesePhoneAttribute(string msg) : base(ExtRegularString.ChinesePhone)
        {
            ErrorMessage = msg;
        }

    }
}
