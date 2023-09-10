using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Silver.Basic.Core;

namespace Silver.Basic.Attribute
{
    public class ChineseMobileAttribute : RegularExpressionAttribute
    {
        public ChineseMobileAttribute() : base(ExtRegularString.ChineseMobile)
        {
            ErrorMessage = "手机号码格式不正确";
        }

        public ChineseMobileAttribute(string msg) : base(ExtRegularString.ChineseMobile)
        {
            ErrorMessage = msg;
        }

    }
}
