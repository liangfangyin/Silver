using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Silver.Basic.Core;

namespace Silver.Basic.Attribute
{
    public class HongPassAttribute : RegularExpressionAttribute
    {
        public HongPassAttribute() : base(ExtRegularString.HongPass)
        {
            ErrorMessage = "港澳通行证格式不正确";
        }

        public HongPassAttribute(string msg) : base(ExtRegularString.HongPass)
        {
            ErrorMessage = msg;
        }

    }
}
