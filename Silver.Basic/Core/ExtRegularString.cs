using System;
using System.Collections.Generic;
using System.Text;

namespace Silver.Basic.Core
{
    /// <summary>
    /// 正则表达式
    /// </summary>
    public class ExtRegularString
    {

        /// <summary>
        /// 邮件
        /// </summary>
        public const string Email = @"^[a-zA-Z0-9.!#$%&'*+\/=?^_`{|}~-]+@[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?(?:\.[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?)*$";
        /// <summary>
        /// 网址
        /// </summary>
        public const string Url = @"[a-zA-z]+://[^\s]*";
        /// <summary>
        /// 中文
        /// </summary>
        public const string Chinese = @"[\u4e00-\u9fa5]";
        /// <summary>
        /// HTML
        /// </summary>
        public const string Html = @"<(\S*?)[^>]*>.*?</\1>|<.*? />";
        /// <summary>
        /// 用户名
        /// </summary>
        public const string UserName = @"^[a-zA-Z][a-zA-Z0-9_]{4,15}$";
        /// <summary>
        /// 固定电话
        /// </summary>
        public const string ChinesePhone = @"\d{3}-\d{8}|\d{4}-\d{7}";
        /// <summary>
        /// 手机号
        /// </summary>
        public const string ChineseMobile = @"^1[3456789]\d{9}$";
        /// <summary>
        /// 邮政编码
        /// </summary>
        public const string ZipCode = @"[0-9]\d{5}(?!\d)";
        /// <summary>
        /// 身份证
        /// </summary>
        public const string CardID = @"^[1-9]\d{5}[1-9]\d{3}((0\d)|(1[0-2]))(([0|1|2]\d)|3[0-1])\d{3}(\d|x|X)$";
        /// <summary>
        /// 军官证
        /// </summary>
        public const string Officer = @"^[0-9]{8}$";
        /// <summary>
        /// 护照
        /// </summary>
        public const string PassPort = @"^[a-zA-Z0-9]{5,17}$";
        /// <summary>
        /// 营业执照
        /// </summary>
        public const string Business = @"^[a-zA-Z0-9]{10,20}$";
        /// <summary>
        /// 驾照
        /// </summary>
        public const string Driving = @"^[1-9]\d{5}[1-9]\d{3}((0\d)|(1[0-2]))(([0|1|2]\d)|3[0-1])\d{3}(\d|x|X)$";
        /// <summary>
        /// 组织机构代码证
        /// </summary>
        public const string Organization = @"^[a-zA-Z0-9]{10,20}$";
        /// <summary>
        /// 台胞证
        /// </summary>
        public const string TaiwanOrgan = @"^([0-9]{8}|[0-9]{10})$";
        /// <summary>
        /// 港澳通行证
        /// </summary>
        public const string HongPass = @"^[a-zA-Z0-9]{6,10}$";
        /// <summary>
        /// 车牌号
        /// </summary>
        public const string License = @"^([京津晋冀蒙辽吉黑沪苏浙皖闽赣鲁豫鄂湘粤桂琼渝川贵云藏陕甘青宁新][ABCDEFGHJKLMNPQRSTUVWXY][1-9DF][1-9ABCDEFGHJKLMNPQRSTUVWXYZ]\d{3}[1-9DF]|[京津晋冀蒙辽吉黑沪苏浙皖闽赣鲁豫鄂湘粤桂琼渝川贵云藏陕甘青宁新][ABCDEFGHJKLMNPQRSTUVWXY][\dABCDEFGHJKLNMxPQRSTUVWXYZ]{5})$";
        /// <summary>
        /// ICP备案号
        /// </summary>
        public const string ICP = @"^[京津晋冀蒙辽吉黑沪苏浙皖闽赣鲁豫鄂湘粤桂琼渝川贵云藏陕甘青宁新]ICP备\d{8}(-[1-9]\d?)?$";
        /// <summary>
        /// 银联付款码
        /// </summary>
        public const string UnionPayCode = @"^(?:62)\d{17}$";
        /// <summary>
        /// 护士资格证编号
        /// </summary>
        public const string NurseCode = @"^[21]\d{11}$";
        /// <summary>
        /// 学位证书
        /// </summary>
        public const string Academic = @"^(?:[1-9]\d{4}[423]2\d{3}[ES](?!00000)\d{5})|(?:C[1-9]\d{4}[423]2\d{3}Z(?!00000)\d{5})|(?:Z[1-9]\d{4}[423]2\d{3}[YCJESIKFABLDG](?!00000)\d{5})|(?:[CQTL]?[1-9]\d{4}[423]2\d{3}(?!000000)\d{6})$";
        /// <summary>
        /// 微信消息模板id（template_id）
        /// </summary>
        public const string WeChatMsg = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)[\w-]{43}$";
        /// <summary>
        /// IP4地址
        /// </summary>
        public const string Ip4Address = @"^((25[0-5]|2[0-4]\d|[01]?\d\d?)\.){3}(25[0-5]|2[0-4]\d|[01]?\d\d?)$";
        /// <summary>
        /// IP6地址
        /// </summary>
        public const string Ip6Address = @"^([a-f0-9]{1,4}(:[a-f0-9]{1,4}){7}|[a-f0-9]{1,4}(:[a-f0-9]{1,4}){0,7}::[a-f0-9]{0,4}(:[a-f0-9]{1,4}){0,7})$";
        /// <summary>
        /// 匹配正整数
        /// </summary>
        public const string PositiveIntegers = @"^[1-9]\d*$";
        /// <summary>
        /// 匹配负整数
        /// </summary>
        public const string NegativeIntegers = @"^-[1-9]\d*$";
        /// <summary>
        ///匹配整数
        /// </summary>
        public const string Integer = @"^-?[0-9]+$";
        /// <summary>
        /// 匹配非负整数（正整数 + 0）
        /// </summary>
        public const string PositiveIntegersAndZero = @"^[0-9]+$";
        /// <summary>
        /// 匹配非正整数（负整数 + 0）
        /// </summary>
        public const string NegativeIntegersAndZero = @"^-[0-9]+$";
        /// <summary>
        /// 匹配正浮点数
        /// </summary>
        public const string Float = @"^(\-|\+)?\d+(\.\d+)?$";
        /// <summary>
        /// 匹配由26个英文字母组成的字符串
        /// </summary>
        public const string Letters = @"^[A-Za-z]+$";
        /// <summary>
        /// 匹配由26个英文字母的大写组成的字符串
        /// </summary>
        public const string UppercaseLetters = @"^[A-Z]+$";
        /// <summary>
        /// 匹配由26个英文字母的小写组成的字符串
        /// </summary>
        public const string LowercaseLetters = @"^[a-z]+$";
        /// <summary>
        /// 匹配由数字和26个英文字母组成的字符串
        /// </summary>
        public const string LettersAndNumber = @"^[A-Za-z0-9]+$";
        /// <summary>
        /// 匹配由数字、26个英文字母或者下划线组成的字符串
        /// </summary>
        public const string LettersAndNumberAndLine = @"^\w+$";
    }
}
