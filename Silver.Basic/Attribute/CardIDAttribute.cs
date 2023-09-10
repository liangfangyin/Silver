using System.ComponentModel.DataAnnotations;
using Silver.Basic.Core;

namespace Silver.Basic.Attribute
{

    public class CardIDAttribute : RegularExpressionAttribute
    {
        public CardIDAttribute() : base(ExtRegularString.CardID)
        {
            ErrorMessage = "身份证格式不正确";
        }

        public CardIDAttribute(string msg) : base(ExtRegularString.CardID)
        {
            ErrorMessage = msg;
        }

    }

}
