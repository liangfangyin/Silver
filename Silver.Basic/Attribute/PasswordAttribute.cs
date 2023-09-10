using System.ComponentModel.DataAnnotations;

namespace Silver.Basic.Attribute
{
    public class PasswordAttribute : RegularExpressionAttribute
    {
        private const string RegexPattern = @"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]{6,18}$";

        public PasswordAttribute() : base(RegexPattern)
        {
            ErrorMessage = "请输入6-18位字母与数字组合的密码";
        }

        public PasswordAttribute(string msg) : base(RegexPattern)
        {
            ErrorMessage = msg;
        }

    }

}
