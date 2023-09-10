using System.ComponentModel.DataAnnotations;
using Silver.Basic.Core;

namespace Silver.Basic.Attribute
{
    public class UrlAttribute: RegularExpressionAttribute
    {

        public UrlAttribute() : base(ExtRegularString.Url)
        {
            ErrorMessage = "网址格式不正确";
        }

        public UrlAttribute(string msg) : base(ExtRegularString.Url)
        {
            ErrorMessage = msg;
        }


    }
}
