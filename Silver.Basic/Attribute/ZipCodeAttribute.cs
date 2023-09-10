using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Silver.Basic.Core;

namespace Silver.Basic.Attribute
{
    public class ZipCodeAttribute : RegularExpressionAttribute
    {
        public ZipCodeAttribute() : base(ExtRegularString.ZipCode)
        {
            ErrorMessage = "邮政编码格式不正确";
        }

        public ZipCodeAttribute(string msg) : base(ExtRegularString.ZipCode)
        {
            ErrorMessage = msg;
        }


    }
}
