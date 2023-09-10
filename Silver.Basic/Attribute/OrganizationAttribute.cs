using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Silver.Basic.Core;

namespace Silver.Basic.Attribute
{ 
    public class OrganizationAttribute : RegularExpressionAttribute
    {
        public OrganizationAttribute() : base(ExtRegularString.Organization)
        {
            ErrorMessage = "组织机构代码证格式不正确";
        }

        public OrganizationAttribute(string msg) : base(ExtRegularString.Organization)
        {
            ErrorMessage = msg;
        }

    }
}
