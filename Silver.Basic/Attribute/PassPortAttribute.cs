using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Silver.Basic.Core;

namespace Silver.Basic.Attribute
{ 
    public class PassPortAttribute : RegularExpressionAttribute
    {
        public PassPortAttribute() : base(ExtRegularString.PassPort)
        {
            ErrorMessage = "护照格式不正确";
        }

        public PassPortAttribute(string msg) : base(ExtRegularString.PassPort)
        {
            ErrorMessage = msg;
        }

    }
}
