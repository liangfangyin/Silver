using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Silver.Basic.Core;

namespace Silver.Basic.Attribute
{
    public class DrivingAttribute : RegularExpressionAttribute
    {
        public DrivingAttribute() : base(ExtRegularString.Driving)
        {
            ErrorMessage = "驾驶证号码格式不正确";
        }

        public DrivingAttribute(string msg) : base(ExtRegularString.Driving)
        {
            ErrorMessage = msg;
        }

    }
}
