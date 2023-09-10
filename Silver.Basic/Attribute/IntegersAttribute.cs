using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Silver.Basic.Core;

namespace Silver.Basic.Attribute
{
    public class PositiveIntegersAttribute : RegularExpressionAttribute
    {
        public PositiveIntegersAttribute() : base(ExtRegularString.PositiveIntegers)
        {
            ErrorMessage = "正整数格式不正确";
        }

        public PositiveIntegersAttribute(string msg) : base(ExtRegularString.PositiveIntegers)
        {
            ErrorMessage = msg;
        }

    }

    public class NegativeIntegersAttribute : RegularExpressionAttribute
    {
        public NegativeIntegersAttribute() : base(ExtRegularString.NegativeIntegers)
        {
            ErrorMessage = "负整数格式不正确";
        }

        public NegativeIntegersAttribute(string msg) : base(ExtRegularString.NegativeIntegers)
        {
            ErrorMessage = msg;
        }

    }

    public class IntegerAttribute : RegularExpressionAttribute
    {
        public IntegerAttribute() : base(ExtRegularString.Integer)
        {
            ErrorMessage = "整数格式不正确";
        }

        public IntegerAttribute(string msg) : base(ExtRegularString.Integer)
        {
            ErrorMessage = msg;
        }

    }

    /// <summary>
    /// 正负浮点数
    /// </summary>
    public class FloatAttribute : RegularExpressionAttribute
    {
        public FloatAttribute() : base(ExtRegularString.Float)
        {
            ErrorMessage = "数字格式不正确";
        }

        public FloatAttribute(string msg) : base(ExtRegularString.Float)
        {
            ErrorMessage = msg;
        }

    }



}
