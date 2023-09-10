using Silver.Basic.Core;
using System.ComponentModel.DataAnnotations;

namespace Silver.Basic.Attribute
{

    public class BusinessAttribute : RegularExpressionAttribute
    {
        public BusinessAttribute() : base(ExtRegularString.ZipCode)
        {
            ErrorMessage = "营业执照格式不正确";
        }

        public BusinessAttribute(string msg) : base(ExtRegularString.ZipCode)
        {
            ErrorMessage = msg;
        }

    }

}
