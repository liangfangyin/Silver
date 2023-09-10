using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Silver.Basic.Core;

namespace Silver.Basic.Attribute
{
    public class HTMLAttribute:RegularExpressionAttribute
    {

        public HTMLAttribute() : base(ExtRegularString.Html)
        {
            ErrorMessage = "HTML格式不正确";
        }

        public HTMLAttribute(string msg) : base(ExtRegularString.Html)
        {
            ErrorMessage = msg;
        }

    }
}
