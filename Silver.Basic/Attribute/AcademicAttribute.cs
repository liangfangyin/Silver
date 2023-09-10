using System.ComponentModel.DataAnnotations;
using Silver.Basic.Core;

namespace Silver.Basic.Attribute
{
    public class AcademicAttribute : RegularExpressionAttribute
    {
        public AcademicAttribute() : base(ExtRegularString.Academic)
        {
            ErrorMessage = "学位证书格式不正确";
        }

        public AcademicAttribute(string msg) : base(ExtRegularString.Academic)
        {
            ErrorMessage = msg;
        }

    }
}
