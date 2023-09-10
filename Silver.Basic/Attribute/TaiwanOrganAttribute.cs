using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Silver.Basic.Core;

namespace Silver.Basic.Attribute
{ 
    public class TaiwanOrganAttribute : RegularExpressionAttribute
    {
        public TaiwanOrganAttribute() : base(ExtRegularString.TaiwanOrgan)
        {
            ErrorMessage = "台湾通行证格式不正确";
        }

        public TaiwanOrganAttribute(string msg) : base(ExtRegularString.TaiwanOrgan)
        {
            ErrorMessage = msg;
        }

    }
}
