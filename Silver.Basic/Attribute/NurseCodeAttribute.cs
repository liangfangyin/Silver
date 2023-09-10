using System.ComponentModel.DataAnnotations;
using Silver.Basic.Core;

namespace Silver.Basic.Attribute
{
    public class NurseCodeAttribute : RegularExpressionAttribute
    {
        public NurseCodeAttribute() : base(ExtRegularString.NurseCode)
        {
            ErrorMessage = "护士资格证编号不正确";
        }

        public NurseCodeAttribute(string msg) : base(ExtRegularString.NurseCode)
        {
            ErrorMessage = msg;
        }


    }
}
