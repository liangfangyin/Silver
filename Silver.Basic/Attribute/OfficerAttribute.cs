using System.ComponentModel.DataAnnotations;
using Silver.Basic.Core;

namespace Silver.Basic.Attribute
{
    public class OfficerAttribute : RegularExpressionAttribute
    {
        public OfficerAttribute() : base(ExtRegularString.Officer)
        {
            ErrorMessage = "军官证格式不正确";
        }

        public OfficerAttribute(string msg) : base(ExtRegularString.Officer)
        {
            ErrorMessage = msg;
        }


    }

}
