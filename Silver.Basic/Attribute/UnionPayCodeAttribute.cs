using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Silver.Basic.Core;

namespace Silver.Basic.Attribute
{
    public class UnionPayCodeAttribute : RegularExpressionAttribute
    {
        public UnionPayCodeAttribute() : base(ExtRegularString.UnionPayCode)
        {
            ErrorMessage = "银联付款码格式不正确";
        }

        public UnionPayCodeAttribute(string msg) : base(ExtRegularString.UnionPayCode)
        {
            ErrorMessage = msg;
        }

    }
}
