using System.ComponentModel.DataAnnotations;
using Silver.Basic.Core;

namespace Silver.Basic.Attribute
{
    public class Ip4AddressAttribute : RegularExpressionAttribute
    {
        public Ip4AddressAttribute() : base(ExtRegularString.Ip4Address)
        {
            ErrorMessage = "IP4格式不正确";
        }

        public Ip4AddressAttribute(string msg) : base(ExtRegularString.Ip4Address)
        {
            ErrorMessage = msg;
        }

    }

    public class Ip6AddressAttribute : RegularExpressionAttribute
    {
        public Ip6AddressAttribute() : base(ExtRegularString.Ip6Address)
        {
            ErrorMessage = "IP6格式不正确";
        }

        public Ip6AddressAttribute(string msg) : base(ExtRegularString.Ip6Address)
        {
            ErrorMessage = msg;
        }

    }

}
