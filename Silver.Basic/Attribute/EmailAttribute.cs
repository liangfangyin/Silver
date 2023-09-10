using System.ComponentModel.DataAnnotations;
using Silver.Basic.Core;

namespace Silver.Basic.Attribute
{
    public class EmailAttribute : RegularExpressionAttribute
    {

        public EmailAttribute() : base(ExtRegularString.Email)
        {
            ErrorMessage = "邮件格式不正确";
        }

        public EmailAttribute(string msg) : base(ExtRegularString.Email)
        {
            ErrorMessage = msg;
        }

    }
}
