using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Silver.Basic.Core;

namespace Silver.Basic.Attribute
{
    public class LicenseAttribute : RegularExpressionAttribute
    {
        public LicenseAttribute() : base(ExtRegularString.License)
        {
            ErrorMessage = "车牌号格式不正确";
        }

        public LicenseAttribute(string msg) : base(ExtRegularString.License)
        {
            ErrorMessage = msg;
        }

    }
}
