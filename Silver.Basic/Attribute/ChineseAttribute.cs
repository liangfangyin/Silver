using System.ComponentModel.DataAnnotations;
using Silver.Basic.Core;

namespace Silver.Basic.Attribute
{
    public class ChineseAttribute:RegularExpressionAttribute
    {

        public ChineseAttribute() : base(ExtRegularString.Chinese)
        {
            ErrorMessage = "请输入中文";
        }

        public ChineseAttribute(string msg) : base(ExtRegularString.Chinese)
        {
            ErrorMessage = msg;
        }

    }
}
