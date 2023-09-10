using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Silver.Basic.Core;

namespace Silver.Basic.Attribute
{
    public class ICPAttribute : RegularExpressionAttribute
    {
        public ICPAttribute() : base(ExtRegularString.ICP)
        {
            ErrorMessage = "ICP备案号格式不正确";
        }

        public ICPAttribute(string msg) : base(ExtRegularString.ICP)
        {
            ErrorMessage = msg;
        }

    }
}
