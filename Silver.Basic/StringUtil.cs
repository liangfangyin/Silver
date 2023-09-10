﻿using Newtonsoft.Json;
using Silver.Basic.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using ToolGood.Words;

namespace Silver.Basic
{
    /// <summary>
    /// 字符串帮助类
    /// </summary>
    public static class StringUtil
    {
        #region 验证

        /// <summary>
        /// 是否空字符串或null
        /// </summary>
        /// <param name="theString"></param>
        /// <returns></returns>
        public static bool IsEmpty(this string theString)
        {
            if (string.IsNullOrEmpty(theString))
            {
                return true;
            }
            return theString.Trim().Length == 0;
        }

        /// <summary>
        /// 是否空字符串或null
        /// </summary>
        /// <param name="theString"></param>
        /// <returns></returns>
        public static bool IsNotEmpty(this string theString)
        {
            return !IsEmpty(theString);
        }

        /// <summary>
        /// 两个字符串是否相同，不区分大小写
        /// </summary>
        /// <param name="value1"></param>
        /// <param name="value2"></param>
        /// <returns></returns>
        public static bool IsEquals(this string value1, string value2)
        {
            return value1.ToLower() == value2.ToLower();
        }

        /// <summary>
        /// 两个字符串是否相同，区分大小写
        /// </summary>
        /// <param name="value1"></param>
        /// <param name="value2"></param>
        /// <returns></returns>
        public static bool IsEqualsCase(this string value1, string value2)
        {
            return value1 == value2;
        }

        /// <summary>
        /// 是否时间
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsDateTime(this string value)
        {
            if (DateTime.TryParse(value, out DateTime dt))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 是否图片
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsImage(this string value)
        {
            if (value.IsEmpty()) return false;
            value = value.ToLower();
            if (value.EndsWith(".gif") || value.EndsWith(".jpg") || value.EndsWith(".png") || value.EndsWith(".jpeg") || value.EndsWith(".bmp"))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 校验密码强度 
        /// </summary>
        /// <param name="password">密码</param>
        /// <returns>
        /// 至少6位、大小写、数字
        /// </returns>
        public static (bool, string) IsValidPassword(this string password)
        {
            string pwd = password;
            if (pwd.Length <= 6)
            {
                return (false, "密码过短，至少需要6个字符！");
            }
            var regex = new Regex(@"(?=.*[0-9])                     #必须包含数字
                                            (?=.*[a-zA-Z])                  #必须包含小写或大写字母
                                            # (?=([\x21-\x7e]+)[^a-zA-Z0-9])  #必须包含特殊符号
                                            .{6,100}                         #至少6个字符，最多30个字符
                                            ", RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace);
            if (!regex.Match(pwd).Success)
            {
                return (false, "密码必须包含数字，必须包含小写或大写字母");
            }
            return (true, "成功");
        }

        /// <summary>
        /// 判断IP是否是私有地址
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public static bool IsPrivateIP(this IPAddress ip)
        {
            if (IPAddress.IsLoopback(ip)) return true;
            ip = ip.IsIPv4MappedToIPv6 ? ip.MapToIPv4() : ip;
            byte[] bytes = ip.GetAddressBytes();
            return ip.AddressFamily switch
            {
                AddressFamily.InterNetwork when bytes[0] == 10 => true,
                AddressFamily.InterNetwork when bytes[0] == 100 && bytes[1] >= 64 && bytes[1] <= 127 => true,
                AddressFamily.InterNetwork when bytes[0] == 169 && bytes[1] == 254 => true,
                AddressFamily.InterNetwork when bytes[0] == 172 && bytes[1] == 16 => true,
                AddressFamily.InterNetwork when bytes[0] == 192 && bytes[1] == 88 && bytes[2] == 99 => true,
                AddressFamily.InterNetwork when bytes[0] == 192 && bytes[1] == 168 => true,
                AddressFamily.InterNetwork when bytes[0] == 198 && bytes[1] == 18 => true,
                AddressFamily.InterNetwork when bytes[0] == 198 && bytes[1] == 51 && bytes[2] == 100 => true,
                AddressFamily.InterNetwork when bytes[0] == 203 && bytes[1] == 0 && bytes[2] == 113 => true,
                AddressFamily.InterNetworkV6 when ip.IsIPv6Teredo || ip.IsIPv6LinkLocal || ip.IsIPv6Multicast || ip.IsIPv6SiteLocal => true,
                AddressFamily.InterNetworkV6 when ip.ToString().StartsWith("::") => true,
                AddressFamily.InterNetworkV6 when ip.ToString().StartsWith("64:ff9b::") => true,
                AddressFamily.InterNetworkV6 when ip.ToString().StartsWith("100::") => true,
                AddressFamily.InterNetworkV6 when ip.ToString().StartsWith("2001::") => true,
                AddressFamily.InterNetworkV6 when ip.ToString().StartsWith("2001:2") => true,
                AddressFamily.InterNetworkV6 when ip.ToString().StartsWith("2001:db8:") => true,
                AddressFamily.InterNetworkV6 when ip.ToString().StartsWith("2002:") => true,
                AddressFamily.InterNetworkV6 when ip.ToString().StartsWith("fc") => true,
                AddressFamily.InterNetworkV6 when ip.ToString().StartsWith("fd") => true,
                AddressFamily.InterNetworkV6 when ip.ToString().StartsWith("fe") => true,
                AddressFamily.InterNetworkV6 when bytes[0] == 255 => true,
                _ => false
            };
        }

        /// <summary>
        /// 邮件
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static bool IsEmail(this string instance)
        {
            Regex regexExpression = new Regex(ExtRegularString.Email, RegexOptions.Singleline | RegexOptions.CultureInvariant | RegexOptions.Compiled);
            return !string.IsNullOrWhiteSpace(instance) && regexExpression.IsMatch(instance);
        }

        /// <summary>
        /// 网址
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static bool IsUrl(this string instance)
        {
            Regex regexExpression = new Regex(ExtRegularString.Url, RegexOptions.Singleline | RegexOptions.CultureInvariant | RegexOptions.Compiled);
            return !string.IsNullOrWhiteSpace(instance) && regexExpression.IsMatch(instance);
        }

        /// <summary>
        /// 中文
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static bool IsChinese(this string instance)
        {
            Regex regexExpression = new Regex(ExtRegularString.Chinese, RegexOptions.Singleline | RegexOptions.CultureInvariant | RegexOptions.Compiled);
            return !string.IsNullOrWhiteSpace(instance) && regexExpression.IsMatch(instance);
        }

        /// <summary>
        /// HTML
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static bool IsHtml(this string instance)
        {
            Regex regexExpression = new Regex(ExtRegularString.Html, RegexOptions.Singleline | RegexOptions.CultureInvariant | RegexOptions.Compiled);
            return !string.IsNullOrWhiteSpace(instance) && regexExpression.IsMatch(instance);
        }

        /// <summary>
        /// 用户名
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static bool IsUserName(this string instance)
        {
            Regex regexExpression = new Regex(ExtRegularString.UserName, RegexOptions.Singleline | RegexOptions.CultureInvariant | RegexOptions.Compiled);
            return !string.IsNullOrWhiteSpace(instance) && regexExpression.IsMatch(instance);
        }

        /// <summary>
        /// 固定电话
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static bool IsChinesePhone(this string instance)
        {
            Regex regexExpression = new Regex(ExtRegularString.ChinesePhone, RegexOptions.Singleline | RegexOptions.CultureInvariant | RegexOptions.Compiled);
            return !string.IsNullOrWhiteSpace(instance) && regexExpression.IsMatch(instance);
        }

        /// <summary>
        /// 手机号
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static bool IsChineseMobile(this string instance)
        {
            Regex regexExpression = new Regex(ExtRegularString.ChineseMobile, RegexOptions.Singleline | RegexOptions.CultureInvariant | RegexOptions.Compiled);
            return !string.IsNullOrWhiteSpace(instance) && regexExpression.IsMatch(instance);
        }

        /// <summary>
        /// 邮政编码
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static bool IsZipCode(this string instance)
        {
            Regex regexExpression = new Regex(ExtRegularString.ZipCode, RegexOptions.Singleline | RegexOptions.CultureInvariant | RegexOptions.Compiled);
            return !string.IsNullOrWhiteSpace(instance) && regexExpression.IsMatch(instance);
        }

        /// <summary>
        /// 身份证
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static bool IsCardID(this string instance)
        {
            Regex regexExpression = new Regex(ExtRegularString.CardID, RegexOptions.Singleline | RegexOptions.CultureInvariant | RegexOptions.Compiled);
            return !string.IsNullOrWhiteSpace(instance) && regexExpression.IsMatch(instance);
        }

        /// <summary>
        /// 军官证
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static bool IsOfficer(this string instance)
        {
            Regex regexExpression = new Regex(ExtRegularString.Officer, RegexOptions.Singleline | RegexOptions.CultureInvariant | RegexOptions.Compiled);
            return !string.IsNullOrWhiteSpace(instance) && regexExpression.IsMatch(instance);
        }

        /// <summary>
        /// 护照
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static bool IsPassPort(this string instance)
        {
            Regex regexExpression = new Regex(ExtRegularString.PassPort, RegexOptions.Singleline | RegexOptions.CultureInvariant | RegexOptions.Compiled);
            return !string.IsNullOrWhiteSpace(instance) && regexExpression.IsMatch(instance);
        }

        /// <summary>
        /// 营业执照
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static bool IsBusiness(this string instance)
        {
            Regex regexExpression = new Regex(ExtRegularString.Business, RegexOptions.Singleline | RegexOptions.CultureInvariant | RegexOptions.Compiled);
            return !string.IsNullOrWhiteSpace(instance) && regexExpression.IsMatch(instance);
        }

        /// <summary>
        /// 驾照
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static bool IsDriving(this string instance)
        {
            Regex regexExpression = new Regex(ExtRegularString.Driving, RegexOptions.Singleline | RegexOptions.CultureInvariant | RegexOptions.Compiled);
            return !string.IsNullOrWhiteSpace(instance) && regexExpression.IsMatch(instance);
        }

        /// <summary>
        /// 组织机构代码证
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static bool IsOrganization(this string instance)
        {
            Regex regexExpression = new Regex(ExtRegularString.Organization, RegexOptions.Singleline | RegexOptions.CultureInvariant | RegexOptions.Compiled);
            return !string.IsNullOrWhiteSpace(instance) && regexExpression.IsMatch(instance);
        }

        /// <summary>
        /// 台胞证
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static bool IsTaiwanOrgan(this string instance)
        {
            Regex regexExpression = new Regex(ExtRegularString.TaiwanOrgan, RegexOptions.Singleline | RegexOptions.CultureInvariant | RegexOptions.Compiled);
            return !string.IsNullOrWhiteSpace(instance) && regexExpression.IsMatch(instance);
        }

        /// <summary>
        /// 港澳通行证
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static bool IsHongPass(this string instance)
        {
            Regex regexExpression = new Regex(ExtRegularString.HongPass, RegexOptions.Singleline | RegexOptions.CultureInvariant | RegexOptions.Compiled);
            return !string.IsNullOrWhiteSpace(instance) && regexExpression.IsMatch(instance);
        }

        /// <summary>
        /// 车牌号
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static bool IsLicense(this string instance)
        {
            Regex regexExpression = new Regex(ExtRegularString.License, RegexOptions.Singleline | RegexOptions.CultureInvariant | RegexOptions.Compiled);
            return !string.IsNullOrWhiteSpace(instance) && regexExpression.IsMatch(instance);
        }

        /// <summary>
        /// ICP备案号
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static bool IsICP(this string instance)
        {
            Regex regexExpression = new Regex(ExtRegularString.ICP, RegexOptions.Singleline | RegexOptions.CultureInvariant | RegexOptions.Compiled);
            return !string.IsNullOrWhiteSpace(instance) && regexExpression.IsMatch(instance);
        }

        /// <summary>
        /// 银联付款码
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static bool IsUnionPayCode(this string instance)
        {
            Regex regexExpression = new Regex(ExtRegularString.UnionPayCode, RegexOptions.Singleline | RegexOptions.CultureInvariant | RegexOptions.Compiled);
            return !string.IsNullOrWhiteSpace(instance) && regexExpression.IsMatch(instance);
        }

        /// <summary>
        /// 护士资格证编号
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static bool IsNurseCode(this string instance)
        {
            Regex regexExpression = new Regex(ExtRegularString.NurseCode, RegexOptions.Singleline | RegexOptions.CultureInvariant | RegexOptions.Compiled);
            return !string.IsNullOrWhiteSpace(instance) && regexExpression.IsMatch(instance);
        }

        /// <summary>
        /// 学位证书
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static bool IsAcademic(this string instance)
        {
            Regex regexExpression = new Regex(ExtRegularString.Academic, RegexOptions.Singleline | RegexOptions.CultureInvariant | RegexOptions.Compiled);
            return !string.IsNullOrWhiteSpace(instance) && regexExpression.IsMatch(instance);
        }

        /// <summary>
        /// 微信消息模板id（template_id）
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static bool IsWeChatMsg(this string instance)
        {
            Regex regexExpression = new Regex(ExtRegularString.WeChatMsg, RegexOptions.Singleline | RegexOptions.CultureInvariant | RegexOptions.Compiled);
            return !string.IsNullOrWhiteSpace(instance) && regexExpression.IsMatch(instance);
        }

        /// <summary>
        /// IP地址
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static bool IsIpAddress(this string instance)
        {
            Regex regexExpression4 = new Regex(ExtRegularString.Ip4Address, RegexOptions.Singleline | RegexOptions.CultureInvariant | RegexOptions.Compiled);
            Regex regexExpression6 = new Regex(ExtRegularString.Ip6Address, RegexOptions.Singleline | RegexOptions.CultureInvariant | RegexOptions.Compiled);
            return !string.IsNullOrWhiteSpace(instance) && (regexExpression4.IsMatch(instance) || regexExpression6.IsMatch(instance));
        }


        /// <summary>
        /// 截取字符串列表
        /// </summary>
        /// <param name="input"></param>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static List<string> ExtractStringBetweenBeginAndEnd(this string input, string begin, string end)
        {
            List<string> list_match = new List<string>();
            if (string.IsNullOrEmpty(input))
            {
                return list_match;
            }
            Regex regex = new Regex("(?<=(" + begin + "))[.\\s\\S]*?(?=(" + end + "))", RegexOptions.Multiline | RegexOptions.Singleline);
            foreach (Match match in regex.Matches(input))
            {
                if (match != null && !string.IsNullOrEmpty(match.Value))
                {
                    list_match.Add(match.Value);
                }
            }
            return list_match;
        }


        /// <summary>
        /// 截取字符串
        /// </summary>
        /// <param name="input"></param>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static string ExtractStringBetweenBeginAndEndString(this string input, string begin, string end)
        {
            if (string.IsNullOrEmpty(input))
            {
                return "";
            }
            Regex regex = new Regex("(?<=(" + begin + "))[.\\s\\S]*?(?=(" + end + "))", RegexOptions.Multiline | RegexOptions.Singleline);
            foreach (Match match in regex.Matches(input))
            {
                if (match != null && !string.IsNullOrEmpty(match.Value))
                {
                    return match.Value;
                }
            }
            return "";
        }

        #endregion

        #region 类型转换

        /// <summary>
        /// 生成真正的随机数
        /// </summary>
        /// <param name="r"></param>
        /// <param name="seed"></param>
        /// <returns></returns>
        public static int ToStrictNext(this Random r, int seed = int.MaxValue)
        {
            return new Random((int)Stopwatch.GetTimestamp()).Next(seed);
        }

        /// <summary>
        /// 转布尔
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private static bool ToBool(this string data)
        {
            switch (data.ToString().Trim().ToLower())
            {
                case "0":
                    return false;
                case "1":
                    return true;
                case "是":
                    return true;
                case "否":
                    return false;
                case "yes":
                    return true;
                case "no":
                    return false;
                case "开启":
                    return true;
                case "禁用":
                    return false;
                default:
                    return false;
            }
        }

        /// <summary>
        /// 字符串转时间，失败放回当前时间
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static DateTime ToDateTime(this string value)
        {
            if (DateTime.TryParse(value, out DateTime dt))
            {
                return dt;
            }
            return DateTime.Now;
        }

        /// <summary>
        /// 字符串转Unicode
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToUnicode(this string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return value;
            StringBuilder strResult = new StringBuilder();
            if (!string.IsNullOrEmpty(value))
            {
                for (int i = 0; i < value.Length; i++)
                {
                    strResult.Append("\\u");
                    strResult.Append(((int)value[i]).ToString("x4"));
                }
            }
            return strResult.ToString();
        }

        /// <summary>
        /// Unicode转字符串
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToUnicodeToString(this string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return value;
            return Regex.Unescape(value);
        }

        /// <summary>
        /// 对象转json
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToJson(this object value)
        {
            if (value == null)
            {
                return "";
            }
            return JsonConvert.SerializeObject(value);
        }

        /// <summary>
        /// json转对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T JsonToObject<T>(this string value)
        {
            if (value.IsEmpty())
            {
                return default(T);
            }
            return JsonConvert.DeserializeObject<T>(value);
        }

        /// <summary>
        /// 转整型
        /// </summary>
        /// <param name="value"></param>
        /// <param name="Default"></param>
        /// <returns></returns>
        public static int ToInt(this string value, int Default = 0)
        {
            int.TryParse(value, out Default);
            return Default;
        }

        /// <summary>
        /// 转长整型
        /// </summary>
        /// <param name="value"></param>
        /// <param name="Default"></param>
        /// <returns></returns>
        public static long ToLong(this string value, long Default = 0)
        {
            long.TryParse(value, out Default);
            return Default;
        }

        /// <summary>
        /// 转double
        /// </summary>
        /// <param name="value"></param>
        /// <param name="Default"></param>
        /// <returns></returns>
        public static double ToDouble(this string value, double Default = 0)
        {
            double.TryParse(value, out Default);
            return Default;
        }

        /// <summary>
        /// 转decimal
        /// </summary>
        /// <param name="value"></param>
        /// <param name="Default"></param>
        /// <returns></returns>
        public static decimal ToDecimal(this string value, decimal Default = 0)
        {
            decimal.TryParse(value, out Default);
            return Default;
        }

        /// <summary>
        /// 转float
        /// </summary>
        /// <param name="value"></param>
        /// <param name="Default"></param>
        /// <returns></returns>
        public static float ToFloat(this string value, float Default = 0)
        {
            float.TryParse(value, out Default);
            return Default;
        }

        /// <summary>
        /// 指定小数点
        /// </summary>
        /// <param name="d"></param>
        /// <param name="s"></param>
        /// <returns></returns>
        public static decimal ToFixed(this object value, int s)
        {
            decimal d = Convert.ToDecimal(value);
            return decimal.Round(d, s);
        }


        /// <summary>
        /// 字符串转性别
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int ToSexInt(this string value)
        {
            int sex = 2;
            switch (value)
            {
                case "男":
                    sex = 1;
                    break;
                case "女":
                    sex = 0;
                    break;
                case "未知":
                    sex = 2;
                    break;
                default:
                    sex = 2;
                    break;
            }
            return sex;
        }

        /// <summary>
        /// 性别转字符串
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToSex(this int value)
        {
            string sex = "未知";
            switch (value)
            {
                case 1:
                    sex = "男";
                    break;
                case 0:
                    sex = "女";
                    break;
                case 2:
                    sex = "未知";
                    break;
                default:
                    sex = "未知";
                    break;
            }
            return sex;
        }
          
        /// <summary>
        /// 数字转字符串
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static string ToNumberText(this int number)
        {
            if (number < 10000)
            {
                return number.ToString();
            }
            else if (number >= 10000 && number < 100000000)
            {
                return decimal.Round(Convert.ToDecimal(Convert.ToDecimal(number) / Convert.ToDecimal(10000)), 2).ToString() + "万";
            }
            else
            {
                return decimal.Round(Convert.ToDecimal(Convert.ToDecimal(number) / Convert.ToDecimal(100000000)), 2).ToString() + "亿";
            }
        }

        /// <summary>
        /// 中文字体简体繁体转换
        /// </summary>
        /// <param name="text">文字</param>
        /// <param name="format">1：转简体  2：转繁体</param>
        /// <returns></returns>
        public static string ToChinese(this string text, int format = 1)
        {
            /// <summary>
            /// 简体字
            /// </summary>
            string Simplified = "一丁丂七丄丅丆万丈三上下丌不与丏丐丑丒专且丕世丗丘丙业丛东丝丞丢丠両丢丣两严并丧丨丩个丫丬中丮丯丰丱串丳临丵丶丷丸丹为主丼丽举丿乀乁乂乃乄久乆乇么义乊之乌乍乎乏乐乑乒乓乔乕乖乗乘乙乚乛乜九乞也习乡乢乣乤乥书乧乨乩乪乫乬乭乮乯买乱乲乳乴乵乶乷乸乹乺乻乼乽乾乿亀亁乱亃亄亅了亇予争亊事二亍于亏亐云互亓五井亖亗亘亘亚些亜亝亚亟亠亡亢亣交亥亦产亨亩亪享京亭亮亯亰亱亲亳亴亵亶亷亸亹人亻亼亽亾亿什仁仂仃仄仅仆仇仈仉今介仌仍从仏仐仑仒仓仔仕他仗付仙仚仛仜仝仞仟仠仡仢代令以仦仧仨仩仪仫们仭仮仯仰仱仲仳仴仵件价仸仹仺任仼份仾仿伀企伂伃伄伅伆伇伈伉伊汲伌伍伎伏伐休伒伓伔夫伖众优伙会伛伜伝伞伟传伡伢伣伤伥伦伧伨伩伪伫伬伭伮伯估伱伲伳伴伵伶伷伸伹伺伻似伽伾伿佀佁佂佃佄佅但伫布佉佊佋佌位低住佐佑佒体占何佖佗佘余佚佛作佝佞佟你佡佢佣佤佥佦佧佨佩徊佫佬佭佮佯佰佱佲佳佴并佶佷佸佹佺佻佼佽佾使侀侁侂侃侄侅来侇侈侉侊例侌侍侎侏侐侑侒侓侔侕仑侗侘侙侚供侜依侞侟侠価侢侣侤侥侦侧侨侩侪侫侬侭侮侯侰侱侲侳侴侵侣局侸侹侺侻侼侽侾便俀俣系促俄俅俆俇俈俉俊俋俌俍俎俏俐俑俒俓俔俕俖俗俘俙俚俛俜保俞俟侠信俢俣俤俥俦俧俨俩俪俫俬俭修俯俰俱俲俳俴俵俶俷俸俹俺俻俼俽俾俿伥倁倂倃倄倅俩倇倈仓倊个倌倍倎倏倐们倒倓倔倕幸倗倘候倚倛倜倝倞借倠倡倢仿値倥倦倧倨倩倪伦倬倭倮倯倰倱倲倳倴倵倶倷倸倹债倻值倽倾倿偀偁偂偃偄偅偆假偈伟偊偋偌偍偎偏偐偑偒偓偔偕偖偗偘偙做偛停偝偞偟偠偡偢偣偤健偦偧偨偩偪偫偬偭偮偯偰偱偲偳侧侦偶偷偸偹咱偻偼伪偾偿傀傁傂傃傄傅傆傇傈傉傊傋傌傍傎傏傐杰傒傓傔傕伧傗伞备傚傛傜傝傞傟傠傡家傣傤傥傦傧储傩傪傫催佣傮偬傰傱傲传伛债傶伤傸傹傺傻傼傽倾傿僀僁偻僃僄仅僆戮僈佥僊僋僌働僎像僐侨僒僓僔仆僖僗僘僙僚僛僜僝僞僟僠僡僢僣僤侥僦僧偾僩僪僫僬僭僮僯僰雇僲僳僴僵僶僷僸价僺僻僼僽僾僿仪儁侬儃亿当儆儇侩俭儊儋儌儍儎儏傧儑儒儓俦侪儖儗尽儙儚儛儜儝儞偿儠儡儢儣儤儥儦儧儨儩优儫儬儭儮儯儰儱储儳儴儵儶俪罗儹傩傥俨儽儾儿兀允兂元兄充兆凶先光兊克兑免兎兏児兑儿兓兔兕兖兖兘兙党兛兜兝兞兟兠兡兢兣兤入兦内全两兪八公六兮兯兰共兲关兴兵其具典兹兺养兼兽兾兿冀冁冂冃冄内円冇冈冉册冋册再冎冏冐胄冒冓冔冕冖冗冘写冚军农冝冞冟冠冡冢冣冤冥冦冧冨冩幂冫冬冭冮冯冰冱冲决冴况冶冷冸冹冺冻冼冽冾冿净凁凂凃凄凅准凇净凉凊凋凌冻凎减凐凑凒凓凔凕凖凗凘凙凚凛凛凝凞凟几凡凢凣凤凥処凧凨凩凪凫凬凭凮凯凰凯凲凳凴凵凶凷凸凹出击凼函凾凿刀刁刂刃刄刅分切刈刉刊刋刌刍刎刏刐刑划刓刔刕刖列刘则刚创刜初刞刟删刡刢刣判别刦刧刨利删别刬刭刮刯到刱刲刳刴刵制刷券刹刺刻刼刽刾刿剀剁剂剃刭剅剆则剈剉削克剌前刹剏剐剑剒剓剔剕剖剗剘剙剚刚剜剥剞剟剠剡剢剣剤剥剦剧剨剩剪剫剬剭剐副剰剱割剳剀创剶铲剸剹剺剻剼剽剾剿劀劁劂划劄劅劆剧劈刘刽劋刿剑劎劏劐剂劒劓劔劕劖劗劘劙劚力劜劝办功加务劢劣劤劥劦劧动助努劫劬劭劮劯劰励劲劳労劵劶劷劸効劺匡劼劽劾势勀劲勂勃勄勅勆勇勈勉勊勋勌勍勎勏勐勑勒勓勔动勖勖勘务勚勋勜胜劳募勠勡势积勤勥剿勧勨勩勪勫勬勭勮勯勰劢勲勳勴励勶勷劝勹勺匀勼勽勾勿匀匁匂匃匄包匆匇匈匉匊匋匌匍匎匏匐匑匒匓匔匕化北匘匙匚匛匜匝匞匟匠匡匢匣匤匥匦匧匨匩匪匫匬匦匮汇匰匮匲匳匴匵匶匷匸匹区医匼匽匾匿区十卂千卄卅卆升午卉半卋卌卍华协卐卑卒卓协单卖南単卙博卛卜卝卞卟占卡卢卣卤卥卦卧卨卩卪卫卬卭卮卯印危卲即却卵卶卷卸恤卺却卼卽卾卿厀厁厂厃厄厅历厇厈厉厊压厌厍厎厏厐厑厒厓厔厕厖厗厘厍厚厛厜厝厞原厠厡厢厣厤厥厦厧厨厩厪厫厬厌厮厯厰厱厉厳厣厵厶厷厸厹厺去厼厽厾县叀叁参参叄叅叆叇又叉及友双反収叏叐发叒叓叔叕取受变叙叚叛叜叝叞叟叠叡丛口古句另叧叨叩只叫召叭叮可台叱史右叴叵叶号司叹叺叻叼叽叾叿吀吁吂吃各吅吆吇合吉吊寸同名后吏吐向吒吓吔吕吖吗吘吙吚君吜吝吞吟吠吡吢吣吤吥否吧吨吩吪含听吭吮启吰吱吲吴吴吵呐吷吸吹吺吻吼吽吾吿呀呁吕呃呄呅呆呇呈呉告呋呌呍尺呏呐呑呒呓呔呕呖呗员呙呚呛呜呝呞呟呠呡呢呣呤呥呦呧周呩呪呫呬呭呮呯呰呱呲味呴呵呶呷呸呹呺呻呼命呾呿咀咁咂咃咄咅咆咇咈咉咊咋和咍咎咏咐咑咒咓咔咕咖咗咘咙咚咛咜咝咞咟咠咡咢咣咤咥咦咧咨咩咪咫咬咭咮咯咰咱咲咳咴咵咶咷咸咹咺咻呙咽咾咿哀品哂哃哄哅哆哇哈哉哊哋哌响哎哏哐哑哒哓哔哕哖哗哘哙哚哛哜哝哞哟哠员哢哣哤哥哦哧哨哩哪哫哬哭哮哯哰哱哲哳哴哵哶哷哸哹哺哻哼哽哾哿唀唁唂唃呗唅唆唇唈唉唊唋唌唍唎唏唐唑唒唓唔唕唖唗唘唙唚唛唜唝唞唟唠唡唢唣唤唥唦唧唨唩唪唫唬唭售唯唰唱唲唳唴唵唶唷念唹唺唻唼唽唾唿啀啁啂啃啄啅商啇啈啉啊啋啌啍啎问啐啑啒啓啔啕啖啗啘啙啚啛啜啝哑启啠啡啢衔啤啥啦啧啨啩啪啫啬啭啮啯啰啱啲啳啴啵啶啷啸啹啺啻啼啽啾啿喀喁喂喃善喅喆喇喈喉喊喋喌喍喎喏喐喑喒喓喔喕喖喗喘喙唤喛喜喝喞喟喠喡喢喣喤喥喦喧喨喩丧吃乔喭单喯喰喱哟喳喴喵営喷喸喹喺喻喼喽喾喿嗀嗁嗂嗃嗄嗅呛啬嗈嗉嗊嗋嗌嗍吗嗏嗐嗑嗒嗓嗔嗕嗖嗗嗘嗙呜嗛嗜嗝嗞嗟嗠嗡嗢嗣嗤嗥嗦嗧嗨唢嗪嗫嗬嗭嗮嗯嗰嗱嗲嗳嗴嗵哔嗷嗸嗹嗺嗻嗼嗽嗾嗿嘀嘁嘂嘃嘄嘅叹嘇嘈嘉嘊嘋嘌喽嘎嘏嘐嘑嘒嘓呕嘕啧尝嘘嘙嘚嘛唛嘝嘞嘟嘠嘡嘢嘣嘤嘥嘦嘧嘨哗嘪嘫嘬嘭唠啸叽嘱嘲嘳嘴哓嘶嘷呒嘹嘺嘻嘼嘽嘾嘿噀恶噂噃噄噅噆噇噈噉噊噋噌噍噎噏噐噑噒嘘噔噕噖噗噘噙噚噛噜噝噞噟哒噡噢噣噤哝哕噧器噩噪噫噬噭噮嗳噰噱哙噳喷噵噶噷吨当噺噻噼噽噾噿咛嚁嚂嚃嚄嚅嚆吓嚈嚉嚊嚋哜嚍嚎嚏嚐嚑嚒嚓嚔噜嚖嚗嚘啮嚚嚛嚜嚝嚞嚟嚠嚡嚢嚣嚤咽呖嚧咙嚩嚪嚫嚬嚭向嚯嚰嚱嚲喾严嚵嘤嚷嚸嚹嚺嚻嚼嚽嚾嚿啭嗫嚣囃囄冁囆囇呓罗囊囋苏囍囎囏囐嘱囒囓囔囕囖囗囘囙囚四囜囝回囟因囡团団囤囥囦囧囨囩囱囫囬园囮囯困囱囲図围囵囶囷囸囹固囻囼国图囿圀圁圂圃圄圅圆囵圈圉圊国圌围圎圏圐圑园圆圔圕图圗团圙圚圛圜圝圞土圠圡圢圣圤圥圦圧在圩圪圫圬圭圮圯地圱圲圳圴圵圶圷圸圹场圻圼圽圾圿址坁坂坃坄坅坆均坈坉坊坋坌坍坎坏坐坑坒坓坔坕坖块坘坙坚坛坜坝坞坟坠坡坢坣坤坥坦坧坨坩坪坫坬坭坮坯垧坱坲坳坴坵坶坷坸坹坺坻坼坽坾坿垀垁垂垃垄垅垆垇垈垉垊型垌垍垎垏垐垑垒垓垔垕垖垗垘垙垚垛垜垝垞垟垠垡垢垣垤垥垦垧垨垩垪垫垬垭垮垯垰垱垲垳垴垵垶垷垸垹垺垻垼垽垾垿埀埁埂埃埄埅埆埇埈埉埊埋埌埍城埏埐埑埒埓埔埕埖埗埘埙埚埛埜埝埞域埠垭埢埣埤埥埦埧埨埩埪埫埬埭埮埯埰埱埲埳埴埵埶执埸培基埻埼埽埾埿堀堁堂堃堄坚堆堇堈堉垩堋堌堍堎堏堐堑堒堓堔堕堖堗堘堙堚堛堜埚堞堟堠堡堢堣堤堥堦堧堨堩堪堫堬堭堮尧堰报堲堳场堵堶堷堸堹堺堻堼堽堾碱塀塁塂塃塄塅塆塇塈塉块茔塌塍塎垲塐塑埘塓塔塕塖涂塘塙塚塛塜塝塞塟塠塡坞塣埙塥塦塧塨塩塪填塬塭塮塯塰塱塲塳塴尘塶塷塸堑塺塻塼塽塾塿墀墁墂境墄墅墆墇墈墉垫墋墌墍墎墏墐墒墒墓墔墕墖増墘墙墚墛坠墝增墟墠墡墢墣墤墥墦墧墨墩墪墫墬墭堕墯墰墱墲坟墴墵墶墷墸墹墺墙墼墽垦墿壀壁壂壃壄壅壆坛壈壉壊壋壌壍埙壏壐壑壒压壔壕壖壗垒圹垆壛壜壝坏垄壠壡坜壣壤壥壦壧壨坝壪士壬壭壮壮声壱売壳壴壵壶壷壸壹壶壻壼寿壾壿夀夁夂夃处夅夆备夈変夊夋夌复夎夏夐夑夒夓夔夕外夗夘夙多夛夜夝夞够够夡梦夣夤夥夦大夨天太夫夬夭央夯夰失夲夳头夵夶夷夸夹夺夻夼夽夹夿奀奁奂奃奄奅奆奇奈奉奊奋奌奍奎奏奂契奒奓奔奕奖套奘奙奚奛奜奝奞奟奠奡奢奣奤奥奦奥奨奁夺奫奬奭奋奯奰奱奲女奴奵奶奷奸她奺奻奼好奾奿妀妁如妃妄妅妆妇妈妉妊妋妌妍妎妏妐妑妒妓妔妕妖妗妘妙妚妛妜妆妞妟妠妡妢妣妤妥妦妧妨妩妪妫妬妭妮妯妰妱妲你妴妵妶妷妸妹妺妻妼妽妾妿姀姁姂姃姄姅姆姇姈姉姊始姌姗姎姏姐姑姒姓委姕姖姗姘姙姚姛姜姝姞姟姠姡姢姣姤姥奸姧姨姩侄姫姬姭姮姯姰姱姲姳姴姵姶姷姸姹姺姻姼姽姾姿娀威娂娃娄娅娆娇娈娉娊娋娌娍娎娏娐娑娒娓娔娕娖娗娘娙娚娱娜娝娞娟娠娡娢娣娤娥娦娧娨娩娪娫娬娭娮娯娰娱娲娳娴娵娶娷娸娹娺娻娼娽娾娿婀娄婂婃婄婅婆婇婈婉婊婋婌婍婎婏婐婑婒婓婔婕婖婗婘婙婚婛婜婝婞婟婠婡婢婣婤婥妇婧婨婩婪婫婬娅婮婯婰婱婲婳婴婵婶婷婸婹婺婻婼婽婾婿媀媁媂媃媄媅媆媇媈媉媊媋媌媍媎媏媐媑媒媓媔媕媖媗媘媙媚媛媜媝媞媟媠媡媢媣媤媥媦娲媨媩媪媫媬媭媮妫媰媱媲媳媴媵媶媷媸媹媺媻媪妈媾媿嫀嫁嫂嫃嫄嫅嫆嫇嫈嫉嫊嫋嫌嫍嫎嫏嫐嫑嫒嫓嫔嫕嫖妪嫘嫙嫚嫛嫜嫝嫞嫟嫠嫡嫢嫣嫤嫥嫦嫧嫨嫩嫪嫫嫬嫭嫮嫯嫰嫱嫲嫳嫴妩嫶嫷嫸嫹嫺娴嫼嫽嫾嫿嬀嬁嬂嬃嬄嬅嬆嬇娆嬉嬊婵娇嬍嬎嬏嬐嬑嬒嬓嬔嬕嬖嬗嬘嫱嬚嬛嬜袅嬞嬟嬠嫒嬢嬣嬷嬥嬦嬧嬨嬩嫔嬫嬬嬭嬮嬯婴嬱嬲嬳嬴嬵嬶嬷婶嬹嬺嬻嬼嬽嬾嬿孀孁孂娘孄孅孆孇孈孉孊孋娈孍孎孏子孑孒孓孔孕孖字存孙孚孛孜孝孞孟孠孡孢季孤孥学孧孨孩孪孙孬孭孮孯孰孱孲孳孴孵孶孷学孹孺孻孼孽孾孪宀宁宂它宄宅宆宇守安宊宋完宍宎宏宐宑宒宓宔宕宖宗官宙定宛宜宝实実宠审客宣室宥宦宧宨宩宪宫宬宭宫宯宰宱宲害宴宵家宷宸容宺宻宼宽宾宿寀寁寂寃寄寅密寇寈寉寊寋富寍寎寏寐寑寒寓寔寕寖寗寘寙寚寛寜寝寞察寠寡寝寣寤寥实宁寨审寪写宽寭寮寯寰寱寲寳寴宠宝寷寸对寺寻导寽対寿尀封専尃射尅将将专尉尊寻尌对导小尐少尒尓尔尕尖尗尘尙尚尛尜尝尞尟尠尡尢尣尤尥尦尧尨尩尪尫尬尭尮尯尰就尲尳尴尵尶尴尸尹尺尻尼尽尾尿局屁层屃屄居届屇屈屉届屋屌屍屎屏屐屑屒屓屔展屖屗屘屙屚屛屉扉属屟屠屡屡屣层履屦屧屦屩屪屫属屭屮屯屰山屲屳屴屵屶屷屸屹屺屻屼屽屾屿岀岁岂岃岄岅岆岇岈岉岊岋岌岍岎岏岐岑岒岓岔岕岖岗岘岙岚岛岜岝岞岟岠冈岢岣岤岥岦迢岨岩岪岫岬岭岮岯岰岱岲岳岴岵岶岷岸岹岺岻岼岽岾岿峀峁峂峃峄峅峆峇峈峉峊峋峌峍峎峏峐峑峒峓峔峕峖峗峘峙峚峛峜峝峞峟峠峡峢峣峤峥峦峧峨峩峪峫峬峭峮峯峰峱峲峳岘峵岛峷峸峹峺峻峼峡峾峿崀崁崂崃崄崅崆崇崈崉崊崋崌崃崎崏崐崑崒崓崔崕崖岗崘仑崚崛崜崝崞崟岽崡峥崣崤崥崦崧崨崩崪崫崬崭崮崯崰崱崲嵛崴崵崶崷崸崹崺崻崼崽崾崿嵀嵁嵂嵃嵄嵅嵆嵇嵈嵉嵊嵋嵌嵍嵎嵏岚嵑岩嵓嵔嵕嵖嵗嵘嵙嵚嵛嵜嵝嵞嵟嵠嵡嵢嵣嵤嵥嵦嵧嵨嵩嵪嵫嵬嵭嵮嵯嵰嵱嵲嵳嵴嵵嵶嵷嵸嵹嵺嵻嵼嵽嵾嵿嶀嵝嶂嶃崭嶅嶆岖嶈嶉嶊嶋嶌嶍嶎嶏嶐嶑嶒嶓嶔嶕嶖崂嶘嶙嶚嶛嶜嶝嶞嶟峤嶡嶢嶣嶤嶥嶦峄嶨嶩嶪嶫嶬嶭嶮嶯嶰嶱嶲嶳嶴嶵嶶嶷嵘嶹岭嶻屿岳嶾嶿巀巁巂巃巄巅巆巇巈巉巊岿巌巍巎巏巐漓峦巓巅巕岩巗巘巙巚巛巜川州巟巠巡巢巣巤工左巧巨巩巪巫巬巭差巯巯己已巳巴巵巶巷巸卺巺巻巼巽巾巿帀币市布帄帅帆帇师帉帊帋希帍帎帏帐帑帒帓帔帕帖帗帘帙帚帛帜帝帞帟帠帡帢帣帤帅带帧帨帩帪师帬席帮帯帰帱帲帐帴帵带帷常帹帺帻帼帽帾帿帧幁幂帏幄幅幆幇幈幉幊幋幌幍幎幏幐幑幒幓幔幕幖帼帻幙幚幛幜幝幞帜幠幡幢币幤幥幦幧幨幩幪帮帱幭幮幯幰幱干平年幵并幷幸干幺幻幼幽几广庀庁仄広庄庅庆庇庈庉床庋庌庍庎序庐庑庒库应底庖店庘庙庚庛府庝庞废庠庡庢庣庤庥度座庨庩庪库庬庭庮庯庰庱庲庳庴庵庶康庸庹庺庻庼庽庾庿廀厕厢廃厩廅廆廇厦廉廊廋廌廍廎廏廐廑廒廓廔廕廖廗廘廙厨廛廜厮廞庙厂庑废广廤廥廦廧廨廪廪廫庐廭廮廯廰痈廲厅廴廵延廷廸廹建廻廼廽廾廿开弁异弃弄弅弆弇弈弉弊弋弌弍弎式弐弑弑弓吊引弖弗弘弙弚弛弜弝弞弟张弡弢弣弤弥弦弧弨弩弪弫弬弭弮弯弰弱弲弪弴张弶强弸弹强弻弼弽弾弿彀彁彂彃彄彅别彇弹彉强彋弥彍弯彏彐彑归当彔录彖彗彘汇彚彛彜彝彞彟彠彡形彣彤彦彦彧彨彩彪雕彬彭彮彯彰影彲彳彴彵彶彷彸役彺彻彼彽彾佛往征徂徃径待徆徇很徉徊律後徍徎徏徐径徒従徔徕徖得徘徙徚徛徜徝从徟徕御徢徣徤徥徦徧徨复循徫旁徭微徯徰徱徲徳徴徵徶德徸彻徺徻徼徽徾徿忀忁忂心忄必忆忇忈忉忊忋忌忍忎忏忐忑忒忓忔忕忖志忘忙忚忛応忝忞忟忠忡忢忣忤忥忦忧忨忩忪快忬忭忮忯忰忱忲忳忴念忶忷忸忹忺忻忼忽忾忿怀态怂怃怄怅怆怇怈怉怊怋怌怍怎怏怐怑怒怓怔怕怖怗怘怙怚怛怜思怞怟怠怡怢怣怤急怦性怨怩怪怫怬怭怮怯怰怱怲怳怴怵怶怷怸怹怺总怼怽怾怿恀恁恂恃恄恅恒恇恈恉恊恋恌恍恎恏恐恑恒恓恔恕恖恗恘恙恚恛恜恝恞恟恠恡恢恣恤耻恦恧恨恩恪恫恬恭恮息恰恱恲恳恴恵恶恷恸恹恺恻恼恽恾恿悀悁悂悃悄悦悆悇悈悉悊悋悌悍悎悏悐悑悒悓悔悕悖悗悘悙悚悛悜悝悞悟悠悡悢患悤悥悦悧您悩悪悫悬悭悮悯悰悱悲悳悴怅闷悷悸悹悺悻悼凄悾悿惀惁惂惃惄情惆惇惈惉惊惋惌惍惎惏惐惑惒惓惔惕惖惗惘惙惚惛惜惝惞惟惠恶惢惣惤惥惦惧惨惩惪惫惬惭惮惯惰恼恽想惴惵惶惷惸惹惺恻惼惽惾惿愀愁愂愃愄愅愆愇愈愉愊愋愌愍愎意愐愑愒愓愔愕愖愗愘愙愚爱惬愝愞感愠愡愢愣愤愥愦愧悫愩愪愫愬愭愮愯愰愱愲愳怆愵愶恺愸愹愺愻愼愽忾愿慀慁慂慃栗慅慆殷慈慉慊态慌愠慎慏慐慑慒慓慔慕慖慗惨慙惭慛慜慝慞恸慠慡慢惯慤慥慦慧慨慩怄怂慬慭虑慯慰慱慲悭慴慵庆慷慸慹慺慻戚慽慾慿憀憁忧憃憄憅憆憇憈憉惫憋憌憍憎憏怜凭愦憓憔憕憖憗憘憙惮憛憜憝憞憟憠憡憢憣愤憥憦憧憨憩憪悯憬憭怃憯憰憱宪憳憴憵忆憷憸憹憺憻憼憽憾憿懀懁懂懃懄懅懆恳懈应懊懋怿檩懎懏懐懑懒懓懔懕懖懗懘懙懚懛懜懝蒙怼懠懡懢懑懤懥懦懧恹懩懪懫懬懭懮懯懰懱惩懳懴懵懒怀悬懹忏懻惧懽慑懿恋戁戂戃戄戅戆戆戈钺戊戋戌戍戎戏成我戒戓戋戕或戗战戙戚戛戜戝戞戟戠戡戢戣戤戥戦戗戨戬截戫戬戭戮戯战戱戏戳戴戵户户戸戹戺戻戼戽戾房所扁扂扃扄扅扆扇扈扉扊手扌才扎扏仂扑扒打扔払扖扗托扙扚扛扜扝扞扟叉扡扦扣扤扥扦执扨扩扪扫扬扭扮扯扰扱扲扳扴扵扶扷扸批抵扻扼扽找承技抁抂拚抄抅擦抇抈抉把抋抌抍殒抏抐抑抒抓抔投抖抗折抙抚抛抜抝択抟抠抡抢抣护报抦抧抨抩抪披抬抭抮抯抰抱抲抳曳抵抶抷抸抹抺抻押抽抾抿拀拁拂拃拄担拆拇拈拉拊抛拌拍拎拏拐拑拒拓拔拕拖拗拘拙拚招拜拝拞拟拠拡拢拣拤拥拦拧拨择拪拫括拭拮拯拰拱拲拳拴拵拶拷拸拹拺拻拼拽拾拿挀持挂挃挄挅挆指挈按挊挋挌挍挎挏挐挑挒挓挔挕挖挗挘挙挚挛挜挝挞挟挠挡挢挣挤挥挦挧挨挩挪挫挬挭挮振挰挱挲挳挴挵局挷挸挹挺挻挼挽挟挿捀捁捂捃捄捅捆捇捈捉捊捋捌扞捎捏捐捑捒捓捔捕捖捗捘捙捚捛捜捝捞损捠捡换捣捤捥捦捧舍捩捪扪捬捭据捯捰捱卷捳捴捵捶捷捸捹捺捻捼捽捾捿掀掁掂扫抡掅掆掇授掉掊掋掌掍掎掏掐掑排掓掔掕掖掗掘挣掚挂掜掝掞掟掠采探掣掤接掦控推掩措掫掬掭掮掯掰掱掲掳掴掵掶掷掸掹掺掻掼掽掾掿拣揁揂揃揄揅揆揇揈揉揊揋揌揍揎描提揑插揓揔揕揖揗揘揙扬换揜揝揞揟揠握揢揣揤揥揦揧揨揩揪揫揬揭挥揯揰揱揲揳援揵揶揷揸背揺揻揼揽揾揿搀搁搂搃搄搅构搇搈搉搊搋搌损搎搏搐搑搒搓搔搕摇捣搘搙搚搛搜搝搞擀搠搡搢搣搤搥搦搧搨搩搪搫搬搭搮搯搰搱搲搳搴搵抢搷搸搹携搻搼搽搾搿摀摁摂摃摄摅摆摇摈摉摊摋摌摍摎摏摐掴摒摓摔摕摖摗摘摙摚摛掼摝摞搂摠摡摢摣摤摥摦摧摨摩摪摫摬摭摮挚摰摱摲抠摴摵抟摷摸摹摺掺摼摽摾摿撀撁撂撃撄撅撆撇捞撉撊撋撌撍撎撏撑撑撒挠撔撕撖撗撘撙捻撛撜撝撞挢撠撡掸掸撤拨撦撧撨撩撪抚撬播撮撯撰撱扑揿撴撵撶撷撸撹撺挞撼撽挝捡擀拥擂擃掳擅擆择擈擉击挡擌操擎擏擐擑擒擓担擕擖擗擘擙据擛擜擝擞擟挤擡擢捣擤擥擦擧擨擩擪擫拟擭擮摈拧搁掷擳扩擵擶撷擸擹摆擞撸擽扰擿攀攁攂攃摅攅撵攇攈攉攊攋攌攍攎拢攐攑攒攓拦攕撄攗攘搀攚撺携摄攞攟攠攡攒挛摊攥攦攧攨攩搅攫揽攭攮支攰攱攲攳攴攵收考攸改攺攻攼攽放政敀敁敂敃敄故敆敇效敉敊敋敌敍敎敏敐救敒敓敔敕敖败叙教敚敛敜敝敞敟敠敡敢散敤敥敦敧敨敩敪敫敬敭敮敯数敱敲敳整敌敶敷数敹敺敻敼敽敾敿斀斁敛毙斄斅斆文斈斉斊斋斌斍斎斏斐斑斒斓斔斓斖斗斘料斚斛斜斝斞斟斠斡斢斣斤斥斦斧斨斩斪斫斩断斮斯新斱斲斳斴斵斶断斸方斺斻於施斾斿旀旁旗旃旄旅旆旇旈旉旊旋旌旍旎族旐旑旒旓旔旕旖旗旘旙旚旛旜旝旞旟无旡既旣旤日旦旧旨早旪旫旬旭旮旯旰旱旲旳旴旵时旷旸旹旺旻旼旽旾旿昀昁昂昃昄昅昆昇昈昉昊昋昌昍明昏昐昑昒易昔昕昖昗昘昙昚昛昜昝昞星映昡昢昣昤春昦昧昨昩昪昫昬昭昮是昰昱昲昳昴昵昶昷昸昹昺昻昼昽显昿晀晁时晃晄晅晆晇晈晋晊晋晌晍晎晏晐晑晒晓晔晕晖晗晘晙晚晛晜昼曦晟晠晡晢晣晤晥晦晧晨晩晪晫晬晭普景晰晱晲晳晴晵晶晷晸晹智晻晼晽晾晿暀暁暂暃暄暅暆暇晕晖暊暋暌暍暎暏暐暑暒暓暔暕暖暗阳暙暚暛暜暝暞暟暠暡畅暣暤暥暦暧暨暩暪暂暬暭暮暯暰昵暲暳暴暵暶暷了暹暺暻暼暽暾暿曀曁曂曃晔曅历昙曈晓曊曋曌曍曎曏曐曑曒曓曔曕暧曗曘曙曚曛曜曝曞曟旷曡曢曣曤曥曦曧曨曩曪曫晒曭曮曯曰曱曲曳更曵曶曷书曹曺曻曼曽曾替最朁朂会朄朅朆朇月有朊朋朌服朎朏朐朑朒朓朔朕朖朗朘朙朚望朜朝朞期朠朡朢朣朤朥朦胧木朩未末本札术术朰朱朲朳朴朵朶朷朸朹机朻朼朽朾朿杀杁杂权杄杅杆圬杈杉杊杋杌杍李杏材村杒杓杔杕杖杗杘杙杚杛杜杝杞束杠条杢杣杤来杦杧杨杩杪杫杬杭杮杯杰东杲杳杴杵杶杷杸杹杺杻杼杽松板枀极枂枃构枅枆枇枈枉枊枋枌枍枎枏析枑枒枓枔枕枖林枘栀枚枛果枝枞枟枠枡枢枣枤枥枦枧枨枩枪枫枬枭枮枯枰枱枲枳拐枵架枷枸枹枺枻枼枽枾枿柀柁柂柃柄柅柆柇柈柉柊柋柌柍柎柏某柑柒染柔柕柖柗柘柙柚柛柜柝柞柟柠柡柢柣柤查柦柧柨柩柪柫柬柭柮柯柰柱柲柳柴栅柶柷柸柹柺査柼柽柾柿栀栁栂栃栄栅栆标栈栉栊栋栌栍栎栏栐树栒栓栔栕栖栗栘栙栚栛栜栝栞栟栠校栢栣栤栥栦栧栨栩株栫栬栭栮栯栰栱栲栳栴栵栶样核根栺栻格栽栾栿桀桁桂桃桄桅框桇案桉桊桋桌桍桎桏桐桑桒桓桔桕桖桗桘桙桚桛桜桝桞桟桠桡桢档桤桥桦桧桨桩桪桫桬桭桮桯桰桱桲桳桴桵桶桷桸桹桺桻桼桽桾杆梀梁梂梃梄梅梆梇梈梉梊梋梌梍梎梏梐梑梒梓栀梕梖梗梘梙梚梛梜条梞枭梠梡梢梣梤梥梦梧梨梩梪梫梬梭梮梯械梱梲梳梴梵梶梷梸梹梺梻梼梽梾梿检棁棂棃弃棅棆棇棈棉棊棋棌棍棎棏棐棑棒棓棔棕枨枣棘棙棚棛棜棝棞栋棠棡棢棣棤棥棦栈棨棩棪棫棬棭森棯棰棱栖棳棴棵棶棷棸棹棺棻棼棽棾棿椀椁椂椃椄椅椆椇椈椉椊椋椌植椎桠椐椑椒椓椔椕椖椗椘椙椚椛検椝椞椟椠椡椢椣椤椥椦椧椨椩椪椫椬椭椮椯椰椱椲椳椴椵椶椷椸椹椺椻椼椽椾椿楀楁楂楃楄楅楆楇楈楉杨楋楌楍楎楏楐楑楒枫楔楕楖楗楘楙楚楛楜楝楞楟楠楡楢楣楤楥楦楧桢楩楪楫楬业楮楯楰楱楲楳楴极楶楷楸楹楺楻楼楽楾楿榀榁概榃榄榅榆榇榈榉榊榋榌榍榎榏榐榑榒榓榔榕榖榗榘榙榚榛榜榝榞榟榠榡榢榣榤榥干榧榨榩杩榫榬榭荣榯榰榱榲榳榴榵榶榷榸榹榺榻榼榽榾桤槀槁槂盘槄槅槆槇槈槉槊构槌枪槎槏槐槑槒杠槔槕槖槗様槙槚槛槜槝槞槟槠槡槢槣槤槥槦椠椁槩槪槫槬槭槮槯槰槱槲桨槴槵槶槷槸槹槺槻槼槽槾槿樀桩乐樃樄枞樆樇樈樉樊樋樌樍樎樏樐梁樒楼樔樕樖樗樘标樚樛樜樝枢樟樠模樢样樤樥樦樧樨権横樫樬樭樮樯樰樱樲樳樴樵樶樷朴树桦樻樼樽樾樿橀橁橂橃橄橅橆橇桡橉橊桥橌橍橎橏橐橑橒橓橔橕橖橗橘橙橚橛橜橝橞机橠橡椭橣橤橥橦橧橨橩橪横橬橭橮橯橰橱橲橳橴橵橶橷橸橹橺橻橼橽橾橿檀檩檂檃檄檅檆檇檈柽檊檋檌檍檎檏檐檑檒檓档檕檖檗檘檙檚檛桧檝檞檟檠檡检樯檤檥檦檧檨檩檪檫檬檭檮台檰檱檲槟檴檵檶檷柠檹檺槛檼檽檾檿櫀櫁櫂柜櫄櫅櫆櫇櫈櫉櫊櫋櫌櫍櫎櫏櫐櫑櫒橹櫔櫕櫖櫗櫘櫙榈栉櫜椟橼栎櫠櫡櫢櫣櫤橱櫦槠栌櫩枥櫫榇櫭櫮櫯櫰櫱櫲栊櫴櫵櫶櫷榉櫹棂樱櫼櫽櫾櫿欀欁欂欃栏欅欆欇欈欉权欋欌欍欎椤欐欑栾欓欔欕榄欗欘欙欚欛欜欝棂欟欠次欢欣欤欥欦欧欨欩欪欫欬欭欮欯欰欱欲欳欴欵欶欷欸欹欺欻欼钦款欿歀歁歂歃歄歅歆歇歈歉歊歋歌歍叹歏欧歑歒歓歔歕歖歗歘歙歚歛歜歝歞欤歠欢止正此步武歧歨歩歪歫歬歭歮歯歰歱岁歳歴歵歶历归歹歺死歼歽歾殁殀殁殂殃殄殅殆殇殈殉殊残殌殍殎殏殐殑殒殓殔殕殖殗残殙殚殛殜殝殒殟殠殡殢殣殇殥殦殧殨殩殪殚殬殭殓殡殰殱歼殳殴段殶殷殸殹杀殻壳殽殾殿毁毁毂毃毄毅殴毇毈毉毊毋毋母毎每毐毑毒毓比毕毖毗毘毙毚毛毜毝毞毟毠毡毢毣毤毥毦毧毨毩毪毫球毭毮毯毰毱毲毳毴毵毶毷毸毹毺毻毼毽毾毵氀氁氂氃氄氅氆氇毡氉氊氋氇氍氎氏氐民氒氓气氕氖気氘氙氚氛氜氝氞氟氠氡氢气氤氥氦氧氨氩氪氢氩氭氮氯氰氱氲氲水氵氶氷永氹氺氻氼氽泛氿汀汁求汃汄汅汆汇汈汉汊汋汌丸泛汏汐汑汒汓汔汕汖汗汘污汚汛汜汝汞江池污汢汣汤汥汦汧汨汩汪汫汬汭汮汯汰汱汲汳汴汵汶汷汸汹决汻汼汽汾汿沀沁沂沃沄沅沆沇沈沉沊沋沌沍沎沏沐沑没沓沔沕冲沗沘沙沚沛沜沝沞沟沠没沢沣沤沥沦沧沨沩沪沫沬沭沮沯沰沱沲河沴沵沶沷沸油沺治沼沽沾沿泀况泂泃泄泅泆泇泈泉泊泋泌泍泎泏泐泑泒泓泔法泖泗泘泙泚泛泜泝泞泟泠泡波泣泤泥泦泧注泩泪泫泬泭泮泯泰泱泲泳泴泵泶泷泸泹泺泻泼泽泾泿洀洁洂洃洄洅洆洇洈洉洊洋洌洍洎洏洐洑洒洓洔洕洖洗洘洙洚洛洜洝洞洟洠洡洢洣洤津洦洧洨泄洪洫洬洭洮洯洰洱洲洳洴洵汹洷洸洹洺活洼洽派洿浀流浂浃浄浅浆浇浈浉浊测浌浍济浏浐浑浒浓浔浕浖浗浘浙浚浛浜浝浞浟浠浡浢浣浤浥浦浧浨浩浪浫里浭浮浯浰浱浲浳浴浵浶海浸浃浺浻浼浽浾浿涀涁涂涃涄涅涆泾消涉涊涋涌涍涎涏涐涑涒涓涔涕涖涗涘涙涚涛涜涝涞涟涠涡涢涣涤涥润涧涨涩涪涫涬涭涮涯涰涱液涳涴涵涶涷涸涹涺涻凉涽涾涿淀淁淂淃淄淅淆淇淈淉淊淋淌淍淎淏淐淑凄淓淔淕淖淗淘淙泪淛淜淝淞淟淠淡淢淣淤渌淦淧净淩沦淫淬淭淮淯淰深淲淳淴渊涞混淸淹浅添淼淽淾淿渀渁渂渃渄清渆渇済渉渊渋渌渍渎渏渐渑渒渓渔渕渖渗渘涣渚减渜渝渞渟渠渡渢渣渤渥涡渧渨温渪渫测渭渮港渰渱渲渳渴渵渶渷游渹渺渻渼渽浑渿湀湁湂湃湄湅湆湇湈湉凑湋湌湍湎湏湐湑湒湓湔湕湖湗湘湙湚湛湜湝浈湟湠湡湢湣湤湥湦涌湨湩湪湫湬湭湮汤湰湱湲湳湴湵湶湷湸湹湺湻湼湽湾湿満溁溂溃溄溅溆溇沩溉溊溋溌溍溎溏源溑溒溓溔溕准溗溘溙溚溛溜沟溞溟溠溡溢溣溤溥溦溧溨溩溪温溬溭溮溯溰溱溲溳溴溵溶溷溸溹溺溻湿溽溾溿滀滁滂滃沧灭滆滇滈滉滊滋涤滍荥滏滐滑滒滓滔滕滖滗滘滙滚滛滜滝滞滟滠满滢滣滤滥滦滧滨滩滪滫沪滭滮滞滰滱渗滳滴滵滶卤浒滹滺滻滼滽滚满漀渔漂漃漄漅漆漇漈漉漊漋漌漍漎漏漐漑漒漓演漕漖漗漘漙沤漛漜漝漞漟漠漡汉涟漤漥漦漧漨漩漪漫渍漭漮漯漰漱涨漳漴漵漶漷渐漹漺漻漼漽漾浆潀颍潂潃潄潅潆潇潈潉潊潋潌潍潎潏潐泼潒潓洁潕潖潗潘潙潚潜潜潝潞泻潠潡潢潣润潥潦潧潨潩潪潫潬潭潮浔溃潱潲潳潴潵潶滗潸潹潺潻潼潽潾涠涩澁澂澃澄澅浇涝澈澉澊澋澌澍澎澏澐澑澒澓澔澕澖涧澘澙澚澛澜澝澞澟渑澡澢澣泽澥澦澧澨泶澪澫澬澭浍澯澰淀澲澳澴澵澶澷澸澹澺澻澼澽澾澿激浊濂浓濄濅濆濇濈濉濊濋濌濍濎濏濐濑濒濓濔湿濖濗泞濙濚蒙濜濝濞济濠濡濢濣涛濥濦濧濨濩濪滥濬濭濮濯潍滨濲濳濴濵濶濷濸濹溅濻泺濽滤濿瀀瀁瀂瀃瀄滢渎瀇瀈泻瀊渖瀌瀍瀎浏瀐瀑瀒瀓瀔濒瀖瀗泸瀙瀚瀛瀜沥瀞潇潆瀡瀢瀣瀤瀥瀦泷濑瀩瀪瀫瀬瀭瀮瀯弥瀱潋瀳瀴瀵瀶瀷瀸瀹瀺瀻瀼瀽澜瀿灀灁灂沣滠灅灆灇灈灉灊灋灌灍灎灏灐洒灒灓灔漓灖灗滩灙灚灛灜灏灞灟灠灡灢湾滦灥灦灧灨灩灪火灬灭灮灯灰灱灲灳灴灵灶灷灸灹灺灻灼灾灾灿炀炁炂炃炄炅炆炇炈炉炊炋炌炍炎炏炐炑炒炓炔炕炖炗炘炙炚炛炜炝炞炟炠炡炢炣炤炥炦炧炨炩炪炫炬炭炮炯炰炱炲炳炴炵炶炷炸点为炻炼炽炾炿烀烁烂烃烄烅烆烇烈烉烊烋烌烍烎乌烐烑烒烓烔烕烖烗烘烙烚烛烜烝烞烟烠烡烢烣烤烥烦烧烨烩烪烫烬热烮烯烰烱烲烳烃烵烶烷烸烹烺烻烼烽烾烿焀焁焂焃焄焅焆焇焈焉焊焋焌焍焎焏焐焑焒焓焔焕焖焗焘焙焚焛焜焝焞焟焠无焢焣焤焥焦焧焨焩焪焫焬焭焮焯焰焱焲焳焴焵然焷焸焹焺焻焼焽焾焿煀煁煂煃煄煅煆煇煈炼煊煋煌煍煎煏煐煑炜煓煔煕煖煗煘烟煚煛煜煝煞煟煠煡茕煣煤焕煦照煨烦煪煫炀煭煮煯煰煱煲煳煴煵煶煷煸煹煺煻煼煽煾煿熀熁熂熃熄熅熆熇熈熉熊熋熌熍熎熏熐熑荧熓熔熕熖炝熘熙熚熛熜熝熞熟熠熡熢熣熤熥熦熧熨熩熪熫熬熭熮熯熰热熲熳熴熵熶熷熸熹熺熻熼熽炽熿燀烨燂燃焰燅燆燇灯炖燊燋燌燍燎燏磷燑烧燓燔燕燖燗燘烫燚燛焖燝燞营燠燡燢燣燤燥灿燧燨燩燪燫毁烛燮燯燰燱燲燳烩燵燶燷燸燹燺燻烬燽焘燿爀爁爂爃爄爅爆爇爈爉爊爋爌烁爎爏炉爑爒爓爔爕爖爗爘爙爚烂爜爝爞爟爠爡爢爣爤爥爦爧爨爩爪爫爬争爮爯爰爱爲爳爴爵父爷爸爹爷爻爼爽尔爿牀牁牂牃牄牅墙片版牉牊牋牌牍牎牏牐牑牒牓牔牕牖牗牍牙牚牛牜牝牞牟牠牡牢牣牤牥牦牧牨物瘪牫牬牭牮牯牰牱牲牳抵牵牶牷牸特牺牻牼牵牾牿犀犁犂犃犄犅犆犇犈犉犊犋犌犍犎犏犐犑犒犓犔犕荦犗犘犙犚犁犜犝犞犟犠犡犊犣犤犥犦牺犨犩犪犫犬犭犮犯犰犱犲犳犴犵状犷犸犹犺犻犼犽犾犿状狁狂狃狄狅狆狇狈狉狊狋狌狍狎狏狐狑狒狓狔狕狖狗狘狙狚狛狜狝狞狟狠狡狢狣狤狥狦狧狨狩狪狫独狭狮狯狰狱狲狳狴狵狶狷狸狭狺狻狼狈狾狿猀猁猂猃猄猅猆猇猈猉猊猋猌猍猎猏猐猑猒猓猔猕猖猗猘狰猚猛猜猝猞猟猠猡猢猣猤猥猦猧猨猩猪猫猬猭献猯猰猱猲猳猴猵犹猷猸猹猺狲猼猽猾猿獀獁獂獃狱狮獆獇獈獉獊獋獌獍奖獏獐獑獒獓獔獕獖獗獘獙獚獛獜獝獞獟獠獡獢獣獤獥獦獧独獩狯猃獬獭猕獯狞獱获獳獴猎獶犷兽獹獭献猕獽獾獿猡玁玂玃玄玅兹率玈玉玊王玌玍玎玏玐玑玒玓玔玕玖玗玘玙玚玛玜玝玞玟玠玡玢玣玤玥玦玧珏玩玪玫玬玭玮环现玱玲玳玴玵玶玷玸玹玺玻玼玽玾玿珀珁珂珃珄珅珆珇珈珉珊珋珌珍珎珏珐珑珒珓珔珕珖珗珘珙珚珛珜珝珞珟珠珡珢珣珤珥珦珧珨珩珪珫珬班佩珯珰珱珲珳珴珵珶珷珸珹珺珻珼珽现珿琀琁琂球琄琅理琇琈琉琊琋琌琍琎琏琐琑琒琓琔琕琖琗琘琙琚琛琜琝琞琟琠琡琢琣琤琥琦琧琨琩琪琫琬琭琮琯琰琱琲琳琴琵琶琷琸琹珐琻琼琽琾珲瑀瑁瑂瑃瑄瑅瑆瑇瑈瑉瑊玮瑌瑍瑎瑏瑐瑑瑒瑓瑔瑕瑖瑗瑘瑙瑚瑛瑜瑝瑞瑟瑠瑡瑢琐瑶瑥瑦瑧瑨莹玛瑫瑬瑭瑮琅瑰瑱瑲瑳瑴瑵瑶瑷瑸瑹瑺瑻瑼瑽瑾瑿璀璁璂璃璄璅璆璇璈琏璊璋璌璍璎璏璐璑璒璓璔璕璖璗璘璙璚璛璜璝璞璟璠璡璢玑璤璥瑷璧璨璩璪璫璬璭璮璯环璱璲璳璴璵璶璷璸璹璺璻璼玺璾璿瓀瓁瓂瓃瓄瓅瓆瓇瓈瓉琼瓋瓌瓍瓎珑瓐瓑瓒瓓璎瓕镶瓗瓘瓙瓒瓛瓜瓝瓞瓟瓠瓡瓢瓣瓤瓥瓦瓧瓨瓩瓪瓫瓬瓭瓮瓯瓰瓱瓲瓳瓴瓵瓶瓷瓸瓹瓺瓻瓼瓽瓾瓿甀甁甂甃甄甅甆甇甈甉甊甋瓯甍甎甏甐甑甒甓甔瓮甖甗甘甙甚甛甜甝甞生甠甡产産甤甥苏甧用甩甪甫甬甭甮甯田由甲申甴电甶男甸甹町画甼甽甾甿畀畁畂畃畄畅畆畇畈畉畊畋界畍畎畏畐畑畒畓畔畕畖畗畘留畚畛畜亩畞畟畠畡毕畣畤略畦畧畨畩番画畲畭畮畯异畱畲畳畴畵当畷畸畹畺畻畼畽畾畿疀疁疂疃疄疅疆畴疈疉叠疋疌疍疎疏疐疑疒疓疔疕疖疗疘疙疚疛疜疝疞疟疠疡疢疣疤疥疦疧疨疩疪疫疬疭疮疯疰疱疲疳疴疵疶疷疸疹疺疻疼疽疾痱痀痁痂痃痄病痆症痈痉痊痋痌痍痎痏痐痑痒痓痔痕痖痗痘痉痚痛痜痝痞痟酸痡痢痣痤痥痦痧痨痩痪痫痬痭痮痯痰痱麻麻痴痵痶痷痸痹痹痻痼痽痾痿瘀瘁瘂瘃瘄瘅瘆瘇瘈瘉瘊疯瘌疡瘎瘏瘐瘑瘒痪瘔瘕瘖瘗瘘瘙瘚瘛瘜瘝瘗瘟瘠疮瘢瘣瘤瘥瘦疟瘨瘩瘪瘫瘬瘭瘮瘯瘰瘱瘲瘳瘴瘵瘶瘷瘸瘹瘘瘻瘼瘽瘾瘿癀癁疗癃癄癅痨痫癈瘅癊癋癌癍癎癏癐癑癒癓癔癕癖癗疠癙癚癛癜癝癞瘪癠痴痒癣疖症癦癧癨癞癪癫癣瘿瘾癯痈瘫癫癳癴癵癶癷癸癹発登发白百癿皀皁皂皃的皅皆皇皈皉皊皋皌皍皎皏皐皑皒皓皔皕皖皗皘皙皑皛皜皝皞皟皠皡皢皣皤皥皦皧皨皩皪皫皬皭皮皯疱皱皲皳皴皵皶皷皲皹皱隳皼皽皾皿盀盁盂盃盄盅盆盇盈盉益盋盌盍盎盏盐监盒盓盔盕盖盗盘盙盚盛盗盝盏盟盠尽盢监盘盥盦卢盨盩荡盫盬盭目盯盰盱盲盳直盵盶盷相盹盺盻盼盽盾盿眀省眂眃眄眅眆眇眈眉眊看県眍眎眏眐眑眒眓眔眕眖眗眘眙眚眛眜眝眞真眠眡眢眣眤眦眦眧眨眩眪眫眬眭眮眯眰眱眲眳眴眵眶眷眸眹眺眻眼眽众眿着睁睂睃睄睅睆睇睈睉睊睋睌睍睎困睐睑睒睓睔睕睖睗睘睙睚睛睁睝睐睟睠睡睢督睤睥睦睧睨睩睾睫睬睭睮睯睰睱睲睳睴睵睶睷睸睹睺睻睼睽睾睿瞀瞁瞂瞃瞄瞅瞆眯瞈瞉瞊瞋瞌瞍瞎瞏瞐瞑瞒瞓瞔瞕瞖瞗瞘瞙瞚瞛瞜瞝瞒瞟瞠瞡瞢瞣瞤瞥瞦瞧瞨瞩瞪瞫瞬了瞮瞯瞰瞱瞲瞳瞴瞵瞶瞷瞸瞹瞺瞻睑瞽瞾瞿矀矁矂矃矄矅矆蒙矈矉矊矋矌矍矎矏矐矑矒胧矔矕矖矗矘矙瞩矛矜矝矞矟矠矡矢矣矤知矦矧矨矩矪矫矬短矮矫矰矱矲石矴矵矶矷矸矹矺矻矼矽矾矿砀码砂砃砄砅砆砇砈砉砊砋砌砍砎砏砐砑砒砓研砕砖砗砘砙砚砛砜砝砞砟砠砡砢砣砤砥砦砧砨砩砪砫砬砭砮砯砰砱炮砳破砵砶砷砸砹砺砻砼砽砾砿础硁硂朱硄硅硆硇硈硉硊硋硌硍硎硏硐硑硒硓硔硕硖硗硘硙硚硛硜硝硞硟硠硡硢硣硖硥硦硧砗硩硪硫硬硭确砚硰硱硲硳硴硵硶硷硸硹硺硻硼硽硾硿碀碁碂碃碄碅碆碇碈碉碊碋碌碍碎碏碐碑碒碓碔碕碖碗碘碙碚碛碜碝碞碟碠碡碢碣碤碥碦碧碨硕碪碫碬砀碮碯碰碱碲碳碴碵碶碷碸碹确碻码碽碾碿磀磁磂磃磄磅磆磇磈磉磊磋磌磍磎磏磐磑磒磓磔磕磖磗磘磙砖磛磜磝磞磟磠磡磢碜磤磥磦碛磨磩磪磫磬磭磮矶磰磱磲磳磴磵磶磷磸磹磺磻磼硗磾磿礀礁礂礃礄礅礆礇礈礉礊礋礌礍础礏礐礑礒礓礔礕礖礗礘碍礚礛礜礝礞礟礠礡礢礣礤礥矿礧礨礩砺砾矾礭礮礯礰砻礲礳礴礵礶礷礸礹示礻礼礽社礿祀祁祂祃祄祆祆只祈祉祊祋祌祍祎祏佑祑祒祓祔秘祖祗祘祙祚祛祜祝神祟祠祡祢祣祤祥祦祧票祩祪祫祬祭祮祯祰祱祲祳祴祵祶祷祸祹祺祻祼祽祾禄禀禁禂禃禄禅禆禇禈禉禊禋禌祸祯福禐禑禒禓禔禕禖禗禘禙禚禛禜禝禞禟禠禡禢禣禤禥御禧禨禩禅禫禬禭礼禯祢祷禲禳禴禵禶禷禸禹禺离禼禽禾秃秀私秂秃秄秅秆秇籼秉秊秋秌种秎秏秐科秒秓秔秕秖秗秘秙秚秛秜秝秞租秠秡秢秣秤秥秦秧秨秩秪秫秬秭秮积称秱秲秳秴秵秶秷秸秹秺移秼秽秾秿稀稁稂稃稄税稆稇秆稉稊程稌稍税稏稐稑稒稓稔稕稖稗稘稙稚稛棱稝稞禀稠稡稢稣稤稥稦稧稨稩稪稫稬稭种稯稰称稲稳稴稵稶稷稸稹稺稻稼稽稾稿谷穁穂穃穄穅穆穇穈穉穊穋稣积颖穏穐穑穒穓穔穕穖穗穘穙穚穛穜穝穞穟穠穑秽穣穤穥穦穧穨稳穪获穬穭穮穯穰穱穲穳穴穵究穷穸穹空穻穼穽穾穿窀突窂窃窄窅窆窇窈窉窊窋窌窍窎窏窐窑窒窓窔窕窖窗窘窙窚窛窜窝窞窟窠窡窢窣窤窥窦窧窨窝洼窫窬窭穷窑窰窱窲窳窴窵窭窷窸窹窥窻窼窽窾窿竀竁竂竃窜窍竆窦竈竉窃立竌竍竎竏竐竑竒竓竔竕竖竗竘站竚竛竜竝竞竟章竡竢竣竤童竦竧竨竩竪竫竬竭竮端竰竱竲竳竴竵竞竷竸竹竺竻竼竽竾竿笀笁笂笃笄笅笆笇笈笉笊笋笌笍笎笏笐笑笒笓笔笕笖笗笘笙笚笛笜笝笞笟笠笡笢笣笤笥符笧笨笩笪笫第笭笮笯笰笱笲笳笴笵笶笷笸笹笺筇笼笽笾笿筀筁筂筃筄筅笔筇筈等筊筋筌笋筎筏筐筑筒筓答筕策筗筘筙筚筛筜筝筞筟筠筡筢筣筤筥筦笕筨筩筪筫筬筭筮筯筰筱筲筳筴筵筶筷筸筹筺筻筼筽签筿简箁箂箃箅箅箆个箈箉箊笺箌箍箎筝箐箑箒箓箔箕箖算箘箙箚箛箜箝箞箟箠管箢箣箤箥箦箧箨箩箪箫箬箭箮箯箰箱箲箳箴箵箶箷箸箹箺箻箼箽箾箿节篁篂篃范篅篆篇篈筑篊箧篌篍篎篏篐篑篒篓篔篕篖篗篘篙篚篛篜篝篞篟筱篡篢篣笃篥篦篧篨筛篪篫篬篭篮篯篰篱篲筚篴篵篶篷篸篹篺篻篼篽篾篿箦簁簂簃簄簅簆簇簈簉簊簋簌篓簎簏簐蓑簒簓簔簕簖簗簘簙簚簛簜簝箪簟簠简簢篑簤簥簦簧簨簩簪箫簬簭簮簯簰簱簲簳簴簵簶檐簸簹簺簻簼签帘簿籀籁籂篮籄籅籆籇籈籉籊籋筹籍籎籏藤籑籒籓籔籕籖籗籘籙籚籛箨籝籞籁笼籡籢籣签龠籦籧籨笾簖籫篱籭箩籯籰籱吁米籴籵籶籷籸籹籺类籼籽籾籿粀粁粂粃粄粅粆粇粈粉粊粋粌粍粎粏粐粑粒粓粔粕粖粗粘粙粚粛粜粝粞粟粠粡粢粣粤粥粦粧粨粩粪粫粬粭粮粯粰粱粲粳粴粤粶粷粸粹粺粻粼粽精粿糀糁糂糃糄糅糆糇糈糉糊糋糌糍糎糏糐糑糒糓糔糕糖糗糘糙糚糛糜糁粪糟糠糡馍糣糤糥糦粮糨糩糪糫糬糭糮糯团糱粝糳籴糵粜糷糸糹糺系糼糽纠糿纪紁纣紃约红纡纥纨纫紊纹紌纳紎紏纽紑紒纾纯纰紖纱紘纸级纷纭紝紞紟素纺索紣紤紥紦紧紨紩紪紫紬紭紮累细绂绁绅紴紵紶紷紸绍绀紻绋紽紾绐绌絁终弦组絅绊絇絈絉絊絋経絍绗絏结絑絒絓絔绝絖絗絘絙絚绦絜絝绞絟絠络绚絣絤絥给絧绒絩絪絫絬絭絮絯絰统丝绦絴絵絶絷絸绢絺絻絼絽絾絿綀绑綂绡綄綅绠綇绨綉綊綋綌綍綎绥綐綑綒经綔綕綖綗綘継続綛综綝缍綟绿綡绸绻綤綥綦綧綨綩綪綫绶维綮綯绾纲网綳缀彩綶綷纶绺绮绽綼绰绫绵緀緁緂緃绲緅緆缁緈緉紧绯緌緍緎総緐緑绪緓緔緕緖缃缄缂线緛緜缉缎緟缔缗緢缘緤緥缌緧编缓緪緫缅緭緮纬緰缑缈緳练緵缏緷緸缇緺致緼緽緾緿縀縁縂縃縄縅縆縇萦缙缢缒縌縍縎縏绉缣縒縓縔縕縖縗縘縙绦缚縜缜缟缛縠縡縢县縤縥縦縧縨縩縪缝縬缡缩演縰纵缧缚纤缦絷缕縸缥縺縻縼总绩縿繀繁繂绷繄缫缪繇襁繉繊繋繌繍繎繏繐繑缯繓织缮繖繗繘繙缭繛繜繝绕繟繠绣缋繣繤繥繦繧繨绳绘系繬茧繮缳缲繱繲缴繴繵繶繷繸绎繺繻继缤缱繿纀纁纂纃纄纅纆纇缬纉纩纋续累纎缠纐纑纒缨才纕纤纗缵纙纚纛缆纝纞纟纠纡红纣纤纥约级纨纩纪纫纬纭纮纯纰纱纲纳纴纵纶纷纸纹纺纻纼纽纾线绀绁绂练组绅细织终绉绊绋绌绍绎经绐绑绒结绔绕绖绗绘给绚绛络绝绞统绠绡绢绣绤绥绦继绨绩绪绫绬续绮绯绰绱绲绳维绵绶绷绸绹绺绻综绽绾绿缀缁缂缃缄缅缆缇缈缉缊缋缌缍缎缏缐缑缒缓缔缕编缗缘缙缚缛缜缝缞缟缠缡缢缣缤缥缦缧缨缩缪缫缬缭缮缯缰缱缲缳缴缵缶缷缸缹缺缻缼钵缾缿罀罁罂罃罄罅罆罇坛罉罊罋罂罍罎罏罐网罒罓罔罕罖罗罘罙罚罛罜罝罞罟罠罡罢罣罤罥罦罧罨罩罪罫罬罭置罯罚罱署罳罴骂罶罢罸罹罺罻罼罽罾罿羀羁羂羃羄罗罴羇羁羉羊芈羌羍美羏羐羑羒羓羔羕羖羗羘羙羚羛羜羝羞羟羠羡羢羣群羟羦羧羡义羪羫羬羭羮羯羰羱羲羳羴羵羶羷羸羹羺羻羼羽羾羿翀翁翂翃翄翅翆翇翈翉翊翋翌翍翎翏翐翑习翓翔翕翖翗翘翙翚翛翜翝翞翟翠翡翢翣翤翥翦翧翨翩翪翫翬翭翮翯翰翱翲翳翴翵翶翷翸翘翺翻翼翽翾翿耀老耂考耄者耆耇耈耉耊耋而耍耎耏耐端耒耓耔耕耖耗耘耙耚耛耜耝耞耟耠耡耢耣耤耥耦耧耨耩耪耫耧耭耮耯耰耱耲耳耴耵耶耷耸耹耺耻耼耽耾耿聀聁聂聃聄聅聆聇聈聉聊聋职聍聎聏聐聑聒聓联聕圣聗聘聙聚聛聜聝闻聟聠聡聢聣聤聥聦聧聨聩聪聫聬聭聮联聪聱声耸聴聩聂职聸聍聺聻聼听聋聿肀肁肂肃肄肃肆肇肈肉肊肋肌肍肎肏肐肑肒肓肔肕肖肗肘肙肚肛肜肝肞肟肠股肢肣肤肥肦肧肨肩肪肫肬肭肮肯肰肱育肳肴肵肶肷肸肹肺肻肼肽肾肿胀胁胂胃胄胅胆胇胈胉胊胋背胍胎胏胐胑胒胓胔胕胖胗胘胙胚胛胜胝胞胟胠胡胢胣胤胥胦胧胨胩胪胫胬胭胮胯胰胱胲胳胴胵胶胷胸胹胺胻胼能胾胿脀脁脂脃脄胁脆脇脉脉脊脋脌脍脎脏脐脑脒脓脔脕脖脗脘脙脚胫脜脝脞脟脠脡脢唇脤脥脦睃脨修脪脱脬脭脮脯脰脱脲脳脴脵脶脷脸胀脺脻脼脽脾脿腀腁腂腃腄腅腆腇腈腉腊腋腌腍肾腏腐腑腒腓腔腕腖腗腘腙腚腛腜腝腞腟腠脶腢腣腤腥脑腧腨腩腪肿腬腭腮腯腰腱腲脚腴腵腶腷肠腹腺腻腼腽腾腿膀膁膂腽膄膅膆膇膈膉膊膋膌膍膎膏膐膑膒膓膔膕膖膗膘膙肤膛膜膝膞膟胶膡膢膣膤膥膦膧膨腻膪膫膬膭膮膯膰膱膲膳膴膵膶膷膸膹膺膻膼胆脍脓臀臁臂臃臄臅臆臇臈脸臊臋臌脐臎膑臐臑臒臓臔臕臖臗腊臙胪臛臜臝臞脏脔臡臢臣臤卧臦臧临臩自臫臬臭臮臯臰臱臲至致臵臶臷臸臹台臻臼臽臾臿舀舁舂舃舄舅舆与兴举旧舋舌舍舎舏舐舑舒舓舔舕舖舗舘舙舚舛舜舝舞舟舠舡舢舣舤舥舦舧舨舩航舫般舭舮舯舰舱舲舳舴舵舶舷舸船舺舻舼舽舾舿艀艁艂艃艄艅艆艇艈艉艊艋艌艍艎艏艐艑艒艓艔艕艖艗艘舱艚艛艜艝艞艟艠艡艢橹舣艥舰艧艨艩艪舻艬艭艮良艰艰色艳艴艵艶艳艹艹艺艻艼艽艾艿芀芁节芃芄芅芆芇芈芉芊芋芌芍芎芏苄芑芒芓芔芕芖芗芘芙芚芛芜芝芞芟芠芡芢芣芤芥芦芧芨芩芪芫芬芭芮芯芰花芲芳芴芵芶芷芸芹芺刍芼芽芾芿苀苁苂苃苄苅苆苇苈苉苊苋苌苍苎苏苐苑苒苓苔苕苖苗苘苙苚苛苜苝苞苟苠苡苢苣苤若苦苎苨苩苪苫苬苭苮苯苰英苲苳苴苵苶苷苸苹苺苻苼苽苾苿茀茁茂范茄茅茆茇茈茉茊茋茌苟茎茏茐茑茒茓茔茕茖茗茘茙茚茛茜茝茞茟茠茡茢茣茤茥茦茧茨茩茪茫茬茭茮茯茰茱兹茳茴茵茶茷茸茹茺茻茼茽茾茿荀荁荂荃荄荅荆荇荈草荆荋荌荍荎荏荐荑荒荓荔荕荖荗荘荙荚荛荜荝荞荟荠荡荢荣荤荥荦荧荨荩荪荫荬荭荮药荰荱荲荳荴荵荶荷荸荹荺荻荼荽荾荿莀莁莂莃莄莅莆莇莈莉庄莋莌莍莎莏莐莑莒莓莔莕茎莗莘莙莚莛莜莝莞莟莠莡荚莣莤莥莦苋莨莩莪莫莬莭莮莯莰莱莲莳莴莵莶获莸莹莺莻莼莽莾莿菀菁菂菃菄菅菆菇菈菉菊菋菌菍菎菏菐菑菒菓菔菕菖菗菘菙菚菛菜菝菞菟菠菡菢菣菤菥菦菧菨菩菪菫菬菭菮华菰菱菲菳庵菵菶菷菸菹菺菻菼菽菾菿萀萁萂萃萄萅萆苌萈萉莱萋萌萍萎萏萐萑萒萓萔萕萖萗萘萙萚萛萜萝萞萟萠萡萢萣萤营萦萧萨萩萪萫万萭萮萯萰萱萲萳萴莴萶萷萸萹萺萻萼落萾萿葀葁葂葃葄葅葆葇葈叶葊葋葌葍葎葏葐葑荭葓葔葕葖着葘葙葚葛葜葝葞葟葠葡葢董葤葥苇葧葨葩葪葫葬葭葮药葰葱葲葳葴葵葶荤葸葹葺葻葼葽葾葿蒀蒁蒂蒃蒄蒅蒆蒇蒈蒉蒊蒋蒌蒍蒎蒏蒐蒑蒒蒓莳蒕蒖蒗蒘蒙蒚蒛蒜蒝莅蒟蒠蒡蒢蒣蒤蒥蒦蒧蒨蒩蒪蒫蒬蒭蒮蒯蒰蒱蒲蒳蒴蒵蒶蒷蒸蒹蒺蒻苍蒽蒾蒿荪蓁蓂蓃蓄蓅蓆蓇蓈蓉蓊盖蓌蓍蓎蓏蓐蓑蓒蓓蓔蓕蓖蓗蓘蓙蓚蓛蓜蓝蓞蓟蓠蓡蓢蓣蓤蓥蓦蓧蓨蓩蓪蓫蓬蓭莲苁蓰蓱蓲蓳蓴蓵蓶蓷蓸蓹蓺蓻蓼荜蓾蓿蔀蔁蔂蔃蔄蔅菱蔇蔈蔉蔊蔋蔌蔍蔎蔏蔐蔑蔒蔓卜蔕蔖蔗蔘蔙蔚蔛蔜蔝蒌蔟蔠蔡蔢蒋蔤葱茑蔧蔨蔩蔪蔫蔬荫蔮蔯蔰蔱蔲蔳蔴蔵蔶蔷蔸蔹蔺蔻蔼蔽蔾蔿蕀荨蕂蕃蕄蕅蒇蕇蕈蕉蕊蕋蕌蕍荞蕏蕐蕑蕒芸蕔莸蕖蕗荛蕙蕚蕛蕜蕝蕞蕟蕠蕡蒉蕣蕤蕥蕦蕧蕨荡芜蕫蕬萧蕮蕯蕰蕱蕲蕳蕴蕵蕶蓣蕸蕹蕺蕻蕼蕽蕾蕿薀薁薂薃薄薅薆薇荟薉蓟薋芗薍薎薏薐姜薒薓蔷薕薖薗薘薙薚薛薜薝薞莶薠薡薢薣薤薥荐槁薨萨薪薫薬薭薮薯薰薱薲薳薴薵薶薷薸薹荠薻薼薽薾薿藀藁藂藃藄藅藆藇藈藉藊藋藌蓝荩藏藐藑藒藓藔藕藖藗藘藙藚藛藜艺藞藟藠藡藢藣藤药藦藧藨藩薮藫藬藭藮藯藰藱藲藳藴藵苈薯藸蔼蔺藻藼藽藾藿蘀蘁蘂蘃蕲蘅芦苏蘈蘉蕴苹蘌蘍蘎蘏蘐蘑蘒蘓蘔蘕蘖蘖蘘蘙藓蘛蘜蘝蔹蘟蘠蘡茏蘣蘤蘥蘦蘧蘨蘩蘪蘫蘬兰蘮蘯蘰蘱蘲蘳蘴蘵蘶蘷蘸蘹蓠蘻蘼蘽蘾萝虀虁虂虃虄虅虆虇虈虉虊虋虌虍虎虏虐虑虒虓虔处虖虗虘虙虚虚虏虝虞号虠虡虢虣虤虥虦亏虨虩虪虫虬虭虮虯虰虱虲虳虴虵虶虷虸虹虺虻虼虽虾虿蚀蚁蚂蚃蚄蚅蚆蚇蚈蚉蚊蚋蚌蚍蚎蚏蚐蚑蚒蚓蚔蚕蚖蚗蚘蚙蚚蚛蚜蚝蚞蚟蚠蚡蚢蚣蚤蚥蚦蚧蚨蚩蚪蚫蚬蚭蚮蚯蚰蚱蚲蚳蚴蚵蚶蚷蚸蚹蚺蚻蚼蚽蚾蚿蛀蛁蛂蛃蛄蛅蛆蛇蛈蛉蛊蛋蛌蛍蛎蛏蛐蛑蛒蛓蛔蛕蛖蛗蛘蛙蛚蛛蛜蛝蛞蛟蛠蛡蛢蛣蛤蛥蛦蛧蛨蛩蛪蛫蛬蛭蛮蛯蛰蛱蛲蛳蛴蛵蛶蛷蛸蛹蛱蜕蛼蛽蛾蛿蜀蜁蜂蜃蜄蜅蚬蜇蜈蜉蜊蜋蜌蜍蜎蜏蜐蜑蜒蜓蜔蜕蜖蜗蜘蜙蜚蜛蜜蜝蜞蜟蜠蜡蜢蜣蜤蜥蜦蜧蜨蜩蜪蜫蜬蜭蜮蜯蜰蜱蜲蜳蜴蜵蜶蜷蜸蜹蜺蜻蜼蜽蜾蜿蝀蝁蝂蝃蝄蝅蝆蝇蝈蝉蝊蝋蝌蝍蝎蝏蝐蝑蝒蝓蝔蚀蝖蝗蝘蝙蝚蝛蝜蝝蝞蝟蝠蝡蝢蝣蝤蝥虾蝧蝨蝩蝪蝫蝬蝭蝮蝯蝰蝱蝲蝳蝴蝵蝶蝷蜗蝹蝺蝻蝼蝽蝾蝿螀螁螂螃蛳螅螆螇螈螉螊螋螌融螎螏螐螑螒螓螔螕螖螗螘螙螚螛螜螝蚂螟螠螡萤螣螤螥螦螧螨螩螪螫螬螭螮螯螰螱螲螳螴螵螶螷螸螹螺蝼螼螽螾螿蟀蟁蟂蟃蛰蟅蟆蟇蝈蟉蟊蟋蟌蟍蟎蟏蟐蟑蟒蟓蟔蟕蟖蟗蟘蟙蟚蟛蟜蟝蟞蟟蟠蟡蟢虮蟤蟥蟦蟧蟨蟩蟪蟫蝉蟭蟮蛲蟰蟱虫蟳蟴蟵蛏蟷蟸蟹蟺蚁蟼蟽蟾蟿蠀蠁蠂蠃蠄蝇虿蠇蠈蠉蠊蠋蠌蠍蠎蠏蛴蝾蠒蠓蚝蠕蠖蠗蠘蠙蠚蠛蠜蠝蠞蜡蠠蠡蠢蛎蠤蠥蠦蠧蠨蠩蠪蠫蠬蠭蠮蠯蠰蛊蠲蠳蠴蠵蚕蠷蠸蠹蠺蛮蠼蠽蠾蠿血衁衂衃衄衅衆衇衈衉蔑衋行衍衎衏衐衑衒术衔衕衖街衘衙衚卫衜冲衞衟衠衡衢衣衤补衦衧表衩衪衫衬衭衮衯衰衱衲衳衴衵衶衷衸只衺衻衼衽衾衿袀袁袂袃袄袅袆袇袈袉袊袋袌袍袎袏袐袑袒袓袔袕袖袗袘袙袚袛袜袝衮袟袠袡袢袣袤袥袦袧袨袩袪被袬袭袮袯袰袱袲袳袴袵袶袷袸袹袺袻袼袽袾袿裀裁裂裃裄装裆裇裈裉袅裋裌裍裎里裐裑裒裓裔裕裖裗裘裙裚裛补装裞裟裠里裢裣裤裥裦裧裨裩裪裫裬裭裮裯裰裱裲裳裴裵裶裷裸裹裺裻裼制裾裿褀褁褂褃褄褅褆复褈褉褊褋褌褍褎褏褐褑褒褓褔褕褖褗褘褙褚褛褜褝褞褟褠褡褢褣褤褥褦褧褨褩褪褫褬褭褮褯褰褱裤裢褴褵褶褷褛褹褺亵褼褽褾褿襀襁襂襃襄襅襆襇襈襉襊襋襌襍襎襏襐襑襒襓襔襕袄襗襘襙襚襛襜裣襞襟裆襡襢襣褴襥襦襧襨襩袜襫摆襭襮衬襰襱袭襳襴襵襶襷襸襹襺襻襼襽襾西覀要覂覃覄覅覆覇核覉覊见覌覍覎规覐覑覒觅覔覕视覗觇覙覚覛覜覝覞覟覠觋覢覣覤覥觎覧覨覩亲覫觊覭覮觏覰覱觐観覴覵覶觑覸覹觉覻覼览覾觌观见观觃规觅视觇览觉觊觋觌觍觎觏觐觑角觓筋觕觖觗觘觙觚觛觜觝觞觟觠觡觢解觤觥触觧觨觩觪觫觬觭觮觯觰觱觲觳觞觵觯觷触觹觺觻觼觽觾觿言訁订讣訄訅訆訇计訉讯訋讧訍讨訏讦訑訒训訔讪讫托记訙訚讹訜讶訞讼訠訡欣诀訤讷訦訧訨訩访訫訬设訮訯訰许訲訳诉訵诃訷訸訹诊注证訽訾訿詀诂詂詃詄詅诋詇詈詉詊詋詌詍讵詏诈詑诒詓诏评詖詗诎詙詚诅詜詝词詟咏诩询诣詤詥试詧詨诗詪诧诟诡诠詯诘话该详詴诜詶詷詸詹詺詻诙詽詾诖誀誁誂誃诔诛诓夸誈誉誊誋志认誎誏誐诳诶誓誔诞誖誗诱誙诮誛誜誝语誟诚诫誢诬误诰诵誧诲誩说誫説読誮誯谁誱课誳誴誵谇誷誸诽誺誻谊誽誾调諀諁谄諃谆諅諆谈諈诿諊请諌诤諎诹諐诼谅諓諔諕论谂諘諙諚谀谍諝谝諟諠諡诨諣谔諥谛谐諨諩諪谏諬谕谘諯諰讳諲谙諴諵谌讽诸諹谚諻谖諽诺諿谋谒谓謃誊诌謆謇謈謉谎謋謌謍谜謏谧謑謒謓谑謕谡谤謘谦谥讲謜谢謞謟谣謡謢謣謤謥謦謧谟謩謪谪谬謭謮謯謰謱謲讴謴謵謶謷謸谨謺謻謼謽谩謿譀譁譂譃譄譅譆譇譈证譊譋譌譍谲讥譐譑譒譓譔譕谮譗识谯谭譛谱譝譞噪譠譡譢譣譤譥警譧譨譩譪谵譬譭譮译议譱譲譳谴譵譶护譸譹譺譻譼誉譾譿读讁讂讃讄讅讆讇讈讉变讋讌讍雠讏讐讑谗让讔谰谶讗讘讙赞讛谠讝谳讟讠计订讣认讥讦讧讨让讪讫讬训议讯记讱讲讳讴讵讶讷许讹论讻讼讽设访诀证诂诃评诅识诇诈诉诊诋诌词诎诏诐译诒诓诔试诖诗诘诙诚诛诜话诞诟诠诡询诣诤该详诧诨诩诪诫诬语诮误诰诱诲诳说诵诶请诸诹诺读诼诽课诿谀谁谂调谄谅谆谇谈谉谊谋谌谍谎谏谐谑谒谓谔谕谖谗谘谙谚谛谜谝谞谟谠谡谢谣谤谥谦谧谨谩谪谫谬谭谮谯谰谱谲谳谴谵谶谷谸谹谺谻谼谽谾谿豀豁豂豃豄豅豆豇岂豉豊豋豌豍竖豏丰豑豒豓艳豕豖豗豘豙豚豛豜豝豞豟豠象豢豣豤豥豦豧豨豩豪豫猪豭豮豯豰豱豲豳豴豵豶豷豸豹豺豻豼豽豾豿貀貁貂貃貄貅貆貇貈貉貊貋貌狸貎貏貐貑貒猫貔貕貖貗貘貙貚貛貜贝贞貟负财贡貣貤貥貦贫货贩贪贯责貭貮贮贳貱赀贰贵貵贬买贷貹贶费贴贻貾贸贺贲赂赁贿赅賆资贾賉贼賋賌賍賎賏賐赈赊宾賔赇賖賗賘賙赉賛赐賝赏賟赔赓贤卖贱賥赋赕賨賩质賫账赌賮賯賰賱賲賳赖賵賶賷賸賹赚赙购赛赜賿贀贁贂贃贽赘贆贇赠贉赞贋贌赡贎赢赆贑贒赃贔贕赎赝贘贙贚赣贜贝贞负贠贡财责贤败账货质贩贪贫贬购贮贯贰贱贲贳贴贵贶贷贸费贺贻贼贽贾贿赀赁赂赃资赅赆赇赈赉赊赋赌赍赎赏赐赑赒赓赔赕赖赗赘赙赚赛赜赝赞赟赠赡赢赣赤赥赦赧赨赩赪赫赬赭赮赯走赱赲赳赴赵赶起赸赹赺赻赼赽赾赿趀趁趂趃趄超趆趇趈趉越趋趌趍趎趏趐趑趒趓趔赶趖趗趘赵趚趛趜趝趞趟趠趡趢趣趤趥趦趧趋趩趪趫趬趭趮趯趰趱趱足趴趵趶趷趸趹趺趻趼趽趾趿跀跁跂跃跄跅跆跇跈跉跊跋跌跍跎跏跐跑跒跓跔跕跖跗跘跙跚跛跜距跞跟跠迹跢跣跤跥跦跧跨跩跪跫跬跭跮路跰跱跲跳跴践跶跷跸跹跺跻局跽跾跿踀踁踂踃踄踅踆踇踈踉踊踋踌踍踎踏践踑踒踓踔踕踖踗踘踙踚踛踜踝踞踟踠蜷踢踣踤踥踦踧踨踩踪碰踬踭踮踯踰踱踲踳踊踵踶踷踸踹踺踻踼踽踾踿蹀蹁蹂蹃蹄蹅蹆蹇蹈蹉蹊蹋跄蹍蹎蹏蹐蹑蹒蹓蹔跸蹖蹗蹘蹙蹚蹛蹜蹝蹞蹟跖蹡蹢蹒踪蹥蹦蹧蹨蹩蹪蹫蹬蹭蹮蹯蹰蹱蹲蹳蹴蹵蹶蹷蹸蹹跷蹻蹼蹽蹾蹿躀躁躂躃躄躅躆躇躈趸踌跻躌跃躎躏躐踯跞踬躔蹰躖躗躘躙跹躛躜躝躞躟躠蹑躢躣躤蹿躜躧躨躩躏身躬躭躮躯躰躱躲躳躴躵躶躷躸躹躺躻躼躽躾躿躯軁軂軃軄軅軆軇軈軉车轧轨军軎軏軐軑轩軓轫軕軖軗軘軙軚轭軜軝軞软軠軡転軣軤軥軦軧軨軩軪轸軬軭軮軯軰軱軲軳軴軵軶軷轴轵轺轲轶軽轼軿輀輁輂较輄辂輆辁輈载轾輋輌輍輎輏輐輑辄挽辅轻輖輗輘輙輚辆辎辉辋辍輠輡輢輣輤辊辇輧輨辈轮輫輬輭輮辑輰輱輲辏輴輵輶輷输輹輺辐輼輽辗舆轀轁毂轃辖辕辘轇轈转轊轋轌辙轿轏轐轑轒轓辚轕轖轗轘轙轚轛轜轝轞轰轠辔轹轣轳轥车轧轨轩轪轫转轭轮软轰轱轲轳轴轵轶轷轸轹轺轻轼载轾轿辀辁辂较辄辅辆辇辈辉辊辋辌辍辎辏辐辑辒输辔辕辖辗辘辙辚辛辜辝辞辟辠辡辢辣辤辥办辧辨辩辪辫辬辞辫辩辰辱农辳辴辵辶辷辸边辺辻込辽达辿迀迁迂迃迄迅迤过迈迉迊迋迌迍迎迏运近迒迓返迕迖迗还这迚进远违连迟迠迡迢迣迤迥迦迧迨迩迪迫迬迭迮迯述迱迲迳回迵迶迷迸迹乃迻迼追迾迿退送适逃逄逅逆逇逈选逊逋逌逍逎透逐逑递逓途迳逖逗逘这通逛逜逝逞速造逡逢连逤逥逦逧逨逩逪逫逬逭逮逯逰周进逳逴逵逶逷逸逹逺逻逼逽逾逿遀遁遂遃遄遅遆遇遈遉游运遌遍过遏遐遑遒道达违遖遗遘遥遚遛逊遝递遟远遡遢遣遤遥遦遧遨适遪遫遬遭遮遯遰遱迟遳遴遵遶迁选遹遗遻辽遽遾避邀迈邂邃还邅邆迩邈邉边邋邌邍邎逻逦邑邒邓邔邕邖邗邘邙邚邛邜邝邞邟邠邡邢那邤邥邦邧邨邩邪邫邬邭邮邯邰邱邲邳邴邵邶邷邸邹邺邻邼邽邾邿郀郁郂合郄郅郆郇郈郉郊郋郌郍郎郏郐郑郒郓郔郕郖郗郘郙郚郛郜郝郞郏郠郡郢郣郤郥郦郧部郩郪郫郬郭郮郯郰郱郲郳郴邮郶郷郸郹郺郻郼都郾郿鄀鄁鄂鄃鄄鄅郓鄇鄈乡鄊鄋鄌鄍鄎鄏鄐鄑邹鄓邬鄕郧鄗鄘鄙鄚鄛鄜鄝鄞鄟鄠鄡鄢鄣鄤鄥鄦邓鄨鄩鄪鄫鄬郑鄮鄯邻鄱郸鄳邺鄵郐鄷鄸鄹邝鄻鄼鄽鄾鄿酀酁酂酃酄酅酆酇郦酉酊酋酌配酎酏酐酑酒酓酔酕酖酗酘酙酚酛酜酝酞酟酠酡酢酣酤酥酦酧酨酩酪酫酬酭酮酯酰酱酲酳酴酵酶酷酸酹酺酻酼酽酾酿醀醁醂腌醄醅醆醇醈醉醊醋醌醍醎醏醐醑醒醓醔醕醖醗醘醙醚醛丑醝酝醟醠醡醢醣醤醥醦醧醨醩醪医酱醭醮醯醰醱醲醳醴醵醶醷醸醹醺醻醼醽醾醿酿衅釂酾釄酽釆采釈釉释释里重野量厘金釒钆钇钌釖钊钉钋釚釛釜针釞釟釠釡釢钓钐釥扣钏釨钒釪釫钎釭釮釯釰釱釲釳釴钗釶钍釸钕釺釻釼釽釾釿钯钫鈂鈃钭鈅鈆鈇鈈钠鈊鈋鈌钝鈎鈏钤钣鈒鈓钞钮鈖鈗鈘鈙鈚鈛鈜鈝钧鈟鈠鈡鈢钙鈤钬钛钪鈨鈩鈪鈫鈬鈭铌鈯铈鈱鈲钶铃鈵鈶钴钹铍钰鈻鈼钸铀钿钾鉁鉂鉃鉄钜钻鉇铊铉鉊刨鉌铋鉎鉏鉐铂鉒鉓鉔鉕鉖钳鉘鉙铆铅鉜鉝钺鉟鉠鉡鉢鉣钩鉥钲鉧鉨鉩鉪鉫钼钽鉮鉯鉰鉱鉲鉳鉴鉵鉶鉷铰鉹铒铬鉼鉽鉾铪银銁銂铳銄铜銆銇銈銉銊銋銌銍銎銏銐铣銒铨銔銕铢銗铭銙铫銛衔銝銞銟铑銡銢铷銤铱铟銧铵铥铕铯铐銭銮銯銰銱焊锐銴銵銶销銸锈銺锑锉銽銾銿鋀铝鋂锒鋄锌鋆钡鋈鋉鋊鋋铤鋍鋎铗鋐鋑锋鋓鋔鋕鋖鋗鋘鋙鋚鋛鋜锊鋞锓鋠鋡鋢鋣锄鋥锔鋧锇鋩铺鋫鋬鋭铖锆锂铽鋲鋳鋴鋵鋶鋷锯鋹鋺鋻钢鋽鋾鋿錀锞錂錃录錅锖錇锩錉錊錋錌錍錎錏锥錑锕錓錔锟錖錗锤锱铮锛錜錝錞锬锭錡钱錣錤錥锦錧锚錩錪锡錬錭锢错錰錱録锰錴錵表錷铼錹錺錻錼錽錾錿鍀鍁鍂鍃鍄鍅钔锴鍈鍉链锅鍌镀鍎鍏鍐鍑鍒鍓锷鍕鍖鍗铡鍙鍚锻鍜鍝鍞鍟鍠鍡鍢鍣锸锲鍦鍧鍨鍩鍪鍫锹鍭鍮鍯锾鍱鍲鍳鍴键锶鍷鍸鍹锗鍻鍼鍽锺鍿鎀鎁镁鎃鎄鎅鎆鎇鎈鎉镑鎋鎌鎍鎎鎏鎐鎑鎒鎓熔鎕锁枪镉鎙鎚鎛鎜鎝鎞鎟鎠鎡钨蓥鎤鎥镏铠鎨铩锼鎫镐鎭镇鎯镒鎱鎲镍鎴镓鎶鎷鎸鎹鎺鎻鎼鎽鎾鎿鏀鏁鏂镞鏄鏅鏆镟链鏉鏊鏋镆镙鏎鏏鏐镝鏒鏓鏔鏕鏖铿锵鏙鏚鏛镗镘镛铲鏠镜镖鏣镂鏥鏦鏧錾鏩鏪鏫鏬鏭鏮鏯鏰鏱鏲鏳鏴铧鏶镤鏸镪鏺鏻鏼锈鏾鏿鐀鐁鐂铙鐄鐅鐆鐇鐈铣鐊铴鐌鐍鐎鐏镣鐑铹镦镡鐕鐖鐗钟镫鐚鐛鐜鐝鐞鐟镨鐡鐢鐣鐤鐥鐦鐧镄鐩鐪镌鐬鐭镰鐯鐰鐱镯镭鐴铁鐶鐷铎鐹铛鐻鐼钽鐾镱鑀鑁鑂鑃铸鑅鑆鑇鑈鑉镬鑋镔鑍鑎鑏鑐监鉴鑓鑔鑕鑖鑗鑘鑙鑚鑛鑜鑝鑞鑟铄鑡鑢镳刨鑥鑦鑧鑨鑩鑪鑫鑬镧鑮鑯钥鑱镶鑳鑴鑵鑶镊鑸鑹鑺鑻锣钻銮凿钀钁钂钃钄钅钆钇针钉钊钋钌钍钎钏钐钑钒钓钔钕钖钗钘钙钚钛钜钝钞钟钠钡钢钣钤钥钦钧钨钩钪钫钬钭钮钯钰钱钲钳钴钵钶钷钸钹钺钻钼钽钾钿铀铁铂铃铄铅铆铇铈铉铊铋铌铍铎铏铐铑铒铓铔铕铖铗铘铙铚铛铜铝铞铟铠铡铢铣铤铥铦铧铨铩铪铫铬铭铮铯铰铱铲铳铴铵银铷铸铹铺铻铼铽链铿销锁锂锃锄锅锆锇锈锉锊锋锌锍锎锏锐锑锒锓锔锕锖锗锘错锚锛锜锝锞锟锠锡锢锣锤锥锦锧锨锩锪锫锬锭键锯锰锱锲锳锴锵锶锷锸锹锺锻锼锽锾锿镀镁镂镃镄镅镆镇镈镉镊镋镌镍镎镏镐镑镒镓镔镕镖镗镘镙镚镛镜镝镞镟镠镡镢镣镤镥镦镧镨镩镪镫镬镭镮镯镰镱镲镳镴镵镶长镸镹镺镻镼镽镾长门閁闩闪閄閅闫閇閈闭閊开闶閍闳闰閐闲闲间闵閕閖閗闸閙閚閛閜閝閞閟閠阂関阁合阀閦閧闺闽閪阃阆闾閮閯閰阅閲閳閴閵阊閷閸阉閺阎阏阍阈阌闀闁闂阒闄闅板闇闱闉阔阕阑闍闎闏阗闑闒闓阖阙闯闗闘闙闚闛关闝阚闟闠阐辟闣闤闼闦闧门闩闪闫闬闭问闯闰闱闲闳间闵闶闷闸闹闺闻闼闽闾闿阀阁阂阃阄阅阆阇阈阉阊阋阌阍阎阏阐阑阒阓阔阕阖阗阘阙阚阛阜阝阞队阠阡阢阣阤阥阦阧厄阩阪阫阬阭阮址阰阱防阳阴阵阶阷阸阹阺阻阼阽阾阿陀陁陂陃附际陆陇陈陉陊陋陌降陎陏限陑陒陓陔陕陖陗陉陙陚陛陕陕陞陟陠陡院阵除陥陦陧陨险陪陫陬陭陮陯阴陱陲陈陴陵陶陷陆陹険陻陼阳陾陿隀隁隂隃堤隅隆隇隈陧队隋隌隍阶随隐隑隒隓隔陨隖隗隘隙隚际障隝隞隟隠隡隢隣隤隥隦隧随隩险隫隬隭隮隯隰隐隲隳陇隵隶隷隶隹隺只隼隽难隿雀雁雂雃雄雅集雇雈雉雊隽雌雍雎雏雐雑雒雓雔雕虽雗雘双雚雏杂雝鸡雟雠雡离难雤雥雦雧雨雩雪雫雬雭雮雯雰雱云雳雴雵零雷雸雹雺电雼雽雾雿需霁霂霃霄霅霆震霈霉霊霋霌霍霎霏霐沾霒霓霔霕霖霗霘霙霚霛霜霝霞霟霠霡霢霣溜霥霦雾霨霩霪霫霬霭霮霯霰霱露霳霴霵霶霷霸霹霺霻霼霁霾霿靀靁雳靃霭靅靆靇灵靉靊靋靌靍靎靏靐靑青靓靔靕靖靗靘静靓靛静靝非靟靠靡面靣靤靥腼靧靥革靪靫靬靭靮靯靰靱靲靳靴靵靶靷靸靹靺靻靼靽靾靿鞀鞁鞂鞃鞄鞅鞆鞇鞈鞉鞊鞋鞌鞍鞎巩鞐鞑鞒鞓鞔鞕鞖鞗鞘鞙鞚鞛鞜鞝鞞鞟鞠鞡鞢鞣鞤鞥秋鞧鞨鞩鞪鞫鞬鞭鞮鞯鞰鞱鞲鞳鞴鞵鞶鞷鞸鞹鞺鞻鞼鞽鞾鞿韀缰韂鞑韄韅千韇韈鞯韊韦韧韍韎韏韐韑韒韩韔韕韖韗韘韪韚韛韬韝韫韟韠韡韢韣韤韥韦韧韨韩韪韫韬韭韮韯韰韱韲音韴韵韶韷韸韹韺韵韼韽韾响頀页顶顷頄项顺顸须頉顼頋颂頍颀颃预顽颁顿頔頕頖颇领頙頚頛颌頝頞頟頠颉頢頣颐頥颏頧頨頩頪頫頬头頮頯颊頱頲頳頴頵頶颔颈颓頺频頼頽頾頿顀顁顂顃顄顅颗顇顈顉顊顋题额颚颜顐顑顒颛顔顕顖顗愿颡顚颠顜顝类顟顠顡颟顣顤颢顦顾顨顩顪颤顬顭顮显颦颅顲颞颧页顶顷顸项顺须顼顽顾顿颀颁颂颃预颅领颇颈颉颊颋颌颍颎颏颐频颒颓颔颕颖颗题颙颚颛颜额颞颟颠颡颢颣颤颥颦颧风颩颪颫颬颭飑飒颰台颲刮颴颵飓颷颸颹扬颻飕颽颾颿飀飁飂飃飘飅飙飇飈飉飊飋飌飍风飏飐飑飒飓飔飕飖飗飘飙飚飞飜飝飞食飠飡饥飣飤飥飦飧飨饨饪饫飬饬飮饭飰飱饮飳饴飵飶飷飸飹飺飻饲饱饰飿餀餁餂饺餄饼餆餇餈饷养餋饵餍餎餏餐饽馁饿餔餕餖餗余餙肴馄餜餝饯餟餠馅餢餣餤餥餦餧馆餩餪餫餬餭餮餯餰餱餲饧餴喂餶餷餸餹餺餻饩馈馏馊饀饁饂馍饄馒饆饇馐馑饊馈馔饍饎饏饐饥饶饓饔饕饖飨饘饙饚饛餍饝馋饟饠饡饢饣饤饥饦饧饨饩饪饫饬饭饮饯饰饱饲饳饴饵饶饷饸饹饺饻饼饽饾饿馀馁馂馃馄馅馆馇馈馉馊馋馌馍馎馏馐馑馒馓馔馕首馗馘香馚馛馜馝馞馟馠馡馢馣馤馥馦馧馨馩馪馫马驭冯馯馰驮馲驰驯馵馶馷馸馹馺馻馼馽馾馿駀驳駂駃駄駅駆駇駈駉駊駋駌駍駎駏驻驽驹駓驵驾駖駗骀驸駚驶駜驼駞驷駠駡骈駣駤駥駦駧駨駩駪駫駬骇駮駯駰骆駲駳駴駵駶駷駸駹駺駻駼駽駾骏騀骋騂騃騄骓騆騇騈騉騊騋騌骒骑骐騐騑騒験騔騕骛騗騘骗騚騛騜騝騞騟騠騡騢騣騤騥騦騧騨騩騪骞騬骘骝騯腾騱騲騳騴騵驺骚骟騹騺騻騼騽骡騿蓦骜骖骠骢驱驆驇驈驉骅驋驌骁驎骣驐驑驒驓驔骄驖验驘驙惊驿驜驝驞骤驠驡驴驣骧骥驦驧驨驩骊驫马驭驮驯驰驱驲驳驴驵驶驷驸驹驺驻驼驽驾驿骀骁骂骃骄骅骆骇骈骉骊骋验骍骎骏骐骑骒骓骔骕骖骗骘骙骚骛骜骝骞骟骠骡骢骣骤骥骦骧骨骩骪骫骬骭骮肮骰骱骲骳骴骵骶骷骸骹骺骻骼骽骾骿髀髁髂髃髄髅髆髇髈髉髊髋髌髍髎髅髐髑脏髓体髌髋髗高髙髚髛髜髝髞髟髠髡髢髣髤髥髦髧髨髩髪髫髬髭发髯髰髱髲髳髴髵髶髷髸髹髺髻髼髽髾髿鬀鬁鬂鬃鬄鬅松鬇鬈鬉鬊鬋鬌胡鬎鬏鬐鬑鬒鬓鬔鬕鬖鬗鬘鬙须鬛鬜鬝鬞鬟鬠鬡鬓鬣鬤斗鬦闹哄阋鬪鬫鬬鬭阄鬯鬰郁鬲鬳鬴鬵鬶鬷鬸鬹鬺鬻鬼鬽鬾鬿魀魁魂魃魄魅魆魇魈魉魊魋魌魍魉魏魐魑魒魓魔魕魖魗魇魙鱼魛魜魝魞魟魠魡魢魣魤魥魦魧魨魩魪魫魬魭魮鲁魰魱魲魳鲂魵魶鱿魸魹魺魻魼魽魾魿鮀鮁鮂鮃鮄鮅鮆鮇鮈鮉鮊鮋鮌鮍鮎鮏鲐鲍鲋鮓鮔鮕鮖鮗鮘鮙鲒鮛鮜鮝鲕鮟鮠鮡鮢鮣鮤鮥鮦鮧鮨鮩鲔鲛鮬鲑鲜鮯鮰鮱鮲鮳鮴鮵鮶鮷鮸鮹鮺鮻鮼鮽鮾鮿鲧鲠鯂鯃鯄鯅鯆鲩鯈鲤鲨鯋鯌鯍鯎鯏鯐鯑鯒鯓鲻鯕鲭鯗鯘鯙鯚鲷鯜鯝鯞鯟鯠鲱鲵鯣鲲鯥鯦鲳鲸鯩鲮鲰鯬鯭鯮鯯鲶鯱鯲鯳鯴鯵鯶鯷鯸鯹鯺鯻鯼鲫鯾鯿鰀鰁鰂鰃鰄鰅鰆鰇鲽鳇鰊鰋鰌鳅鰎鰏鰐鰑鳆鳃鰔鰕鰖鰗鰘鰙鰚鰛鰜鰝鰞鰟鰠鰡鰢鲥鰤鳏鰦鰧鳎鳐鰪鰫鰬鳍鰮鰯鰰鲢鳌鳓鰴鰵鰶鲦鰸鲣鰺鳗鰼鰽鳔鰿鱀鱁鱂鱃鱄鱅鱆鱇鳕鳖鱊鱋鱌鱍鱎鱏鱐鱑鳟鱓鳝鱕鳜鳞鲟鱙鱚鱛鱜鱝鱞鲎鱠鱡鱢鱣鱤鱥鱦鳢鱨鱩鱪鱫鱬鲚鱮鱯鱰鱱鱲鱳鱴鱵鱶鳄鲈鱹鲡鱻鱼鱽鱾鱿鲀鲁鲂鲃鲄鲅鲆鲇鲈鲉鲊鲋鲌鲍鲎鲏鲐鲑鲒鲓鲔鲕鲖鲗鲘鲙鲚鲛鲜鲝鲞鲟鲠鲡鲢鲣鲤鲥鲦鲧鲨鲩鲪鲫鲬鲭鲮鲯鲰鲱鲲鲳鲴鲵鲶鲷鲸鲹鲺鲻鲼鲽鲾鲿鳀鳁鳂鳃鳄鳅鳆鳇鳈鳉鳊鳋鳌鳍鳎鳏鳐鳑鳒鳓鳔鳕鳖鳗鳘鳙鳚鳛鳜鳝鳞鳟鳠鳡鳢鳣鳤鸟鳦凫鳨鸠鳪鳫鳬鳭鳮鳯鳰鳱鳲凤鸣鳵鸢鳷鳸鳹鳺鳻鳼鳽鳾鳿鴀鴁鴂鴃鴄鴅鸩鸨鴈鸦鴊鴋鴌鴍鴎鴏鴐鴑鴒鴓鴔鸵鴖鴗鴘鴙鴚鸳鴜鸲鴞鸱鴠鴡鴢鸪鴤鴥鸯鴧鸭鴩鴪鴫鴬鴭鴮鸸鸹鴱鴲鴳鴴鴵鴶鴷鴸鴹鴺鸿鴼鴽鴾鸽鵀鵁鸺鵃鵄鵅鵆鵇鵈鵉鵊鵋鵌鵍鵎鵏鵐鹃鹆鹁鵔鵕鵖鵗鵘鵙鵚鵛鹈鹅鵞鵟鹄鹉鵢鵣鵤鵥鵦鵧鵨鵩鹌鵫鹏鵭鵮鹎鵰鵱鹊鵳鵴鵵鵶鵷鵸鵹鵺鵻鵼鵽鵾鵿鶀鶁鶂鶃鶄鶅鶆鸫鶈鹑鶊鶋鶌鶍鶎鶏鶐鶑鶒鶓鶔鶕鶖鶗鹕鶙鹗鶛鶜鶝鶞鶟鶠鶡鶢鶣鶤鶥鶦鶧鶨鹜鶪鶫鶬鶭鶮莺鶰骞鶲鶳鹤鶵鶶鶷鶸鶹鶺鹘鹣鶽鶾鹚鷀鷁鹞鷃鷄鷅鷆鷇鷈鷉鷊鷋鷌鷍鷎鷏鷐鷑鷒鹧鷔鷕鷖鸥鷘鸷鹨鷛鷜鷝鷞鷟鷠鷡鷢鷣鷤鸶鹪鷧鷨鷩鷪鷫鷬鷭鷮鹩鷰鷱鹫鹇鹇鷵鷶鷷鹬鹰鹭鷻鷼鷽鷾鷿鸀鸁鸂鸃鸄鸅鸆鸇鸈鸉鸊鸋鸌鸍鸎鸏鸐鸑鸒鸓鸔鸬鸖鸗鸘鸙鹦鹳鸜鹂鸾鸟鸠鸡鸢鸣鸤鸥鸦鸧鸨鸩鸪鸫鸬鸭鸮鸯鸰鸱鸲鸳鸴鸵鸶鸷鸸鸹鸺鸻鸼鸽鸾鸿鹀鹁鹂鹃鹄鹅鹆鹇鹈鹉鹊鹋鹌鹍鹎鹏鹐鹑鹒鹓鹔鹕鹖鹗鹘鹙鹚鹛鹜鹝鹞鹟鹠鹡鹢鹣鹤鹥鹦鹧鹨鹩鹪鹫鹬鹭鹮鹯鹰鹱鹲鹳鹴卤鹶鹷鹸咸鹾鹻硷盐鹾鹿麀麁麂麃麄麅麆麇麈麉麊麋麌麍麎麏麐麑麒麓麔麕麖丽麘麙麚麛麜麝麞麟麠麡麢麣麤麦麦麧麨麸麪麫麬麭麮麯麰麱麲麳麴面麶麷麸麹麺麻麽麽麾麿黀黁黂黄黄黅黆黇黈黉黊黋黉黍黎黏黐黑黒黓黔黕黖黗默黙黚黛黜黝点黟黠黡黢黣黤黥黦黧党黩黪黫黬黭黮黯黰黱黪黳霉黵黶黩黸黹黺黻黼黾黾鼋鼀鼁鼂鼃鼄鼅鼆鳌鼈鼍鼊鼋鼌鼍鼎鼏鼐鼑鼒鼓鼔冬鼖鼗鼘鼙鼚鼛鼜鼝鼞鼟鼠鼡鼢鼣鼤鼥鼦鼧鼨鼩鼪鼫鼬鼭鼮鼯鼰鼱鼲鼳鼹鼵鼶鼷鼸鼹鼺鼻鼼鼽鼾鼿齀齁齂齃齄齅齆齇齈齉齐斋齌齍齎齑齐齑齿齓龀齕齖齗齘龅齚齛龇齝齞龃龆龄齢出齤齥龈啮齨齩龊齫龉齭齮齯齰齱龋齳齴齵齶龌齸齹齺齻齼齽齾齿龀龁龂龃龄龅龆龇龈龉龊龋龌龙龎龏庞龑龒龓龚龛龖龗龘龙龚龛龟龝龞龟龠龡龢龣龤龥";

            /// <summary>
            /// 繁体字
            /// </summary>
            string Traditional = "一丁丂七丄丅丆萬丈三上下丌不與丏丐丑丒專且丕世丗丘丙業叢東絲丞丟丠両丟丣兩嚴並喪丨丩個丫丬中丮丯豐丱串丳臨丵丶丷丸丹為主丼麗舉丿乀乁乂乃乄久乆乇么義乊之烏乍乎乏樂乑乒乓喬乕乖乗乘乙乚乛乜九乞也習鄉乢乣乤乥書乧乨乩乪乫乬乭乮乯買亂乲乳乴乵乶乷乸乹乺乻乼乽乾乿亀亁亂亃亄亅了亇予爭亊事二亍于虧亐云互亓五井亖亗亙亙亞些亜亝亞亟亠亡亢亣交亥亦產亨畝亪享京亭亮亯亰亱親亳亴褻亶亷亸亹人亻亼亽亾億什仁仂仃仄僅仆仇仈仉今介仌仍從仏仐侖仒倉仔仕他仗付仙仚仛仜仝仞仟仠仡仢代令以仦仧仨仩儀仫們仭仮仯仰仱仲仳仴仵件價仸仹仺任仼份仾仿伀企伂伃伄伅伆伇伈伉伊伋伌伍伎伏伐休伒伓伔伕伖眾優伙會傴伜伝傘偉傳伡伢伣傷倀倫傖伨伩偽佇伬伭伮伯估伱伲伳伴伵伶伷伸伹伺伻似伽伾伿佀佁佂佃佄佅但佇佈佉佊佋佌位低住佐佑佒體佔何佖佗佘余佚佛作佝佞佟你佡佢傭佤僉佦佧佨佩佪佫佬佭佮佯佰佱佲佳佴併佶佷佸佹佺佻佼佽佾使侀侁侂侃侄侅來侇侈侉侊例侌侍侎侏侐侑侒侓侔侕侖侗侘侙侚供侜依侞侟俠価侢侶侤僥偵側僑儈儕侫儂侭侮侯侰侱侲侳侴侵侶侷侸侹侺侻侼侽侾便俀俁係促俄俅俆俇俈俉俊俋俌俍俎俏俐俑俒俓俔俕俖俗俘俙俚俛俜保俞俟俠信俢俁俤俥儔俧儼倆儷俫俬儉修俯俰俱俲俳俴俵俶俷俸俹俺俻俼俽俾俿倀倁倂倃倄倅倆倇倈倉倊個倌倍倎倏倐們倒倓倔倕倖倗倘候倚倛倜倝倞借倠倡倢倣値倥倦倧倨倩倪倫倬倭倮倯倰倱倲倳倴倵倶倷倸倹債倻值倽傾倿偀偁偂偃偄偅偆假偈偉偊偋偌偍偎偏偐偑偒偓偔偕偖偗偘偙做偛停偝偞偟偠偡偢偣偤健偦偧偨偩偪偫傯偭偮偯偰偱偲偳側偵偶偷偸偹偺僂偼偽僨償傀傁傂傃傄傅傆傇傈傉傊傋傌傍傎傏傐傑傒傓傔傕傖傗傘備傚傛傜傝傞傟傠傡傢傣傤儻傦儐儲儺傪傫催傭傮傯傰傱傲傳傴債傶傷傸傹傺傻傼傽傾傿僀僁僂僃僄僅僆僇僈僉僊僋僌働僎像僐僑僒僓僔僕僖僗僘僙僚僛僜僝僞僟僠僡僢僣僤僥僦僧僨僩僪僫僬僭僮僯僰僱僲僳僴僵僶僷僸價僺僻僼僽僾僿儀儁儂儃億儅儆儇儈儉儊儋儌儍儎儏儐儑儒儓儔儕儖儗儘儙儚儛儜儝儞償儠儡儢儣儤儥儦儧儨儩優儫儬儭儮儯儰儱儲儳儴儵儶儷儸儹儺儻儼儽儾兒兀允兂元兄充兆兇先光兊克兌免兎兏児兌兒兓兔兕兗兗兘兙黨兛兜兝兞兟兠兡兢兣兤入兦內全兩兪八公六兮兯蘭共兲關興兵其具典茲兺養兼獸兾兿冀囅冂冃冄內円冇岡冉冊冋冊再冎冏冐冑冒冓冔冕冖冗冘寫冚軍農冝冞冟冠冡冢冣冤冥冦冧冨冩冪冫冬冭冮馮冰冱沖決冴況冶冷冸冹冺凍冼冽冾冿凈凁凂凃凄凅準凇凈涼凊凋凌凍凎減凐湊凒凓凔凕凖凗凘凙凚凜凜凝凞凟幾凡凢凣鳳凥処凧凨凩凪鳧凬憑凮凱凰凱凲凳凴凵兇凷凸凹出擊凼函凾鑿刀刁刂刃刄刅分切刈刉刊刋刌芻刎刏刐刑劃刓刔刕刖列劉則剛創刜初刞刟刪刡刢刣判別刦刧刨利刪別刬剄刮刯到刱刲刳刴刵制刷券剎刺刻刼劊刾劌剴剁劑剃剄剅剆則剈剉削剋剌前剎剏剮劍剒剓剔剕剖剗剘剙剚剛剜剝剞剟剠剡剢剣剤剝剦劇剨剩剪剫剬剭剮副剰剱割剳剴創剶剷剸剹剺剻剼剽剾剿劀劁劂劃劄劅劆劇劈劉劊劋劌劍劎劏劐劑劒劓劔劕劖劗劘劙劚力劜勸辦功加務勱劣劤劥劦劧動助努劫劬劭劮劯劰勵勁勞労劵劶劷劸効劺劻劼劽劾勢勀勁勂勃勄勅勆勇勈勉勊勛勌勍勎勏勐勑勒勓勔動勖勗勘務勚勛勜勝勞募勠勡勢勣勤勥勦勧勨勩勪勫勬勭勮勯勰勱勲勳勴勵勶勷勸勹勺勻勼勽勾勿勻匁匂匃匄包匆匇匈匉匊匋匌匍匎匏匐匑匒匓匔匕化北匘匙匚匛匜匝匞匟匠匡匢匣匤匥匭匧匨匩匪匫匬匭匱匯匰匱匲匳匴匵匶匷匸匹區醫匼匽匾匿區十卂千卄卅卆升午卉半卋卌卍華協卐卑卒卓協單賣南単卙博卛卜卝卞卟占卡盧卣鹵卥卦臥卨卩卪衛卬卭卮卯印危卲即卻卵卶卷卸卹巹卻卼卽卾卿厀厁廠厃厄廳歷厇厈厲厊壓厭厙厎厏厐厑厒厓厔廁厖厗厘厙厚厛厜厝厞原厠厡廂厴厤厥廈厧廚廄厪厫厬厭廝厯厰厱厲厳厴厵厶厷厸厹厺去厼厽厾縣叀叁參參叄叅叆叇又叉及友雙反収叏叐發叒叓叔叕取受變敘叚叛叜叝叞叟疊叡叢口古句另叧叨叩只叫召叭叮可臺叱史右叴叵葉號司嘆叺叻叼嘰叾叿吀吁吂吃各吅吆吇合吉吊吋同名后吏吐向吒嚇吔呂吖嗎吘吙吚君吜吝吞吟吠吡吢吣吤吥否吧噸吩吪含聽吭吮啟吰吱吲吳吳吵吶吷吸吹吺吻吼吽吾吿呀呁呂呃呄呅呆呇呈呉告呋呌呍呎呏吶呑嘸囈呔嘔嚦唄員咼呚嗆嗚呝呞呟呠呡呢呣呤呥呦呧周呩呪呫呬呭呮呯呰呱呲味呴呵呶呷呸呹呺呻呼命呾呿咀咁咂咃咄咅咆咇咈咉咊咋和咍咎詠咐咑咒咓咔咕咖咗咘嚨咚嚀咜咝咞咟咠咡咢咣咤咥咦咧咨咩咪咫咬咭咮咯咰咱咲咳咴咵咶咷咸咹咺咻咼咽咾咿哀品哂哃哄哅哆哇哈哉哊哋哌響哎哏哐啞噠嘵嗶噦哖嘩哘噲哚哛嚌噥哞喲哠員哢哣哤哥哦哧哨哩哪哫哬哭哮哯哰哱哲哳哴哵哶哷哸哹哺哻哼哽哾哿唀唁唂唃唄唅唆唇唈唉唊唋唌唍唎唏唐唑唒唓唔唕唖唗唘唙唚嘜唜唝唞唟嘮唡嗩唣喚唥唦唧唨唩唪唫唬唭售唯唰唱唲唳唴唵唶唷唸唹唺唻唼唽唾唿啀啁啂啃啄啅商啇啈啉啊啋啌啍啎問啐啑啒啓啔啕啖啗啘啙啚啛啜啝啞啟啠啡啢啣啤啥啦嘖啨啩啪啫嗇囀嚙啯啰啱啲啳啴啵啶啷嘯啹啺啻啼啽啾啿喀喁喂喃善喅喆喇喈喉喊喋喌喍喎喏喐喑喒喓喔喕喖喗喘喙喚喛喜喝喞喟喠喡喢喣喤喥喦喧喨喩喪喫喬喭單喯喰喱喲喳喴喵営噴喸喹喺喻喼嘍嚳喿嗀嗁嗂嗃嗄嗅嗆嗇嗈嗉嗊嗋嗌嗍嗎嗏嗐嗑嗒嗓嗔嗕嗖嗗嗘嗙嗚嗛嗜嗝嗞嗟嗠嗡嗢嗣嗤嗥嗦嗧嗨嗩嗪囁嗬嗭嗮嗯嗰嗱嗲噯嗴嗵嗶嗷嗸嗹嗺嗻嗼嗽嗾嗿嘀嘁嘂嘃嘄嘅嘆嘇嘈嘉嘊嘋嘌嘍嘎嘏嘐嘑嘒嘓嘔嘕嘖嘗噓嘙嘚嘛嘜嘝嘞嘟嘠嘡嘢嘣嚶嘥嘦嘧嘨嘩嘪嘫嘬嘭嘮嘯嘰囑嘲嘳嘴嘵嘶嘷嘸嘹嘺嘻嘼嘽嘾嘿噀噁噂噃噄噅噆噇噈噉噊噋噌噍噎噏噐噑噒噓噔噕噖噗噘噙噚噛嚕噝噞噟噠噡噢噣噤噥噦噧器噩噪噫噬噭噮噯噰噱噲噳噴噵噶噷噸噹噺噻噼噽噾噿嚀嚁嚂嚃嚄嚅嚆嚇嚈嚉嚊嚋嚌嚍嚎嚏嚐嚑嚒嚓嚔嚕嚖嚗嚘嚙嚚嚛嚜嚝嚞嚟嚠嚡嚢囂嚤嚥嚦嚧嚨嚩嚪嚫嚬嚭嚮嚯嚰嚱嚲嚳嚴嚵嚶嚷嚸嚹嚺嚻嚼嚽嚾嚿囀囁囂囃囄囅囆囇囈囉囊囋囌囍囎囏囐囑囒囓囔囕囖囗囘囙囚四囜囝回囟因囡團団囤囥囦囧囨囩囪囫囬園囮囯困囪囲図圍圇囶囷囸囹固囻囼國圖囿圀圁圂圃圄圅圓圇圈圉圊國圌圍圎圏圐圑園圓圔圕圖圗團圙圚圛圜圝圞土圠圡圢圣圤圥圦圧在圩圪圫圬圭圮圯地圱圲圳圴圵圶圷圸壙場圻圼圽圾圿址坁坂坃坄坅坆均坈坉坊坋坌坍坎壞坐坑坒坓坔坕坖塊坘坙堅壇壢壩塢墳墜坡坢坣坤坥坦坧坨坩坪坫坬坭坮坯坰坱坲坳坴坵坶坷坸坹坺坻坼坽坾坿垀垁垂垃壟垅壚垇垈垉垊型垌垍垎垏垐垑壘垓垔垕垖垗垘垙垚垛垜垝垞垟垠垡垢垣垤垥墾坰垨堊垪墊垬埡垮垯垰垱塏垳垴垵垶垷垸垹垺垻垼垽垾垿埀埁埂埃埄埅埆埇埈埉埊埋埌埍城埏埐埑埒埓埔埕埖埗塒塤堝埛埜埝埞域埠埡埢埣埤埥埦埧埨埩埪埫埬埭埮埯埰埱埲埳埴埵埶執埸培基埻埼埽埾埿堀堁堂堃堄堅堆堇堈堉堊堋堌堍堎堏堐塹堒堓堔墮堖堗堘堙堚堛堜堝堞堟堠堡堢堣堤堥堦堧堨堩堪堫堬堭堮堯堰報堲堳場堵堶堷堸堹堺堻堼堽堾堿塀塁塂塃塄塅塆塇塈塉塊塋塌塍塎塏塐塑塒塓塔塕塖塗塘塙塚塛塜塝塞塟塠塡塢塣塤塥塦塧塨塩塪填塬塭塮塯塰塱塲塳塴塵塶塷塸塹塺塻塼塽塾塿墀墁墂境墄墅墆墇墈墉墊墋墌墍墎墏墐墑墑墓墔墕墖増墘墻墚墛墜墝增墟墠墡墢墣墤墥墦墧墨墩墪墫墬墭墮墯墰墱墲墳墴墵墶墷墸墹墺墻墼墽墾墿壀壁壂壃壄壅壆壇壈壉壊壋壌壍壎壏壐壑壒壓壔壕壖壗壘壙壚壛壜壝壞壟壠壡壢壣壤壥壦壧壨壩壪士壬壭壯壯聲壱売殼壴壵壺壷壸壹壺壻壼壽壾壿夀夁夂夃處夅夆備夈変夊夋夌復夎夏夐夑夒夓夔夕外夗夘夙多夛夜夝夞夠夠夡夢夣夤夥夦大夨天太夫夬夭央夯夰失夲夳頭夵夶夷夸夾奪夻夼夽夾夿奀奩奐奃奄奅奆奇奈奉奊奮奌奍奎奏奐契奒奓奔奕獎套奘奙奚奛奜奝奞奟奠奡奢奣奤奧奦奧奨奩奪奫奬奭奮奯奰奱奲女奴奵奶奷奸她奺奻奼好奾奿妀妁如妃妄妅妝婦媽妉妊妋妌妍妎妏妐妑妒妓妔妕妖妗妘妙妚妛妜妝妞妟妠妡妢妣妤妥妦妧妨嫵嫗媯妬妭妮妯妰妱妲妳妴妵妶妷妸妹妺妻妼妽妾妿姀姁姂姃姄姅姆姇姈姉姊始姌姍姎姏姐姑姒姓委姕姖姍姘姙姚姛姜姝姞姟姠姡姢姣姤姥姦姧姨姩姪姫姬姭姮姯姰姱姲姳姴姵姶姷姸姹姺姻姼姽姾姿娀威娂娃婁婭嬈嬌孌娉娊娋娌娍娎娏娐娑娒娓娔娕娖娗娘娙娚娛娜娝娞娟娠娡娢娣娤娥娦娧娨娩娪娫娬娭娮娯娰娛媧娳嫻娵娶娷娸娹娺娻娼娽娾娿婀婁婂婃婄婅婆婇婈婉婊婋婌婍婎婏婐婑婒婓婔婕婖婗婘婙婚婛婜婝婞婟婠婡婢婣婤婥婦婧婨婩婪婫婬婭婮婯婰婱婲婳嬰嬋嬸婷婸婹婺婻婼婽婾婿媀媁媂媃媄媅媆媇媈媉媊媋媌媍媎媏媐媑媒媓媔媕媖媗媘媙媚媛媜媝媞媟媠媡媢媣媤媥媦媧媨媩媼媫媬媭媮媯媰媱媲媳媴媵媶媷媸媹媺媻媼媽媾媿嫀嫁嫂嫃嫄嫅嫆嫇嫈嫉嫊嫋嫌嫍嫎嫏嫐嫑嬡嫓嬪嫕嫖嫗嫘嫙嫚嫛嫜嫝嫞嫟嫠嫡嫢嫣嫤嫥嫦嫧嫨嫩嫪嫫嫬嫭嫮嫯嫰嬙嫲嫳嫴嫵嫶嫷嫸嫹嫺嫻嫼嫽嫾嫿嬀嬁嬂嬃嬄嬅嬆嬇嬈嬉嬊嬋嬌嬍嬎嬏嬐嬑嬒嬓嬔嬕嬖嬗嬘嬙嬚嬛嬜嬝嬞嬟嬠嬡嬢嬣嬤嬥嬦嬧嬨嬩嬪嬫嬬嬭嬮嬯嬰嬱嬲嬳嬴嬵嬶嬤嬸嬹嬺嬻嬼嬽嬾嬿孀孁孂孃孄孅孆孇孈孉孊孋孌孍孎孏子孑孒孓孔孕孖字存孫孚孛孜孝孞孟孠孡孢季孤孥學孧孨孩孿孫孬孭孮孯孰孱孲孳孴孵孶孷學孹孺孻孼孽孾孿宀寧宂它宄宅宆宇守安宊宋完宍宎宏宐宑宒宓宔宕宖宗官宙定宛宜寶實実寵審客宣室宥宦宧宨宩憲宮宬宭宮宯宰宱宲害宴宵家宷宸容宺宻宼寬賓宿寀寁寂寃寄寅密寇寈寉寊寋富寍寎寏寐寑寒寓寔寕寖寗寘寙寚寛寜寢寞察寠寡寢寣寤寥實寧寨審寪寫寬寭寮寯寰寱寲寳寴寵寶寷寸對寺尋導寽対壽尀封専尃射尅將將專尉尊尋尌對導小尐少尒尓爾尕尖尗塵尙尚尛尜嘗尞尟尠尡尢尣尤尥尦堯尨尩尪尫尬尭尮尯尰就尲尳尷尵尶尷尸尹尺尻尼盡尾尿局屁層屃屄居屆屇屈屜屆屋屌屍屎屏屐屑屒屓屔展屖屗屘屙屚屛屜屝屬屟屠屢屢屣層履屨屧屨屩屪屫屬屭屮屯屰山屲屳屴屵屶屷屸屹屺屻屼屽屾嶼岀歲豈岃岄岅岆岇岈岉岊岋岌岍岎岏岐岑岒岓岔岕嶇崗峴岙嵐島岜岝岞岟岠岡岢岣岤岥岦岧岨巖岪岫岬嶺岮岯岰岱岲岳岴岵岶岷岸岹岺岻岼崠岾巋峀峁峂峃嶧峅峆峇峈峉峊峋峌峍峎峏峐峑峒峓峔峕峖峗峘峙峚峛峜峝峞峟峠峽峢峣嶠崢巒峧峨峩峪峫峬峭峮峯峰峱峲峳峴峵島峷峸峹峺峻峼峽峾峿崀崁嶗崍崄崅崆崇崈崉崊崋崌崍崎崏崐崑崒崓崔崕崖崗崘崙崚崛崜崝崞崟崠崡崢崣崤崥崦崧崨崩崪崫崬嶄崮崯崰崱崲崳崴崵崶崷崸崹崺崻崼崽崾崿嵀嵁嵂嵃嵄嵅嵆嵇嵈嵉嵊嵋嵌嵍嵎嵏嵐嵑嵒嵓嵔嵕嵖嵗嶸嵙嵚崳嵜嶁嵞嵟嵠嵡嵢嵣嵤嵥嵦嵧嵨嵩嵪嵫嵬嵭嵮嵯嵰嵱嵲嵳嵴嵵嵶嵷嵸嵹嵺嵻嵼嵽嵾嵿嶀嶁嶂嶃嶄嶅嶆嶇嶈嶉嶊嶋嶌嶍嶎嶏嶐嶑嶒嶓嶔嶕嶖嶗嶘嶙嶚嶛嶜嶝嶞嶟嶠嶡嶢嶣嶤嶥嶦嶧嶨嶩嶪嶫嶬嶭嶮嶯嶰嶱嶲嶳嶴嶵嶶嶷嶸嶹嶺嶻嶼嶽嶾嶿巀巁巂巃巄巔巆巇巈巉巊巋巌巍巎巏巐巑巒巓巔巕巖巗巘巙巚巛巜川州巟巠巡巢巣巤工左巧巨鞏巪巫巬巭差巰巰己已巳巴巵巶巷巸巹巺巻巼巽巾巿帀幣市布帄帥帆帇師帉帊帋希帍帎幃帳帑帒帓帔帕帖帗簾帙帚帛幟帝帞帟帠帡帢帣帤帥帶幀帨帩帪師帬席幫帯帰幬帲帳帴帵帶帷常帹帺幘幗帽帾帿幀幁冪幃幄幅幆幇幈幉幊幋幌幍幎幏幐幑幒幓幔幕幖幗幘幙幚幛幜幝幞幟幠幡幢幣幤幥幦幧幨幩幪幫幬幭幮幯幰幱干平年幵并幷幸幹幺幻幼幽幾廣庀庁庂広莊庅慶庇庈庉床庋庌庍庎序廬廡庒庫應底庖店庘廟庚庛府庝龐廢庠庡庢庣庤庥度座庨庩庪庫庬庭庮庯庰庱庲庳庴庵庶康庸庹庺庻庼庽庾庿廀廁廂廃廄廅廆廇廈廉廊廋廌廍廎廏廐廑廒廓廔廕廖廗廘廙廚廛廜廝廞廟廠廡廢廣廤廥廦廧廨廩廩廫廬廭廮廯廰廱廲廳廴廵延廷廸廹建廻廼廽廾廿開弁異棄弄弅弆弇弈弉弊弋弌弍弎式弐弒弒弓弔引弖弗弘弙弚弛弜弝弞弟張弡弢弣弤彌弦弧弨弩弳弫弬弭弮彎弰弱弲弳弴張弶強弸彈強弻弼弽弾弿彀彁彂彃彄彅彆彇彈彉彊彋彌彍彎彏彐彑歸當彔錄彖彗彘彙彚彛彜彝彞彟彠彡形彣彤彥彥彧彨彩彪彫彬彭彮彯彰影彲彳彴彵彶彷彸役彺徹彼彽彾彿往征徂徃徑待徆徇很徉徊律後徍徎徏徐徑徒従徔徠徖得徘徙徚徛徜徝從徟徠御徢徣徤徥徦徧徨復循徫徬徭微徯徰徱徲徳徴徵徶德徸徹徺徻徼徽徾徿忀忁忂心忄必憶忇忈忉忊忋忌忍忎懺忐忑忒忓忔忕忖志忘忙忚忛応忝忞忟忠忡忢忣忤忥忦憂忨忩忪快忬忭忮忯忰忱忲忳忴念忶忷忸忹忺忻忼忽愾忿懷態慫憮慪悵愴怇怈怉怊怋怌怍怎怏怐怑怒怓怔怕怖怗怘怙怚怛憐思怞怟怠怡怢怣怤急怦性怨怩怪怫怬怭怮怯怰怱怲怳怴怵怶怷怸怹怺總懟怽怾懌恀恁恂恃恄恅恆恇恈恉恊戀恌恍恎恏恐恑恒恓恔恕恖恗恘恙恚恛恜恝恞恟恠恡恢恣恤恥恦恧恨恩恪恫恬恭恮息恰恱恲懇恴恵惡恷慟懨愷惻惱惲恾恿悀悁悂悃悄悅悆悇悈悉悊悋悌悍悎悏悐悑悒悓悔悕悖悗悘悙悚悛悜悝悞悟悠悡悢患悤悥悅悧您悩悪愨懸慳悮憫悰悱悲悳悴悵悶悷悸悹悺悻悼悽悾悿惀惁惂惃惄情惆惇惈惉驚惋惌惍惎惏惐惑惒惓惔惕惖惗惘惙惚惛惜惝惞惟惠惡惢惣惤惥惦懼慘懲惪憊愜慚憚慣惰惱惲想惴惵惶惷惸惹惺惻惼惽惾惿愀愁愂愃愄愅愆愇愈愉愊愋愌愍愎意愐愑愒愓愔愕愖愗愘愙愚愛愜愝愞感慍愡愢愣憤愥憒愧愨愩愪愫愬愭愮愯愰愱愲愳愴愵愶愷愸愹愺愻愼愽愾愿慀慁慂慃慄慅慆慇慈慉慊態慌慍慎慏慐懾慒慓慔慕慖慗慘慙慚慛慜慝慞慟慠慡慢慣慤慥慦慧慨慩慪慫慬慭慮慯慰慱慲慳慴慵慶慷慸慹慺慻慼慽慾慿憀憁憂憃憄憅憆憇憈憉憊憋憌憍憎憏憐憑憒憓憔憕憖憗憘憙憚憛憜憝憞憟憠憡憢憣憤憥憦憧憨憩憪憫憬憭憮憯憰憱憲憳憴憵憶憷憸憹憺憻憼憽憾憿懀懁懂懃懄懅懆懇懈應懊懋懌懍懎懏懐懣懶懓懔懕懖懗懘懙懚懛懜懝懞懟懠懡懢懣懤懥懦懧懨懩懪懫懬懭懮懯懰懱懲懳懴懵懶懷懸懹懺懻懼懽懾懿戀戁戂戃戄戅戇戇戈戉戊戔戌戍戎戲成我戒戓戔戕或戧戰戙戚戛戜戝戞戟戠戡戢戣戤戥戦戧戨戩截戫戩戭戮戯戰戱戲戳戴戵戶戶戸戹戺戻戼戽戾房所扁扂扃扄扅扆扇扈扉扊手扌才扎扏扐撲扒打扔払扖扗托扙扚扛扜扝捍扟扠扡扢扣扤扥扦執扨擴捫掃揚扭扮扯擾扱扲扳扴扵扶扷扸批扺扻扼扽找承技抁抂抃抄抅抆抇抈抉把抋抌抍抎抏抐抑抒抓抔投抖抗折抙撫拋抜抝択摶摳掄搶抣護報抦抧抨抩抪披抬抭抮抯抰抱抲抳抴抵抶抷抸抹抺抻押抽抾抿拀拁拂拃拄擔拆拇拈拉拊拋拌拍拎拏拐拑拒拓拔拕拖拗拘拙拚招拜拝拞擬拠拡攏揀拤擁攔擰撥擇拪拫括拭拮拯拰拱拲拳拴拵拶拷拸拹拺拻拼拽拾拿挀持掛挃挄挅挆指挈按挊挋挌挍挎挏挐挑挒挓挔挕挖挗挘挙摯攣挜撾撻挾撓擋撟掙擠揮挦挧挨挩挪挫挬挭挮振挰挱挲挳挴挵挶挷挸挹挺挻挼挽挾挿捀捁捂捃捄捅捆捇捈捉捊捋捌捍捎捏捐捑捒捓捔捕捖捗捘捙捚捛捜捝撈損捠撿換搗捤捥捦捧捨捩捪捫捬捭據捯捰捱捲捳捴捵捶捷捸捹捺捻捼捽捾捿掀掁掂掃掄掅掆掇授掉掊掋掌掍掎掏掐掑排掓掔掕掖掗掘掙掚掛掜掝掞掟掠採探掣掤接掦控推掩措掫掬掭掮掯掰掱掲擄摑掵掶擲撣掹摻掻摜掽掾掿揀揁揂揃揄揅揆揇揈揉揊揋揌揍揎描提揑插揓揔揕揖揗揘揙揚換揜揝揞揟揠握揢揣揤揥揦揧揨揩揪揫揬揭揮揯揰揱揲揳援揵揶揷揸揹揺揻揼攬揾撳攙擱摟搃搄攪搆搇搈搉搊搋搌損搎搏搐搑搒搓搔搕搖搗搘搙搚搛搜搝搞搟搠搡搢搣搤搥搦搧搨搩搪搫搬搭搮搯搰搱搲搳搴搵搶搷搸搹攜搻搼搽搾搿摀摁摂摃攝攄擺搖擯摉攤摋摌摍摎摏摐摑摒摓摔摕摖摗摘摙摚摛摜摝摞摟摠摡摢摣摤摥摦摧摨摩摪摫摬摭摮摯摰摱摲摳摴摵摶摷摸摹摺摻摼摽摾摿撀撁撂撃攖撅撆撇撈撉撊撋撌撍撎撏撐撐撒撓撔撕撖撗撘撙撚撛撜撝撞撟撠撡撢撣撤撥撦撧撨撩撪撫撬播撮撯撰撱撲撳撴攆撶擷擼撹攛撻撼撽撾撿搟擁擂擃擄擅擆擇擈擉擊擋擌操擎擏擐擑擒擓擔擕擖擗擘擙據擛擜擝擻擟擠擡擢擣擤擥擦擧擨擩擪擫擬擭擮擯擰擱擲擳擴擵擶擷擸擹擺擻擼擽擾擿攀攁攂攃攄攅攆攇攈攉攊攋攌攍攎攏攐攑攢攓攔攕攖攗攘攙攚攛攜攝攞攟攠攡攢攣攤攥攦攧攨攩攪攫攬攭攮支攰攱攲攳攴攵收攷攸改攺攻攼攽放政敀敁敂敃敄故敆敇效敉敊敋敵敍敎敏敐救敒敓敔敕敖敗敘教敚斂敜敝敞敟敠敡敢散敤敥敦敧敨敩敪敫敬敭敮敯數敱敲敳整敵敶敷數敹敺敻敼敽敾敿斀斁斂斃斄斅斆文斈斉斊齋斌斍斎斏斐斑斒斕斔斕斖斗斘料斚斛斜斝斞斟斠斡斢斣斤斥斦斧斨斬斪斫斬斷斮斯新斱斲斳斴斵斶斷斸方斺斻於施斾斿旀旁旂旃旄旅旆旇旈旉旊旋旌旍旎族旐旑旒旓旔旕旖旗旘旙旚旛旜旝旞旟無旡既旣旤日旦舊旨早旪旫旬旭旮旯旰旱旲旳旴旵時曠旸旹旺旻旼旽旾旿昀昁昂昃昄昅昆昇昈昉昊昋昌昍明昏昐昑昒易昔昕昖昗昘曇昚昛昜昝昞星映昡昢昣昤春昦昧昨昩昪昫昬昭昮是昰昱昲昳昴昵昶昷昸昹昺昻晝昽顯昿晀晁時晃晄晅晆晇晈晉晊晉晌晍晎晏晐晑曬曉曄暈暉晗晘晙晚晛晜晝晞晟晠晡晢晣晤晥晦晧晨晩晪晫晬晭普景晰晱晲晳晴晵晶晷晸晹智晻晼晽晾晿暀暁暫暃暄暅暆暇暈暉暊暋暌暍暎暏暐暑暒暓暔暕暖暗暘暙暚暛暜暝暞暟暠暡暢暣暤暥暦曖暨暩暪暫暬暭暮暯暰暱暲暳暴暵暶暷暸暹暺暻暼暽暾暿曀曁曂曃曄曅曆曇曈曉曊曋曌曍曎曏曐曑曒曓曔曕曖曗曘曙曚曛曜曝曞曟曠曡曢曣曤曥曦曧曨曩曪曫曬曭曮曯曰曱曲曳更曵曶曷書曹曺曻曼曽曾替最朁朂會朄朅朆朇月有朊朋朌服朎朏朐朑朒朓朔朕朖朗朘朙朚望朜朝朞期朠朡朢朣朤朥朦朧木朩未末本札朮術朰朱朲朳樸朵朶朷朸朹機朻朼朽朾朿殺杁雜權杄杅桿杇杈杉杊杋杌杍李杏材村杒杓杔杕杖杗杘杙杚杛杜杝杞束杠條杢杣杤來杦杧楊榪杪杫杬杭杮杯杰東杲杳杴杵杶杷杸杹杺杻杼杽松板枀極枂枃構枅枆枇枈枉枊枋枌枍枎枏析枑枒枓枔枕枖林枘枙枚枛果枝樅枟枠枡樞棗枤櫪枦枧棖枩槍楓枬梟枮枯枰枱枲枳枴枵架枷枸枹枺枻枼枽枾枿柀柁柂柃柄柅柆柇柈柉柊柋柌柍柎柏某柑柒染柔柕柖柗柘柙柚柛柜柝柞柟檸柡柢柣柤查柦柧柨柩柪柫柬柭柮柯柰柱柲柳柴柵柶柷柸柹柺査柼檉柾柿梔栁栂栃栄柵栆標棧櫛櫳棟櫨栍櫟欄栐樹栒栓栔栕棲栗栘栙栚栛栜栝栞栟栠校栢栣栤栥栦栧栨栩株栫栬栭栮栯栰栱栲栳栴栵栶樣核根栺栻格栽欒栿桀桁桂桃桄桅框桇案桉桊桋桌桍桎桏桐桑桒桓桔桕桖桗桘桙桚桛桜桝桞桟椏橈楨檔榿橋樺檜槳樁桪桫桬桭桮桯桰桱桲桳桴桵桶桷桸桹桺桻桼桽桾桿梀梁梂梃梄梅梆梇梈梉梊梋梌梍梎梏梐梑梒梓梔梕梖梗梘梙梚梛梜條梞梟梠梡梢梣梤梥夢梧梨梩梪梫梬梭梮梯械梱梲梳梴梵梶梷梸梹梺梻梼梽梾梿檢棁欞棃棄棅棆棇棈棉棊棋棌棍棎棏棐棑棒棓棔棕棖棗棘棙棚棛棜棝棞棟棠棡棢棣棤棥棦棧棨棩棪棫棬棭森棯棰棱棲棳棴棵棶棷棸棹棺棻棼棽棾棿椀槨椂椃椄椅椆椇椈椉椊椋椌植椎椏椐椑椒椓椔椕椖椗椘椙椚椛検椝椞櫝槧椡椢椣欏椥椦椧椨椩椪椫椬橢椮椯椰椱椲椳椴椵椶椷椸椹椺椻椼椽椾椿楀楁楂楃楄楅楆楇楈楉楊楋楌楍楎楏楐楑楒楓楔楕楖楗楘楙楚楛楜楝楞楟楠楡楢楣楤楥楦楧楨楩楪楫楬業楮楯楰楱楲楳楴極楶楷楸楹楺楻樓楽楾楿榀榁概榃欖榅榆櫬櫚櫸榊榋榌榍榎榏榐榑榒榓榔榕榖榗榘榙榚榛榜榝榞榟榠榡榢榣榤榥榦榧榨榩榪榫榬榭榮榯榰榱榲榳榴榵榶榷榸榹榺榻榼榽榾榿槀槁槂槃槄槅槆槇槈槉槊構槌槍槎槏槐槑槒槓槔槕槖槗様槙槚檻槜槝槞檳櫧槡槢槣槤槥槦槧槨槩槪槫槬槭槮槯槰槱槲槳槴槵槶槷槸槹槺槻槼槽槾槿樀樁樂樃樄樅樆樇樈樉樊樋樌樍樎樏樐樑樒樓樔樕樖樗樘標樚樛樜樝樞樟樠模樢樣樤樥樦樧樨権橫樫樬樭樮檣樰櫻樲樳樴樵樶樷樸樹樺樻樼樽樾樿橀橁橂橃橄橅橆橇橈橉橊橋橌橍橎橏橐橑橒橓橔橕橖橗橘橙橚橛橜橝橞機橠橡橢橣橤橥橦橧橨橩橪橫橬橭橮橯橰櫥橲橳橴橵橶橷橸櫓橺橻櫞橽橾橿檀檁檂檃檄檅檆檇檈檉檊檋檌檍檎檏檐檑檒檓檔檕檖檗檘檙檚檛檜檝檞檟檠檡檢檣檤檥檦檧檨檁檪檫檬檭檮檯檰檱檲檳檴檵檶檷檸檹檺檻檼檽檾檿櫀櫁櫂櫃櫄櫅櫆櫇櫈櫉櫊櫋櫌櫍櫎櫏櫐櫑櫒櫓櫔櫕櫖櫗櫘櫙櫚櫛櫜櫝櫞櫟櫠櫡櫢櫣櫤櫥櫦櫧櫨櫩櫪櫫櫬櫭櫮櫯櫰櫱櫲櫳櫴櫵櫶櫷櫸櫹櫺櫻櫼櫽櫾櫿欀欁欂欃欄欅欆欇欈欉權欋欌欍欎欏欐欑欒欓欔欕欖欗欘欙欚欛欜欝欞欟欠次歡欣歟欥欦歐欨欩欪欫欬欭欮欯欰欱欲欳欴欵欶欷欸欹欺欻欼欽款欿歀歁歂歃歄歅歆歇歈歉歊歋歌歍歎歏歐歑歒歓歔歕歖歗歘歙歚歛歜歝歞歟歠歡止正此步武歧歨歩歪歫歬歭歮歯歰歱歲歳歴歵歶歷歸歹歺死殲歽歾歿殀歿殂殃殄殅殆殤殈殉殊殘殌殍殎殏殐殑殞殮殔殕殖殗殘殙殫殛殜殝殞殟殠殯殢殣殤殥殦殧殨殩殪殫殬殭殮殯殰殱殲殳毆段殶殷殸殹殺殻殼殽殾殿毀毀轂毃毄毅毆毇毈毉毊毋毌母毎每毐毑毒毓比畢毖毗毘斃毚毛毜毝毞毟毠氈毢毣毤毥毦毧毨毩毪毫毬毭毮毯毰毱毲毳毴毿毶毷毸毹毺毻毼毽毾毿氀氁氂氃氄氅氆氌氈氉氊氋氌氍氎氏氐民氒氓氣氕氖気氘氙氚氛氜氝氞氟氠氡氫氣氤氥氦氧氨氬氪氫氬氭氮氯氰氱氳氳水氵氶氷永氹氺氻氼氽氾氿汀汁求汃汄汅汆匯汈漢汊汋汌汍汎汏汐汑汒汓汔汕汖汗汘汙汚汛汜汝汞江池污汢汣湯汥汦汧汨汩汪汫汬汭汮汯汰汱汲汳汴汵汶汷汸洶決汻汼汽汾汿沀沁沂沃沄沅沆沇沈沉沊沋沌沍沎沏沐沑沒沓沔沕沖沗沘沙沚沛沜沝沞溝沠沒沢灃漚瀝淪滄沨溈滬沫沬沭沮沯沰沱沲河沴沵沶沷沸油沺治沼沽沾沿泀況泂泃泄泅泆泇泈泉泊泋泌泍泎泏泐泑泒泓泔法泖泗泘泙泚泛泜泝濘泟泠泡波泣泤泥泦泧注泩淚泫泬泭泮泯泰泱泲泳泴泵澩瀧瀘泹濼瀉潑澤涇泿洀潔洂洃洄洅洆洇洈洉洊洋洌洍洎洏洐洑灑洓洔洕洖洗洘洙洚洛洜洝洞洟洠洡洢洣洤津洦洧洨洩洪洫洬洭洮洯洰洱洲洳洴洵洶洷洸洹洺活洼洽派洿浀流浂浹浄淺漿澆湞浉濁測浌澮濟瀏浐渾滸濃潯浕浖浗浘浙浚浛浜浝浞浟浠浡浢浣浤浥浦浧浨浩浪浫浬浭浮浯浰浱浲浳浴浵浶海浸浹浺浻浼浽浾浿涀涁涂涃涄涅涆涇消涉涊涋涌涍涎涏涐涑涒涓涔涕涖涗涘涙涚濤涜澇淶漣潿渦涢渙滌涥潤澗漲澀涪涫涬涭涮涯涰涱液涳涴涵涶涷涸涹涺涻涼涽涾涿淀淁淂淃淄淅淆淇淈淉淊淋淌淍淎淏淐淑淒淓淔淕淖淗淘淙淚淛淜淝淞淟淠淡淢淣淤淥淦淧淨淩淪淫淬淭淮淯淰深淲淳淴淵淶混淸淹淺添淼淽淾淿渀渁渂渃渄清渆渇済渉淵渋淥漬瀆渏漸澠渒渓漁渕瀋滲渘渙渚減渜渝渞渟渠渡渢渣渤渥渦渧渨溫渪渫測渭渮港渰渱渲渳渴渵渶渷游渹渺渻渼渽渾渿湀湁湂湃湄湅湆湇湈湉湊湋湌湍湎湏湐湑湒湓湔湕湖湗湘湙湚湛湜湝湞湟湠湡湢湣湤湥湦湧湨湩湪湫湬湭湮湯湰湱湲湳湴湵湶湷湸湹湺湻湼湽灣濕満溁溂潰溄濺溆溇溈溉溊溋溌溍溎溏源溑溒溓溔溕準溗溘溙溚溛溜溝溞溟溠溡溢溣溤溥溦溧溨溩溪溫溬溭溮溯溰溱溲溳溴溵溶溷溸溹溺溻溼溽溾溿滀滁滂滃滄滅滆滇滈滉滊滋滌滍滎滏滐滑滒滓滔滕滖潷滘滙滾滛滜滝滯滟灄滿瀅滣濾濫灤滧濱灘滪滫滬滭滮滯滰滱滲滳滴滵滶滷滸滹滺滻滼滽滾滿漀漁漂漃漄漅漆漇漈漉漊漋漌漍漎漏漐漑漒漓演漕漖漗漘漙漚漛漜漝漞漟漠漡漢漣漤漥漦漧漨漩漪漫漬漭漮漯漰漱漲漳漴漵漶漷漸漹漺漻漼漽漾漿潀潁潂潃潄潅瀠瀟潈潉潊瀲潌濰潎潏潐潑潒潓潔潕潖潗潘潙潚潛潛潝潞潟潠潡潢潣潤潥潦潧潨潩潪潫潬潭潮潯潰潱潲潳潴潵潶潷潸潹潺潻潼潽潾潿澀澁澂澃澄澅澆澇澈澉澊澋澌澍澎澏澐澑澒澓澔澕澖澗澘澙澚澛瀾澝澞澟澠澡澢澣澤澥澦澧澨澩澪澫澬澭澮澯澰澱澲澳澴澵澶澷澸澹澺澻澼澽澾澿激濁濂濃濄濅濆濇濈濉濊濋濌濍濎濏濐瀨瀕濓濔濕濖濗濘濙濚濛濜濝濞濟濠濡濢濣濤濥濦濧濨濩濪濫濬濭濮濯濰濱濲濳濴濵濶濷濸濹濺濻濼濽濾濿瀀瀁瀂瀃瀄瀅瀆瀇瀈瀉瀊瀋瀌瀍瀎瀏瀐瀑瀒瀓瀔瀕瀖瀗瀘瀙瀚瀛瀜瀝瀞瀟瀠瀡瀢瀣瀤瀥瀦瀧瀨瀩瀪瀫瀬瀭瀮瀯瀰瀱瀲瀳瀴瀵瀶瀷瀸瀹瀺瀻瀼瀽瀾瀿灀灁灂灃灄灅灆灇灈灉灊灋灌灍灎灝灐灑灒灓灔灕灖灗灘灙灚灛灜灝灞灟灠灡灢灣灤灥灦灧灨灩灪火灬滅灮燈灰灱灲灳灴靈灶灷灸灹灺灻灼災災燦煬炁炂炃炄炅炆炇炈爐炊炋炌炍炎炏炐炑炒炓炔炕燉炗炘炙炚炛煒熗炞炟炠炡炢炣炤炥炦炧炨炩炪炫炬炭炮炯炰炱炲炳炴炵炶炷炸點為炻煉熾炾炿烀爍爛烴烄烅烆烇烈烉烊烋烌烍烎烏烐烑烒烓烔烕烖烗烘烙烚燭烜烝烞煙烠烡烢烣烤烥煩燒燁燴烪燙燼熱烮烯烰烱烲烳烴烵烶烷烸烹烺烻烼烽烾烿焀焁焂焃焄焅焆焇焈焉焊焋焌焍焎焏焐焑焒焓焔煥燜焗燾焙焚焛焜焝焞焟焠無焢焣焤焥焦焧焨焩焪焫焬焭焮焯焰焱焲焳焴焵然焷焸焹焺焻焼焽焾焿煀煁煂煃煄煅煆煇煈煉煊煋煌煍煎煏煐煑煒煓煔煕煖煗煘煙煚煛煜煝煞煟煠煡煢煣煤煥煦照煨煩煪煫煬煭煮煯煰煱煲煳煴煵煶煷煸煹煺煻煼煽煾煿熀熁熂熃熄熅熆熇熈熉熊熋熌熍熎熏熐熑熒熓熔熕熖熗熘熙熚熛熜熝熞熟熠熡熢熣熤熥熦熧熨熩熪熫熬熭熮熯熰熱熲熳熴熵熶熷熸熹熺熻熼熽熾熿燀燁燂燃燄燅燆燇燈燉燊燋燌燍燎燏燐燑燒燓燔燕燖燗燘燙燚燛燜燝燞營燠燡燢燣燤燥燦燧燨燩燪燫燬燭燮燯燰燱燲燳燴燵燶燷燸燹燺燻燼燽燾燿爀爁爂爃爄爅爆爇爈爉爊爋爌爍爎爏爐爑爒爓爔爕爖爗爘爙爚爛爜爝爞爟爠爡爢爣爤爥爦爧爨爩爪爫爬爭爮爯爰愛爲爳爴爵父爺爸爹爺爻爼爽爾爿牀牁牂牃牄牅牆片版牉牊牋牌牘牎牏牐牑牒牓牔牕牖牗牘牙牚牛牜牝牞牟牠牡牢牣牤牥牦牧牨物牪牫牬牭牮牯牰牱牲牳牴牽牶牷牸特犧牻牼牽牾牿犀犁犂犃犄犅犆犇犈犉犢犋犌犍犎犏犐犑犒犓犔犕犖犗犘犙犚犛犜犝犞犟犠犡犢犣犤犥犦犧犨犩犪犫犬犭犮犯犰犱犲犳犴犵狀獷犸猶犺犻犼犽犾犿狀狁狂狃狄狅狆狇狽狉狊狋狌狍狎狏狐狑狒狓狔狕狖狗狘狙狚狛狜狝獰狟狠狡狢狣狤狥狦狧狨狩狪狫獨狹獅獪猙獄猻狳狴狵狶狷貍狹狺狻狼狽狾狿猀猁猂獫猄猅猆猇猈猉猊猋猌猍獵猏猐猑猒猓猔獼猖猗猘猙猚猛猜猝猞猟猠玀猢猣猤猥猦猧猨猩豬貓猬猭獻猯猰猱猲猳猴猵猶猷猸猹猺猻猼猽猾猿獀獁獂獃獄獅獆獇獈獉獊獋獌獍獎獏獐獑獒獓獔獕獖獗獘獙獚獛獜獝獞獟獠獡獢獣獤獥獦獧獨獩獪獫獬獺獮獯獰獱獲獳獴獵獶獷獸獹獺獻獼獽獾獿玀玁玂玃玄玅玆率玈玉玊王玌玍玎玏玐璣玒玓玔玕玖玗玘玙玚瑪玜玝玞玟玠玡玢玣玤玥玦玧玨玩玪玫玬玭瑋環現玱玲玳玴玵玶玷玸玹璽玻玼玽玾玿珀珁珂珃珄珅珆珇珈珉珊珋珌珍珎玨琺瓏珒珓珔珕珖珗珘珙珚珛珜珝珞珟珠珡珢珣珤珥珦珧珨珩珪珫珬班珮珯珰珱琿珳珴珵珶珷珸珹珺珻珼珽現珿琀琁琂球琄瑯理琇琈琉琊琋琌琍琎璉瑣琑琒琓琔琕琖琗琘琙琚琛琜琝琞琟琠琡琢琣琤琥琦琧琨琩琪琫琬琭琮琯琰琱琲琳琴琵琶琷琸琹琺琻瓊琽琾琿瑀瑁瑂瑃瑄瑅瑆瑇瑈瑉瑊瑋瑌瑍瑎瑏瑐瑑瑒瑓瑔瑕瑖瑗瑘瑙瑚瑛瑜瑝瑞瑟瑠瑡瑢瑣瑤瑥瑦瑧瑨瑩瑪瑫瑬瑭瑮瑯瑰瑱瑲瑳瑴瑵瑤璦瑸瑹瑺瑻瑼瑽瑾瑿璀璁璂璃璄璅璆璇璈璉璊璋璌璍瓔璏璐璑璒璓璔璕璖璗璘璙璚璛璜璝璞璟璠璡璢璣璤璥璦璧璨璩璪璫璬璭璮璯環璱璲璳璴璵璶璷璸璹璺璻璼璽璾璿瓀瓁瓂瓃瓄瓅瓆瓇瓈瓉瓊瓋瓌瓍瓎瓏瓐瓑瓚瓓瓔瓕瓖瓗瓘瓙瓚瓛瓜瓝瓞瓟瓠瓡瓢瓣瓤瓥瓦瓧瓨瓩瓪瓫瓬瓭甕甌瓰瓱瓲瓳瓴瓵瓶瓷瓸瓹瓺瓻瓼瓽瓾瓿甀甁甂甃甄甅甆甇甈甉甊甋甌甍甎甏甐甑甒甓甔甕甖甗甘甙甚甛甜甝甞生甠甡產産甤甥甦甧用甩甪甫甬甭甮甯田由甲申甴電甶男甸甹町畫甼甽甾甿畀畁畂畃畄暢畆畇畈畉畊畋界畍畎畏畐畑畒畓畔畕畖畗畘留畚畛畜畝畞畟畠畡畢畣畤略畦畧畨畩番畫畬畭畮畯異畱畬畳疇畵當畷畸畹畺畻畼畽畾畿疀疁疂疃疄疅疆疇疈疉疊疋疌疍疎疏疐疑疒疓疔疕癤療疘疙疚疛疜疝疞瘧癘瘍疢疣疤疥疦疧疨疩疪疫疬疭瘡瘋疰皰疲疳疴疵疶疷疸疹疺疻疼疽疾疿痀痁痂痃痄病痆癥癰痙痊痋痌痍痎痏痐痑癢痓痔痕痖痗痘痙痚痛痜痝痞痟痠痡痢痣痤痥痦痧癆痩瘓癇痬痭痮痯痰痱痲痳癡痵痶痷痸痹痺痻痼痽痾痿瘀瘁瘂瘃瘄癉瘆瘇瘈瘉瘊瘋瘌瘍瘎瘏瘐瘑瘒瘓瘔瘕瘖瘞瘺瘙瘚瘛瘜瘝瘞瘟瘠瘡瘢瘣瘤瘥瘦瘧瘨瘩癟癱瘬瘭瘮瘯瘰瘱瘲瘳瘴瘵瘶瘷瘸瘹瘺瘻瘼瘽癮癭癀癁療癃癄癅癆癇癈癉癊癋癌癍癎癏癐癑癒癓癔癕癖癗癘癙癚癛癜癝癩癟癠癡癢癬癤癥癦癧癨癩癪癲癬癭癮癯癰癱癲癳癴癵癶癷癸癹発登發白百癿皀皁皂皃的皅皆皇皈皉皊皋皌皍皎皏皐皚皒皓皔皕皖皗皘皙皚皛皜皝皞皟皠皡皢皣皤皥皦皧皨皩皪皫皬皭皮皯皰皺皸皳皴皵皶皷皸皹皺皻皼皽皾皿盀盁盂盃盄盅盆盇盈盉益盋盌盍盎盞鹽監盒盓盔盕蓋盜盤盙盚盛盜盝盞盟盠盡盢監盤盥盦盧盨盩盪盫盬盭目盯盰盱盲盳直盵盶盷相盹盺盻盼盽盾盿眀省眂眃眄眅眆眇眈眉眊看県眍眎眏眐眑眒眓眔眕眖眗眘眙眚眛眜眝眞真眠眡眢眣眤眥眥眧眨眩眪眫眬眭眮瞇眰眱眲眳眴眵眶眷眸眹眺眻眼眽眾眿著睜睂脧睄睅睆睇睈睉睊睋睌睍睎睏睞瞼睒睓睔睕睖睗睘睙睚睛睜睝睞睟睠睡睢督睤睥睦睧睨睩睪睫睬睭睮睯睰睱睲睳睴睵睶睷睸睹睺睻睼睽睪睿瞀瞁瞂瞃瞄瞅瞆瞇瞈瞉瞊瞋瞌瞍瞎瞏瞐瞑瞞瞓瞔瞕瞖瞗瞘瞙瞚瞛瞜瞝瞞瞟瞠瞡瞢瞣瞤瞥瞦瞧瞨矚瞪瞫瞬瞭瞮瞯瞰瞱瞲瞳瞴瞵瞶瞷瞸瞹瞺瞻瞼瞽瞾瞿矀矁矂矃矄矅矆矇矈矉矊矋矌矍矎矏矐矑矒矓矔矕矖矗矘矙矚矛矜矝矞矟矠矡矢矣矤知矦矧矨矩矪矯矬短矮矯矰矱矲石矴矵磯矷矸矹矺矻矼矽礬礦碭碼砂砃砄砅砆砇砈砉砊砋砌砍砎砏砐砑砒砓研砕磚硨砘砙硯砛砜砝砞砟砠砡砢砣砤砥砦砧砨砩砪砫砬砭砮砯砰砱砲砳破砵砶砷砸砹礪礱砼砽礫砿礎硁硂硃硄硅硆硇硈硉硊硋硌硍硎硏硐硑硒硓硔碩硤磽硘硙硚硛硜硝硞硟硠硡硢硣硤硥硦硧硨硩硪硫硬硭確硯硰硱硲硳硴硵硶鹼硸硹硺硻硼硽硾硿碀碁碂碃碄碅碆碇碈碉碊碋碌礙碎碏碐碑碒碓碔碕碖碗碘碙碚磧磣碝碞碟碠碡碢碣碤碥碦碧碨碩碪碫碬碭碮碯碰堿碲碳碴碵碶碷碸碹確碻碼碽碾碿磀磁磂磃磄磅磆磇磈磉磊磋磌磍磎磏磐磑磒磓磔磕磖磗磘磙磚磛磜磝磞磟磠磡磢磣磤磥磦磧磨磩磪磫磬磭磮磯磰磱磲磳磴磵磶磷磸磹磺磻磼磽磾磿礀礁礂礃礄礅礆礇礈礉礊礋礌礍礎礏礐礑礒礓礔礕礖礗礘礙礚礛礜礝礞礟礠礡礢礣礤礥礦礧礨礩礪礫礬礭礮礯礰礱礲礳礴礵礶礷礸礹示礻禮礽社礿祀祁祂祃祄祅祆祇祈祉祊祋祌祍祎祏祐祑祒祓祔祕祖祗祘祙祚祛祜祝神祟祠祡禰祣祤祥祦祧票祩祪祫祬祭祮禎祰祱祲祳祴祵祶禱禍祹祺祻祼祽祾祿稟禁禂禃祿禪禆禇禈禉禊禋禌禍禎福禐禑禒禓禔禕禖禗禘禙禚禛禜禝禞禟禠禡禢禣禤禥禦禧禨禩禪禫禬禭禮禯禰禱禲禳禴禵禶禷禸禹禺離禼禽禾禿秀私秂禿秄秅稈秇秈秉秊秋秌種秎秏秐科秒秓秔秕秖秗秘秙秚秛秜秝秞租秠秡秢秣秤秥秦秧秨秩秪秫秬秭秮積稱秱秲秳秴秵秶秷秸秹秺移秼穢秾秿稀稁稂稃稄稅稆稇稈稉稊程稌稍稅稏稐稑稒稓稔稕稖稗稘稙稚稛稜稝稞稟稠稡稢穌稤稥稦稧稨稩稪稫稬稭種稯稰稱稲穩稴稵稶稷稸稹稺稻稼稽稾稿穀穁穂穃穄穅穆穇穈穉穊穋穌積穎穏穐穡穒穓穔穕穖穗穘穙穚穛穜穝穞穟穠穡穢穣穤穥穦穧穨穩穪穫穬穭穮穯穰穱穲穳穴穵究窮穸穹空穻穼穽穾穿窀突窂竊窄窅窆窇窈窉窊窋窌竅窎窏窐窯窒窓窔窕窖窗窘窙窚窛竄窩窞窟窠窡窢窣窤窺竇窧窨窩窪窫窬窶窮窯窰窱窲窳窴窵窶窷窸窹窺窻窼窽窾窿竀竁竂竃竄竅竆竇竈竉竊立竌竍竎竏竐竑竒竓竔竕豎竗竘站竚竛竜竝競竟章竡竢竣竤童竦竧竨竩竪竫竬竭竮端竰竱竲竳竴竵競竷竸竹竺竻竼竽竾竿笀笁笂篤笄笅笆笇笈笉笊筍笌笍笎笏笐笑笒笓筆筧笖笗笘笙笚笛笜笝笞笟笠笡笢笣笤笥符笧笨笩笪笫第笭笮笯笰笱笲笳笴笵笶笷笸笹箋笻籠笽籩笿筀筁筂筃筄筅筆筇筈等筊筋筌筍筎筏筐筑筒筓答筕策筗筘筙篳篩筜箏筞筟筠筡筢筣筤筥筦筧筨筩筪筫筬筭筮筯筰筱筲筳筴筵筶筷筸籌筺筻筼筽簽筿簡箁箂箃箄箅箆箇箈箉箊箋箌箍箎箏箐箑箒箓箔箕箖算箘箙箚箛箜箝箞箟箠管箢箣箤箥簀篋籜籮簞簫箬箭箮箯箰箱箲箳箴箵箶箷箸箹箺箻箼箽箾箿節篁篂篃範篅篆篇篈築篊篋篌篍篎篏篐簣篒簍篔篕篖篗篘篙篚篛篜篝篞篟篠篡篢篣篤篥篦篧篨篩篪篫篬篭籃篯篰籬篲篳篴篵篶篷篸篹篺篻篼篽篾篿簀簁簂簃簄簅簆簇簈簉簊簋簌簍簎簏簐簑簒簓簔簕籪簗簘簙簚簛簜簝簞簟簠簡簢簣簤簥簦簧簨簩簪簫簬簭簮簯簰簱簲簳簴簵簶簷簸簹簺簻簼簽簾簿籀籟籂籃籄籅籆籇籈籉籊籋籌籍籎籏籐籑籒籓籔籕籖籗籘籙籚籛籜籝籞籟籠籡籢籣籤籥籦籧籨籩籪籫籬籭籮籯籰籱籲米糴籵籶籷籸籹籺類秈籽籾籿粀粁粂粃粄粅粆粇粈粉粊粋粌粍粎粏粐粑粒粓粔粕粖粗粘粙粚粛糶糲粞粟粠粡粢粣粵粥粦粧粨粩糞粫粬粭糧粯粰粱粲粳粴粵粶粷粸粹粺粻粼粽精粿糀糝糂糃糄糅糆糇糈糉糊糋糌糍糎糏糐糑糒糓糔糕糖糗糘糙糚糛糜糝糞糟糠糡糢糣糤糥糦糧糨糩糪糫糬糭糮糯糰糱糲糳糴糵糶糷糸糹糺系糼糽糾糿紀紁紂紃約紅紆紇紈紉紊紋紌納紎紏紐紑紒紓純紕紖紗紘紙級紛紜紝紞紟素紡索紣紤紥紦緊紨紩紪紫紬紭紮累細紱紲紳紴紵紶紷紸紹紺紻紼紽紾紿絀絁終絃組絅絆絇絈絉絊絋経絍絎絏結絑絒絓絔絕絖絗絘絙絚絛絜絝絞絟絠絡絢絣絤絥給絧絨絩絪絫絬絭絮絯絰統絲絳絴絵絶縶絸絹絺絻絼絽絾絿綀綁綂綃綄綅綆綇綈綉綊綋綌綍綎綏綐綑綒經綔綕綖綗綘継続綛綜綝綞綟綠綡綢綣綤綥綦綧綨綩綪綫綬維綮綯綰綱網綳綴綵綶綷綸綹綺綻綼綽綾綿緀緁緂緃緄緅緆緇緈緉緊緋緌緍緎総緐緑緒緓緔緕緖緗緘緙線緛緜緝緞緟締緡緢緣緤緥緦緧編緩緪緫緬緭緮緯緰緱緲緳練緵緶緷緸緹緺緻緼緽緾緿縀縁縂縃縄縅縆縇縈縉縊縋縌縍縎縏縐縑縒縓縔縕縖縗縘縙縚縛縜縝縞縟縠縡縢縣縤縥縦縧縨縩縪縫縬縭縮縯縰縱縲縳縴縵縶縷縸縹縺縻縼總績縿繀繁繂繃繄繅繆繇繈繉繊繋繌繍繎繏繐繑繒繓織繕繖繗繘繙繚繛繜繝繞繟繠繡繢繣繤繥繦繧繨繩繪繫繬繭繮繯繰繱繲繳繴繵繶繷繸繹繺繻繼繽繾繿纀纁纂纃纄纅纆纇纈纉纊纋續纍纎纏纐纑纒纓纔纕纖纗纘纙纚纛纜纝纞纟糾紆紅紂纖紇約級紈纊紀紉緯紜纮純紕紗綱納纴縱綸紛紙紋紡纻纼紐紓線紺紲紱練組紳細織終縐絆紼絀紹繹經紿綁絨結绔繞绖絎繪給絢絳絡絕絞統綆綃絹繡绤綏絳繼綈績緒綾绬續綺緋綽绱緄繩維綿綬繃綢绹綹綣綜綻綰綠綴緇緙緗緘緬纜緹緲緝缊繢緦綞緞緶缐緱縋緩締縷編緡緣縉縛縟縝縫缞縞纏縭縊縑繽縹縵縲纓縮繆繅纈繚繕繒韁繾繰繯繳纘缶缷缸缹缺缻缼缽缾缿罀罁罌罃罄罅罆罇罈罉罊罋罌罍罎罏罐網罒罓罔罕罖羅罘罙罰罛罜罝罞罟罠罡罷罣罤罥罦罧罨罩罪罫罬罭置罯罰罱署罳羆罵罶罷罸罹罺罻罼罽罾罿羀羈羂羃羄羅羆羇羈羉羊羋羌羍美羏羐羑羒羓羔羕羖羗羘羙羚羛羜羝羞羥羠羨羢羣群羥羦羧羨義羪羫羬羭羮羯羰羱羲羳羴羵羶羷羸羹羺羻羼羽羾羿翀翁翂翃翄翅翆翇翈翉翊翋翌翍翎翏翐翑習翓翔翕翖翗翹翙翚翛翜翝翞翟翠翡翢翣翤翥翦翧翨翩翪翫翬翭翮翯翰翱翲翳翴翵翶翷翸翹翺翻翼翽翾翿耀老耂考耄者耆耇耈耉耊耋而耍耎耏耐耑耒耓耔耕耖耗耘耙耚耛耜耝耞耟耠耡耢耣耤耥耦耬耨耩耪耫耬耭耮耯耰耱耲耳耴耵耶耷聳耹耺恥耼耽耾耿聀聁聶聃聄聅聆聇聈聉聊聾職聹聎聏聐聑聒聓聯聕聖聗聘聙聚聛聜聝聞聟聠聡聢聣聤聥聦聧聨聵聰聫聬聭聮聯聰聱聲聳聴聵聶職聸聹聺聻聼聽聾聿肀肁肂肅肄肅肆肇肈肉肊肋肌肍肎肏肐肑肒肓肔肕肖肗肘肙肚肛肜肝肞肟腸股肢肣膚肥肦肧肨肩肪肫肬肭骯肯肰肱育肳肴肵肶肷肸肹肺肻肼肽腎腫脹脅胂胃胄胅膽胇胈胉胊胋背胍胎胏胐胑胒胓胔胕胖胗胘胙胚胛勝胝胞胟胠胡胢胣胤胥胦朧胨胩臚脛胬胭胮胯胰胱胲胳胴胵膠胷胸胹胺胻胼能胾胿脀脁脂脃脄脅脆脇脈脈脊脋脌膾脎臟臍腦脒膿臠脕脖脗脘脙腳脛脜脝脞脟脠脡脢脣脤脥脦脧脨脩脪脫脬脭脮脯脰脫脲脳脴脵腡脷臉脹脺脻脼脽脾脿腀腁腂腃腄腅腆腇腈腉臘腋腌腍腎腏腐腑腒腓腔腕腖腗腘腙腚腛腜腝腞腟腠腡腢腣腤腥腦腧腨腩腪腫腬腭腮腯腰腱腲腳腴腵腶腷腸腹腺膩靦膃騰腿膀膁膂膃膄膅膆膇膈膉膊膋膌膍膎膏膐臏膒膓膔膕膖膗膘膙膚膛膜膝膞膟膠膡膢膣膤膥膦膧膨膩膪膫膬膭膮膯膰膱膲膳膴膵膶膷膸膹膺膻膼膽膾膿臀臁臂臃臄臅臆臇臈臉臊臋臌臍臎臏臐臑臒臓臔臕臖臗臘臙臚臛臜臝臞臟臠臡臢臣臤臥臦臧臨臩自臫臬臭臮臯臰臱臲至致臵臶臷臸臹臺臻臼臽臾臿舀舁舂舃舄舅輿與興舉舊舋舌舍舎舏舐舑舒舓舔舕舖舗舘舙舚舛舜舝舞舟舠舡舢艤舤舥舦舧舨舩航舫般舭舮舯艦艙舲舳舴舵舶舷舸船舺艫舼舽舾舿艀艁艂艃艄艅艆艇艈艉艊艋艌艍艎艏艐艑艒艓艔艕艖艗艘艙艚艛艜艝艞艟艠艡艢艣艤艥艦艧艨艩艪艫艬艭艮良艱艱色艷艴艵艶艷艸艸藝艻艼艽艾艿芀芁節芃芄芅芆芇羋芉芊芋芌芍芎芏芐芑芒芓芔芕芖薌芘芙芚芛蕪芝芞芟芠芡芢芣芤芥蘆芧芨芩芪芫芬芭芮芯芰花芲芳芴芵芶芷蕓芹芺芻芼芽芾芿苀蓯苂苃芐苅苆葦藶苉苊莧萇蒼苧蘇苐苑苒苓苔苕苖苗苘苙苚苛苜苝苞茍苠苡苢苣苤若苦苧苨苩苪苫苬苭苮苯苰英苲苳苴苵苶苷苸蘋苺苻苼苽苾苿茀茁茂范茄茅茆茇茈茉茊茋茌茍莖蘢茐蔦茒茓塋煢茖茗茘茙茚茛茜茝茞茟茠茡茢茣茤茥茦繭茨茩茪茫茬茭茮茯茰茱茲茳茴茵茶茷茸茹茺茻茼茽茾茿荀荁荂荃荄荅荊荇荈草荊荋荌荍荎荏薦荑荒荓荔荕荖荗荘荙莢蕘蓽荝蕎薈薺蕩荢榮葷滎犖熒蕁藎蓀蔭荬葒荮藥荰荱荲荳荴荵荶荷荸荹荺荻荼荽荾荿莀莁莂莃莄蒞莆莇莈莉莊莋莌莍莎莏莐莑莒莓莔莕莖莗莘莙莚莛莜莝莞莟莠莡莢莣莤莥莦莧莨莩莪莫莬莭莮莯莰萊蓮蒔萵莵薟獲蕕瑩鶯莻莼莽莾莿菀菁菂菃菄菅菆菇菈菉菊菋菌菍菎菏菐菑菒菓菔菕菖菗菘菙菚菛菜菝菞菟菠菡菢菣菤菥菦菧菨菩菪菫菬菭菮華菰菱菲菳菴菵菶菷菸菹菺菻菼菽菾菿萀萁萂萃萄萅萆萇萈萉萊萋萌萍萎萏萐萑萒萓萔萕萖萗萘萙萚萛萜蘿萞萟萠萡萢萣螢營縈蕭薩萩萪萫萬萭萮萯萰萱萲萳萴萵萶萷萸萹萺萻萼落萾萿葀葁葂葃葄葅葆葇葈葉葊葋葌葍葎葏葐葑葒葓葔葕葖著葘葙葚葛葜葝葞葟葠葡葢董葤葥葦葧葨葩葪葫葬葭葮葯葰蔥葲葳葴葵葶葷葸葹葺葻葼葽葾葿蒀蒁蒂蒃蒄蒅蒆蕆蒈蕢蒊蔣蔞蒍蒎蒏蒐蒑蒒蒓蒔蒕蒖蒗蒘蒙蒚蒛蒜蒝蒞蒟蒠蒡蒢蒣蒤蒥蒦蒧蒨蒩蒪蒫蒬蒭蒮蒯蒰蒱蒲蒳蒴蒵蒶蒷蒸蒹蒺蒻蒼蒽蒾蒿蓀蓁蓂蓃蓄蓅蓆蓇蓈蓉蓊蓋蓌蓍蓎蓏蓐蓑蓒蓓蓔蓕蓖蓗蓘蓙蓚蓛蓜藍蓞薊蘺蓡蓢蕷蓤鎣驀蓧蓨蓩蓪蓫蓬蓭蓮蓯蓰蓱蓲蓳蓴蓵蓶蓷蓸蓹蓺蓻蓼蓽蓾蓿蔀蔁蔂蔃蔄蔅蔆蔇蔈蔉蔊蔋蔌蔍蔎蔏蔐蔑蔒蔓蔔蔕蔖蔗蔘蔙蔚蔛蔜蔝蔞蔟蔠蔡蔢蔣蔤蔥蔦蔧蔨蔩蔪蔫蔬蔭蔮蔯蔰蔱蔲蔳蔴蔵蔶薔蔸蘞藺蔻藹蔽蔾蔿蕀蕁蕂蕃蕄蕅蕆蕇蕈蕉蕊蕋蕌蕍蕎蕏蕐蕑蕒蕓蕔蕕蕖蕗蕘蕙蕚蕛蕜蕝蕞蕟蕠蕡蕢蕣蕤蕥蕦蕧蕨蕩蕪蕫蕬蕭蕮蕯蕰蕱蘄蕳蘊蕵蕶蕷蕸蕹蕺蕻蕼蕽蕾蕿薀薁薂薃薄薅薆薇薈薉薊薋薌薍薎薏薐薑薒薓薔薕薖薗薘薙薚薛薜薝薞薟薠薡薢薣薤薥薦薧薨薩薪薫薬薭藪薯薰薱薲薳薴薵薶薷薸薹薺薻薼薽薾薿藀藁藂藃藄藅藆藇藈藉藊藋藌藍藎藏藐藑藒蘚藔藕藖藗藘藙藚藛藜藝藞藟藠藡藢藣藤藥藦藧藨藩藪藫藬藭藮藯藰藱藲藳藴藵藶藷藸藹藺藻藼藽藾藿蘀蘁蘂蘃蘄蘅蘆蘇蘈蘉蘊蘋蘌蘍蘎蘏蘐蘑蘒蘓蘔蘕蘗蘗蘘蘙蘚蘛蘜蘝蘞蘟蘠蘡蘢蘣蘤蘥蘦蘧蘨蘩蘪蘫蘬蘭蘮蘯蘰蘱蘲蘳蘴蘵蘶蘷蘸蘹蘺蘻蘼蘽蘾蘿虀虁虂虃虄虅虆虇虈虉虊虋虌虍虎虜虐慮虒虓虔處虖虗虘虙虛虛虜虝虞號虠虡虢虣虤虥虦虧虨虩虪蟲虬虭蟣虯虰虱虲虳虴虵虶虷虸虹虺虻虼雖蝦蠆蝕蟻螞蚃蚄蚅蚆蚇蚈蚉蚊蚋蚌蚍蚎蚏蚐蚑蚒蚓蚔蠶蚖蚗蚘蚙蚚蚛蚜蠔蚞蚟蚠蚡蚢蚣蚤蚥蚦蚧蚨蚩蚪蚫蜆蚭蚮蚯蚰蚱蚲蚳蚴蚵蚶蚷蚸蚹蚺蚻蚼蚽蚾蚿蛀蛁蛂蛃蛄蛅蛆蛇蛈蛉蠱蛋蛌蛍蠣蟶蛐蛑蛒蛓蛔蛕蛖蛗蛘蛙蛚蛛蛜蛝蛞蛟蛠蛡蛢蛣蛤蛥蛦蛧蛨蛩蛪蛫蛬蛭蠻蛯蟄蛺蟯螄蠐蛵蛶蛷蛸蛹蛺蛻蛼蛽蛾蛿蜀蜁蜂蜃蜄蜅蜆蜇蜈蜉蜊蜋蜌蜍蜎蜏蜐蜑蜒蜓蜔蛻蜖蝸蜘蜙蜚蜛蜜蜝蜞蜟蜠蠟蜢蜣蜤蜥蜦蜧蜨蜩蜪蜫蜬蜭蜮蜯蜰蜱蜲蜳蜴蜵蜶蜷蜸蜹蜺蜻蜼蜽蜾蜿蝀蝁蝂蝃蝄蝅蝆蠅蟈蟬蝊蝋蝌蝍蝎蝏蝐蝑蝒蝓蝔蝕蝖蝗蝘蝙蝚蝛蝜蝝蝞蝟蝠蝡蝢蝣蝤蝥蝦蝧蝨蝩蝪蝫蝬蝭蝮蝯蝰蝱蝲蝳蝴蝵蝶蝷蝸蝹蝺蝻螻蝽蠑蝿螀螁螂螃螄螅螆螇螈螉螊螋螌融螎螏螐螑螒螓螔螕螖螗螘螙螚螛螜螝螞螟螠螡螢螣螤螥螦螧螨螩螪螫螬螭螮螯螰螱螲螳螴螵螶螷螸螹螺螻螼螽螾螿蟀蟁蟂蟃蟄蟅蟆蟇蟈蟉蟊蟋蟌蟍蟎蟏蟐蟑蟒蟓蟔蟕蟖蟗蟘蟙蟚蟛蟜蟝蟞蟟蟠蟡蟢蟣蟤蟥蟦蟧蟨蟩蟪蟫蟬蟭蟮蟯蟰蟱蟲蟳蟴蟵蟶蟷蟸蟹蟺蟻蟼蟽蟾蟿蠀蠁蠂蠃蠄蠅蠆蠇蠈蠉蠊蠋蠌蠍蠎蠏蠐蠑蠒蠓蠔蠕蠖蠗蠘蠙蠚蠛蠜蠝蠞蠟蠠蠡蠢蠣蠤蠥蠦蠧蠨蠩蠪蠫蠬蠭蠮蠯蠰蠱蠲蠳蠴蠵蠶蠷蠸蠹蠺蠻蠼蠽蠾蠿血衁衂衃衄釁衆衇衈衉衊衋行衍衎衏衐衑衒術銜衕衖街衘衙衚衛衜衝衞衟衠衡衢衣衤補衦衧表衩衪衫襯衭袞衯衰衱衲衳衴衵衶衷衸衹衺衻衼衽衾衿袀袁袂袃襖裊袆袇袈袉袊袋袌袍袎袏袐袑袒袓袔袕袖袗袘袙袚袛襪袝袞袟袠袡袢袣袤袥袦袧袨袩袪被袬襲袮袯袰袱袲袳袴袵袶袷袸袹袺袻袼袽袾袿裀裁裂裃裄裝襠裇裈裉裊裋裌裍裎裏裐裑裒裓裔裕裖裗裘裙裚裛補裝裞裟裠裡褳襝褲裥裦裧裨裩裪裫裬裭裮裯裰裱裲裳裴裵裶裷裸裹裺裻裼製裾裿褀褁褂褃褄褅褆複褈褉褊褋褌褍褎褏褐褑褒褓褔褕褖褗褘褙褚褸褜褝褞褟褠褡褢褣褤褥褦褧褨褩褪褫褬褭褮褯褰褱褲褳襤褵褶褷褸褹褺褻褼褽褾褿襀襁襂襃襄襅襆襇襈襉襊襋襌襍襎襏襐襑襒襓襔襕襖襗襘襙襚襛襜襝襞襟襠襡襢襣襤襥襦襧襨襩襪襫襬襭襮襯襰襱襲襳襴襵襶襷襸襹襺襻襼襽襾西覀要覂覃覄覅覆覇覈覉覊見覌覍覎規覐覑覒覓覔覕視覗覘覙覚覛覜覝覞覟覠覡覢覣覤覥覦覧覨覩親覫覬覭覮覯覰覱覲観覴覵覶覷覸覹覺覻覼覽覾覿觀見觀觃規覓視覘覽覺覬覡覿觍覦覯覲覷角觓觔觕觖觗觘觙觚觛觜觝觴觟觠觡觢解觤觥觸觧觨觩觪觫觬觭觮觶觰觱觲觳觴觵觶觷觸觹觺觻觼觽觾觿言訁訂訃訄訅訆訇計訉訊訋訌訍討訏訐訑訒訓訔訕訖託記訙訚訛訜訝訞訟訠訡訢訣訤訥訦訧訨訩訪訫訬設訮訯訰許訲訳訴訵訶訷訸訹診註証訽訾訿詀詁詂詃詄詅詆詇詈詉詊詋詌詍詎詏詐詑詒詓詔評詖詗詘詙詚詛詜詝詞詟詠詡詢詣詤詥試詧詨詩詪詫詬詭詮詯詰話該詳詴詵詶詷詸詹詺詻詼詽詾詿誀誁誂誃誄誅誆誇誈譽謄誋誌認誎誏誐誑誒誓誔誕誖誗誘誙誚誛誜誝語誟誠誡誢誣誤誥誦誧誨誩說誫説読誮誯誰誱課誳誴誵誶誷誸誹誺誻誼誽誾調諀諁諂諃諄諅諆談諈諉諊請諌諍諎諏諐諑諒諓諔諕論諗諘諙諚諛諜諝諞諟諠諡諢諣諤諥諦諧諨諩諪諫諬諭諮諯諰諱諲諳諴諵諶諷諸諹諺諻諼諽諾諿謀謁謂謃謄謅謆謇謈謉謊謋謌謍謎謏謐謑謒謓謔謕謖謗謘謙謚講謜謝謞謟謠謡謢謣謤謥謦謧謨謩謪謫謬謭謮謯謰謱謲謳謴謵謶謷謸謹謺謻謼謽謾謿譀譁譂譃譄譅譆譇譈證譊譋譌譍譎譏譐譑譒譓譔譕譖譗識譙譚譛譜譝譞譟譠譡譢譣譤譥警譧譨譩譪譫譬譭譮譯議譱譲譳譴譵譶護譸譹譺譻譼譽譾譿讀讁讂讃讄讅讆讇讈讉變讋讌讍讎讏讐讑讒讓讔讕讖讗讘讙讚讛讜讝讞讟讠計訂訃認譏訐訌討讓訕訖讬訓議訊記讱講諱謳詎訝訥許訛論讻訟諷設訪訣證詁訶評詛識诇詐訴診詆謅詞詘詔诐譯詒誆誄試詿詩詰詼誠誅詵話誕詬詮詭詢詣諍該詳詫諢詡诪誡誣語誚誤誥誘誨誑說誦誒請諸諏諾讀諑誹課諉諛誰諗調諂諒諄誶談谉誼謀諶諜謊諫諧謔謁謂諤諭諼讒諮諳諺諦謎諞谞謨讜謖謝謠謗謚謙謐謹謾謫谫謬譚譖譙讕譜譎讞譴譫讖谷谸谹谺谻谼谽谾谿豀豁豂豃豄豅豆豇豈豉豊豋豌豍豎豏豐豑豒豓豔豕豖豗豘豙豚豛豜豝豞豟豠象豢豣豤豥豦豧豨豩豪豫豬豭豮豯豰豱豲豳豴豵豶豷豸豹豺豻豼豽豾豿貀貁貂貃貄貅貆貇貈貉貊貋貌貍貎貏貐貑貒貓貔貕貖貗貘貙貚貛貜貝貞貟負財貢貣貤貥貦貧貨販貪貫責貭貮貯貰貱貲貳貴貵貶買貸貹貺費貼貽貾貿賀賁賂賃賄賅賆資賈賉賊賋賌賍賎賏賐賑賒賓賔賕賖賗賘賙賚賛賜賝賞賟賠賡賢賣賤賥賦賧賨賩質賫賬賭賮賯賰賱賲賳賴賵賶賷賸賹賺賻購賽賾賿贀贁贂贃贄贅贆贇贈贉贊贋贌贍贎贏贐贑贒贓贔贕贖贗贘贙贚贛贜貝貞負贠貢財責賢敗賬貨質販貪貧貶購貯貫貳賤賁貰貼貴貺貸貿費賀貽賊贄賈賄貲賃賂贓資賅贐賕賑賚賒賦賭赍贖賞賜赑赒賡賠賧賴赗贅賻賺賽賾贗贊赟贈贍贏贛赤赥赦赧赨赩赪赫赬赭赮赯走赱赲赳赴趙趕起赸赹赺赻赼赽赾赿趀趁趂趃趄超趆趇趈趉越趨趌趍趎趏趐趑趒趓趔趕趖趗趘趙趚趛趜趝趞趟趠趡趢趣趤趥趦趧趨趩趪趫趬趭趮趯趰趲趲足趴趵趶趷躉趹趺趻趼趽趾趿跀跁跂躍蹌跅跆跇跈跉跊跋跌跍跎跏跐跑跒跓跔跕跖跗跘跙跚跛跜距躒跟跠跡跢跣跤跥跦跧跨跩跪跫跬跭跮路跰跱跲跳跴踐跶蹺蹕躚跺躋跼跽跾跿踀踁踂踃踄踅踆踇踈踉踴踋躊踍踎踏踐踑踒踓踔踕踖踗踘踙踚踛踜踝踞踟踠踡踢踣踤踥踦踧踨踩蹤踫躓踭踮躑踰踱踲踳踴踵踶踷踸踹踺踻踼踽踾踿蹀蹁蹂蹃蹄蹅蹆蹇蹈蹉蹊蹋蹌蹍蹎蹏蹐躡蹣蹓蹔蹕蹖蹗蹘蹙蹚蹛蹜蹝蹞蹟蹠蹡蹢蹣蹤蹥蹦蹧蹨蹩蹪蹫蹬蹭蹮蹯躕蹱蹲蹳蹴蹵蹶蹷蹸蹹蹺蹻蹼蹽蹾躥躀躁躂躃躄躅躆躇躈躉躊躋躌躍躎躪躐躑躒躓躔躕躖躗躘躙躚躛躦躝躞躟躠躡躢躣躤躥躦躧躨躩躪身躬躭躮軀躰躱躲躳躴躵躶躷躸躹躺躻躼躽躾躿軀軁軂軃軄軅軆軇軈軉車軋軌軍軎軏軐軑軒軓軔軕軖軗軘軙軚軛軜軝軞軟軠軡転軣軤軥軦軧軨軩軪軫軬軭軮軯軰軱軲軳軴軵軶軷軸軹軺軻軼軽軾軿輀輁輂較輄輅輆輇輈載輊輋輌輍輎輏輐輑輒輓輔輕輖輗輘輙輚輛輜輝輞輟輠輡輢輣輤輥輦輧輨輩輪輫輬輭輮輯輰輱輲輳輴輵輶輷輸輹輺輻輼輽輾輿轀轁轂轃轄轅轆轇轈轉轊轋轌轍轎轏轐轑轒轓轔轕轖轗轘轙轚轛轜轝轞轟轠轡轢轣轤轥車軋軌軒轪軔轉軛輪軟轟轱軻轤軸軹軼轷軫轢軺輕軾載輊轎辀輇輅較輒輔輛輦輩輝輥輞辌輟輜輳輻輯辒輸轡轅轄輾轆轍轔辛辜辝辭辟辠辡辢辣辤辥辦辧辨辯辪辮辬辭辮辯辰辱農辳辴辵辶辷辸邊辺辻込遼達辿迀遷迂迃迄迅迆過邁迉迊迋迌迍迎迏運近迒迓返迕迖迗還這迚進遠違連遲迠迡迢迣迤迥迦迧迨邇迪迫迬迭迮迯述迱迲逕迴迵迶迷迸跡迺迻迼追迾迿退送適逃逄逅逆逇逈選遜逋逌逍逎透逐逑遞逓途逕逖逗逘這通逛逜逝逞速造逡逢連逤逥邐逧逨逩逪逫逬逭逮逯逰週進逳逴逵逶逷逸逹逺邏逼逽逾逿遀遁遂遃遄遅遆遇遈遉遊運遌遍過遏遐遑遒道達違遖遺遘遙遚遛遜遝遞遟遠遡遢遣遤遙遦遧遨適遪遫遬遭遮遯遰遱遲遳遴遵遶遷選遹遺遻遼遽遾避邀邁邂邃還邅邆邇邈邉邊邋邌邍邎邏邐邑邒鄧邔邕邖邗邘邙邚邛邜鄺邞邟邠邡邢那邤邥邦邧邨邩邪邫鄔邭郵邯邰邱邲邳邴邵邶邷邸鄒鄴鄰邼邽邾邿郀郁郂郃郄郅郆郇郈郉郊郋郌郍郎郟鄶鄭郒鄆郔郕郖郗郘郙郚郛郜郝郞郟郠郡郢郣郤郥酈鄖部郩郪郫郬郭郮郯郰郱郲郳郴郵郶郷鄲郹郺郻郼都郾郿鄀鄁鄂鄃鄄鄅鄆鄇鄈鄉鄊鄋鄌鄍鄎鄏鄐鄑鄒鄓鄔鄕鄖鄗鄘鄙鄚鄛鄜鄝鄞鄟鄠鄡鄢鄣鄤鄥鄦鄧鄨鄩鄪鄫鄬鄭鄮鄯鄰鄱鄲鄳鄴鄵鄶鄷鄸鄹鄺鄻鄼鄽鄾鄿酀酁酂酃酄酅酆酇酈酉酊酋酌配酎酏酐酑酒酓酔酕酖酗酘酙酚酛酜醞酞酟酠酡酢酣酤酥酦酧酨酩酪酫酬酭酮酯酰醬酲酳酴酵酶酷酸酹酺酻酼釅釃釀醀醁醂醃醄醅醆醇醈醉醊醋醌醍醎醏醐醑醒醓醔醕醖醗醘醙醚醛醜醝醞醟醠醡醢醣醤醥醦醧醨醩醪醫醬醭醮醯醰醱醲醳醴醵醶醷醸醹醺醻醼醽醾醿釀釁釂釃釄釅釆采釈釉釋釋里重野量釐金釒釓釔釕釖釗釘釙釚釛釜針釞釟釠釡釢釣釤釥釦釧釨釩釪釫釬釭釮釯釰釱釲釳釴釵釶釷釸釹釺釻釼釽釾釿鈀鈁鈂鈃鈄鈅鈆鈇鈈鈉鈊鈋鈌鈍鈎鈏鈐鈑鈒鈓鈔鈕鈖鈗鈘鈙鈚鈛鈜鈝鈞鈟鈠鈡鈢鈣鈤鈥鈦鈧鈨鈩鈪鈫鈬鈭鈮鈯鈰鈱鈲鈳鈴鈵鈶鈷鈸鈹鈺鈻鈼鈽鈾鈿鉀鉁鉂鉃鉄鉅鉆鉇鉈鉉鉊鉋鉌鉍鉎鉏鉐鉑鉒鉓鉔鉕鉖鉗鉘鉙鉚鉛鉜鉝鉞鉟鉠鉡鉢鉣鉤鉥鉦鉧鉨鉩鉪鉫鉬鉭鉮鉯鉰鉱鉲鉳鑒鉵鉶鉷鉸鉹鉺鉻鉼鉽鉾鉿銀銁銂銃銄銅銆銇銈銉銊銋銌銍銎銏銐銑銒銓銔銕銖銗銘銙銚銛銜銝銞銟銠銡銢銣銤銥銦銧銨銩銪銫銬銭鑾銯銰銱銲銳銴銵銶銷銸銹銺銻銼銽銾銿鋀鋁鋂鋃鋄鋅鋆鋇鋈鋉鋊鋋鋌鋍鋎鋏鋐鋑鋒鋓鋔鋕鋖鋗鋘鋙鋚鋛鋜鋝鋞鋟鋠鋡鋢鋣鋤鋥鋦鋧鋨鋩鋪鋫鋬鋭鋮鋯鋰鋱鋲鋳鋴鋵鋶鋷鋸鋹鋺鋻鋼鋽鋾鋿錀錁錂錃錄錅錆錇錈錉錊錋錌錍錎錏錐錑錒錓錔錕錖錗錘錙錚錛錜錝錞錟錠錡錢錣錤錥錦錧錨錩錪錫錬錭錮錯錰錱録錳錴錵錶錷錸錹錺錻錼錽鏨錿鍀鍁鍂鍃鍄鍅鍆鍇鍈鍉鍊鍋鍌鍍鍎鍏鍐鍑鍒鍓鍔鍕鍖鍗鍘鍙鍚鍛鍜鍝鍞鍟鍠鍡鍢鍣鍤鍥鍦鍧鍨鍩鍪鍫鍬鍭鍮鍯鍰鍱鍲鍳鍴鍵鍶鍷鍸鍹鍺鍻鍼鍽鍾鍿鎀鎁鎂鎃鎄鎅鎆鎇鎈鎉鎊鎋鎌鎍鎎鎏鎐鎑鎒鎓鎔鎕鎖鎗鎘鎙鎚鎛鎜鎝鎞鎟鎠鎡鎢鎣鎤鎥鎦鎧鎨鎩鎪鎫鎬鎭鎮鎯鎰鎱鎲鎳鎴鎵鎶鎷鎸鎹鎺鎻鎼鎽鎾鎿鏀鏁鏂鏃鏄鏅鏆鏇鏈鏉鏊鏋鏌鏍鏎鏏鏐鏑鏒鏓鏔鏕鏖鏗鏘鏙鏚鏛鏜鏝鏞鏟鏠鏡鏢鏣鏤鏥鏦鏧鏨鏩鏪鏫鏬鏭鏮鏯鏰鏱鏲鏳鏴鏵鏶鏷鏸鏹鏺鏻鏼鏽鏾鏿鐀鐁鐂鐃鐄鐅鐆鐇鐈鐉鐊鐋鐌鐍鐎鐏鐐鐑鐒鐓鐔鐕鐖鐗鐘鐙鐚鐛鐜鐝鐞鐟鐠鐡鐢鐣鐤鐥鐦鐧鐨鐩鐪鐫鐬鐭鐮鐯鐰鐱鐲鐳鐴鐵鐶鐷鐸鐹鐺鐻鐼鐽鐾鐿鑀鑁鑂鑃鑄鑅鑆鑇鑈鑉鑊鑋鑌鑍鑎鑏鑐鑑鑒鑓鑔鑕鑖鑗鑘鑙鑚鑛鑜鑝鑞鑟鑠鑡鑢鑣鑤鑥鑦鑧鑨鑩鑪鑫鑬鑭鑮鑯鑰鑱鑲鑳鑴鑵鑶鑷鑸鑹鑺鑻鑼鑽鑾鑿钀钁钂钃钄钅釓釔針釘釗釙釕釷釬釧釤钑釩釣鍆釹钖釵钘鈣钚鈦鉅鈍鈔鐘鈉鋇鋼鈑鈐鑰欽鈞鎢鉤鈧鈁鈥鈄鈕鈀鈺錢鉦鉗鈷缽鈳钷鈽鈸鉞鉆鉬鉭鉀鈿鈾鐵鉑鈴鑠鉛鉚铇鈰鉉鉈鉍鈮鈹鐸铏銬銠鉺铓铔銪鋮鋏铘鐃铚鐺銅鋁铞銦鎧鍘銖銑鋌銩铦鏵銓鎩鉿銚鉻銘錚銫鉸銥鏟銃鐋銨銀銣鑄鐒鋪铻錸鋱鏈鏗銷鎖鋰锃鋤鍋鋯鋨銹銼鋝鋒鋅锍锎锏銳銻鋃鋟鋦錒錆鍺锘錯錨錛锜锝錁錕锠錫錮鑼錘錐錦锧锨錈锪锫錟錠鍵鋸錳錙鍥锳鍇鏘鍶鍔鍤鍬鍾鍛鎪锽鍰锿鍍鎂鏤镃鐨镅鏌鎮镈鎘鑷镋鐫鎳镎鎦鎬鎊鎰鎵鑌镕鏢鏜鏝鏍镚鏞鏡鏑鏃鏇镠鐔镢鐐鏷镥鐓鑭鐠镩鏹鐙鑊鐳镮鐲鐮鐿镲鑣镴镵鑲長镸镹镺镻镼镽镾長門閁閂閃閄閅閆閇閈閉閊開閌閍閎閏閐閑閒間閔閕閖閗閘閙閚閛閜閝閞閟閠閡関閣閤閥閦閧閨閩閪閫閬閭閮閯閰閱閲閳閴閵閶閷閸閹閺閻閼閽閾閿闀闁闂闃闄闅闆闇闈闉闊闋闌闍闎闏闐闑闒闓闔闕闖闗闘闙闚闛關闝闞闟闠闡闢闣闤闥闦闧門閂閃閆闬閉問闖閏闈閑閎間閔閌悶閘鬧閨聞闥閩閭闿閥閣閡閫鬮閱閬阇閾閹閶鬩閿閽閻閼闡闌闃阓闊闋闔闐阘闕闞阛阜阝阞隊阠阡阢阣阤阥阦阧阨阩阪阫阬阭阮阯阰阱防陽陰陣階阷阸阹阺阻阼阽阾阿陀陁陂陃附際陸隴陳陘陊陋陌降陎陏限陑陒陓陔陜陖陗陘陙陚陛陜陝陞陟陠陡院陣除陥陦隉隕險陪陫陬陭陮陯陰陱陲陳陴陵陶陷陸陹険陻陼陽陾陿隀隁隂隃隄隅隆隇隈隉隊隋隌隍階隨隱隑隒隓隔隕隖隗隘隙隚際障隝隞隟隠隡隢隣隤隥隦隧隨隩險隫隬隭隮隯隰隱隲隳隴隵隸隷隸隹隺隻隼雋難隿雀雁雂雃雄雅集雇雈雉雊雋雌雍雎雛雐雑雒雓雔雕雖雗雘雙雚雛雜雝雞雟讎雡離難雤雥雦雧雨雩雪雫雬雭雮雯雰雱雲靂雴雵零雷雸雹雺電雼雽霧雿需霽霂霃霄霅霆震霈霉霊霋霌霍霎霏霐霑霒霓霔霕霖霗霘霙霚霛霜霝霞霟霠霡霢霣霤霥霦霧霨霩霪霫霬靄霮霯霰霱露霳霴霵霶霷霸霹霺霻霼霽霾霿靀靁靂靃靄靅靆靇靈靉靊靋靌靍靎靏靐靑青靚靔靕靖靗靘靜靚靛靜靝非靟靠靡面靣靤靨靦靧靨革靪靫靬靭靮靯靰靱靲靳靴靵靶靷靸靹靺靻靼靽靾靿鞀鞁鞂鞃鞄鞅鞆鞇鞈鞉鞊鞋鞌鞍鞎鞏鞐韃鞒鞓鞔鞕鞖鞗鞘鞙鞚鞛鞜鞝鞞鞟鞠鞡鞢鞣鞤鞥鞦鞧鞨鞩鞪鞫鞬鞭鞮韉鞰鞱鞲鞳鞴鞵鞶鞷鞸鞹鞺鞻鞼鞽鞾鞿韀韁韂韃韄韅韆韇韈韉韊韋韌韍韎韏韐韑韒韓韔韕韖韗韘韙韚韛韜韝韞韟韠韡韢韣韤韥韋韌韨韓韙韞韜韭韮韯韰韱韲音韴韻韶韷韸韹韺韻韼韽韾響頀頁頂頃頄項順頇須頉頊頋頌頍頎頏預頑頒頓頔頕頖頗領頙頚頛頜頝頞頟頠頡頢頣頤頥頦頧頨頩頪頫頬頭頮頯頰頱頲頳頴頵頶頷頸頹頺頻頼頽頾頿顀顁顂顃顄顅顆顇顈顉顊顋題額顎顏顐顑顒顓顔顕顖顗願顙顚顛顜顝類顟顠顡顢顣顤顥顦顧顨顩顪顫顬顭顮顯顰顱顲顳顴頁頂頃頇項順須頊頑顧頓頎頒頌頏預顱領頗頸頡頰颋頜潁颎頦頤頻颒頹頷颕穎顆題颙顎顓顏額顳顢顛顙顥颣顫颥顰顴風颩颪颫颬颭颮颯颰颱颲颳颴颵颶颷颸颹颺颻颼颽颾颿飀飁飂飃飄飅飆飇飈飉飊飋飌飍風飏飐颮颯颶飔颼飖飗飄飆飚飛飜飝飛食飠飡飢飣飤飥飦飧饗飩飪飫飬飭飮飯飰飱飲飳飴飵飶飷飸飹飺飻飼飽飾飿餀餁餂餃餄餅餆餇餈餉養餋餌饜餎餏餐餑餒餓餔餕餖餗餘餙餚餛餜餝餞餟餠餡餢餣餤餥餦餧館餩餪餫餬餭餮餯餰餱餲餳餴餵餶餷餸餹餺餻餼餽餾餿饀饁饂饃饄饅饆饇饈饉饊饋饌饍饎饏饐饑饒饓饔饕饖饗饘饙饚饛饜饝饞饟饠饡饢饣饤饑饦餳飩餼飪飫飭飯飲餞飾飽飼饳飴餌饒餉饸饹餃饻餅餑饾餓馀餒馂馃餛餡館馇饋馉餿饞馌饃馎餾饈饉饅馓饌馕首馗馘香馚馛馜馝馞馟馠馡馢馣馤馥馦馧馨馩馪馫馬馭馮馯馰馱馲馳馴馵馶馷馸馹馺馻馼馽馾馿駀駁駂駃駄駅駆駇駈駉駊駋駌駍駎駏駐駑駒駓駔駕駖駗駘駙駚駛駜駝駞駟駠駡駢駣駤駥駦駧駨駩駪駫駬駭駮駯駰駱駲駳駴駵駶駷駸駹駺駻駼駽駾駿騀騁騂騃騄騅騆騇騈騉騊騋騌騍騎騏騐騑騒験騔騕騖騗騘騙騚騛騜騝騞騟騠騡騢騣騤騥騦騧騨騩騪騫騬騭騮騯騰騱騲騳騴騵騶騷騸騹騺騻騼騽騾騿驀驁驂驃驄驅驆驇驈驉驊驋驌驍驎驏驐驑驒驓驔驕驖驗驘驙驚驛驜驝驞驟驠驡驢驣驤驥驦驧驨驩驪驫馬馭馱馴馳驅驲駁驢駔駛駟駙駒騶駐駝駑駕驛駘驍罵骃驕驊駱駭駢骉驪騁驗骍骎駿騏騎騍騅骔骕驂騙騭骙騷騖驁騮騫騸驃騾驄驏驟驥骦驤骨骩骪骫骬骭骮骯骰骱骲骳骴骵骶骷骸骹骺骻骼骽骾骿髀髁髂髃髄髏髆髇髈髉髊髖髕髍髎髏髐髑髒髓體髕髖髗高髙髚髛髜髝髞髟髠髡髢髣髤髥髦髧髨髩髪髫髬髭髮髯髰髱髲髳髴髵髶髷髸髹髺髻髼髽髾髿鬀鬁鬂鬃鬄鬅鬆鬇鬈鬉鬊鬋鬌鬍鬎鬏鬐鬑鬒鬢鬔鬕鬖鬗鬘鬙鬚鬛鬜鬝鬞鬟鬠鬡鬢鬣鬤鬥鬦鬧鬨鬩鬪鬫鬬鬭鬮鬯鬰鬱鬲鬳鬴鬵鬶鬷鬸鬹鬺鬻鬼鬽鬾鬿魀魁魂魃魄魅魆魘魈魎魊魋魌魍魎魏魐魑魒魓魔魕魖魗魘魙魚魛魜魝魞魟魠魡魢魣魤魥魦魧魨魩魪魫魬魭魮魯魰魱魲魳魴魵魶魷魸魹魺魻魼魽魾魿鮀鮁鮂鮃鮄鮅鮆鮇鮈鮉鮊鮋鮌鮍鮎鮏鮐鮑鮒鮓鮔鮕鮖鮗鮘鮙鮚鮛鮜鮝鮞鮟鮠鮡鮢鮣鮤鮥鮦鮧鮨鮩鮪鮫鮬鮭鮮鮯鮰鮱鮲鮳鮴鮵鮶鮷鮸鮹鮺鮻鮼鮽鮾鮿鯀鯁鯂鯃鯄鯅鯆鯇鯈鯉鯊鯋鯌鯍鯎鯏鯐鯑鯒鯓鯔鯕鯖鯗鯘鯙鯚鯛鯜鯝鯞鯟鯠鯡鯢鯣鯤鯥鯦鯧鯨鯩鯪鯫鯬鯭鯮鯯鯰鯱鯲鯳鯴鯵鯶鯷鯸鯹鯺鯻鯼鯽鯾鯿鰀鰁鰂鰃鰄鰅鰆鰇鰈鰉鰊鰋鰌鰍鰎鰏鰐鰑鰒鰓鰔鰕鰖鰗鰘鰙鰚鰛鰜鰝鰞鰟鰠鰡鰢鰣鰤鰥鰦鰧鰨鰩鰪鰫鰬鰭鰮鰯鰰鰱鰲鰳鰴鰵鰶鰷鰸鰹鰺鰻鰼鰽鰾鰿鱀鱁鱂鱃鱄鱅鱆鱇鱈鱉鱊鱋鱌鱍鱎鱏鱐鱑鱒鱓鱔鱕鱖鱗鱘鱙鱚鱛鱜鱝鱞鱟鱠鱡鱢鱣鱤鱥鱦鱧鱨鱩鱪鱫鱬鱭鱮鱯鱰鱱鱲鱳鱴鱵鱶鱷鱸鱹鱺鱻魚鱽鱾魷鲀魯魴鲃鲄鲅鲆鲇鱸鲉鲊鮒鲌鮑鱟鲏鮐鮭鮚鲓鮪鮞鲖鲗鲘鲙鱭鮫鮮鲝鲞鱘鯁鱺鰱鰹鯉鰣鰷鯀鯊鯇鲪鯽鲬鯖鯪鲯鯫鯡鯤鯧鲴鯢鯰鯛鯨鲹鲺鯔鲼鰈鲾鲿鳀鳁鳂鰓鱷鰍鰒鰉鳈鳉鳊鳋鰲鰭鰨鰥鰩鳑鳒鰳鰾鱈鱉鰻鳘鳙鳚鳛鱖鱔鱗鱒鳠鳡鱧鳣鳤鳥鳦鳧鳨鳩鳪鳫鳬鳭鳮鳯鳰鳱鳲鳳鳴鳵鳶鳷鳸鳹鳺鳻鳼鳽鳾鳿鴀鴁鴂鴃鴄鴅鴆鴇鴈鴉鴊鴋鴌鴍鴎鴏鴐鴑鴒鴓鴔鴕鴖鴗鴘鴙鴚鴛鴜鴝鴞鴟鴠鴡鴢鴣鴤鴥鴦鴧鴨鴩鴪鴫鴬鴭鴮鴯鴰鴱鴲鴳鴴鴵鴶鴷鴸鴹鴺鴻鴼鴽鴾鴿鵀鵁鵂鵃鵄鵅鵆鵇鵈鵉鵊鵋鵌鵍鵎鵏鵐鵑鵒鵓鵔鵕鵖鵗鵘鵙鵚鵛鵜鵝鵞鵟鵠鵡鵢鵣鵤鵥鵦鵧鵨鵩鵪鵫鵬鵭鵮鵯鵰鵱鵲鵳鵴鵵鵶鵷鵸鵹鵺鵻鵼鵽鵾鵿鶀鶁鶂鶃鶄鶅鶆鶇鶈鶉鶊鶋鶌鶍鶎鶏鶐鶑鶒鶓鶔鶕鶖鶗鶘鶙鶚鶛鶜鶝鶞鶟鶠鶡鶢鶣鶤鶥鶦鶧鶨鶩鶪鶫鶬鶭鶮鶯鶰鶱鶲鶳鶴鶵鶶鶷鶸鶹鶺鶻鶼鶽鶾鶿鷀鷁鷂鷃鷄鷅鷆鷇鷈鷉鷊鷋鷌鷍鷎鷏鷐鷑鷒鷓鷔鷕鷖鷗鷘鷙鷚鷛鷜鷝鷞鷟鷠鷡鷢鷣鷤鷥鷦鷧鷨鷩鷪鷫鷬鷭鷮鷯鷰鷱鷲鷳鷴鷵鷶鷷鷸鷹鷺鷻鷼鷽鷾鷿鸀鸁鸂鸃鸄鸅鸆鸇鸈鸉鸊鸋鸌鸍鸎鸏鸐鸑鸒鸓鸔鸕鸖鸗鸘鸙鸚鸛鸜鸝鸞鳥鳩雞鳶鳴鸤鷗鴉鸧鴇鴆鴣鶇鸕鴨鸮鴦鸰鴟鴝鴛鸴鴕鷥鷙鴯鴰鵂鸻鸼鴿鸞鴻鹀鵓鸝鵑鵠鵝鵒鷴鵜鵡鵲鹋鵪鹍鵯鵬鹐鶉鹒鹓鹔鶘鹖鶚鶻鹙鶿鹛鶩鹝鷂鹟鹠鹡鹢鶼鶴鹥鸚鷓鷚鷯鷦鷲鷸鷺鹮鹯鷹鹱鹲鸛鹴鹵鹶鹷鹸鹹鹺鹻鹼鹽鹺鹿麀麁麂麃麄麅麆麇麈麉麊麋麌麍麎麏麐麑麒麓麔麕麖麗麘麙麚麛麜麝麞麟麠麡麢麣麤麥麥麧麨麩麪麫麬麭麮麯麰麱麲麳麴麵麶麷麩麹麺麻麼麼麾麿黀黁黂黃黃黅黆黇黈黌黊黋黌黍黎黏黐黑黒黓黔黕黖黗默黙黚黛黜黝點黟黠黡黢黣黤黥黦黧黨黷黲黫黬黭黮黯黰黱黲黳黴黵黶黷黸黹黺黻黼黽黽黿鼀鼁鼂鼃鼄鼅鼆鼇鼈鼉鼊黿鼌鼉鼎鼏鼐鼑鼒鼓鼔鼕鼖鼗鼘鼙鼚鼛鼜鼝鼞鼟鼠鼡鼢鼣鼤鼥鼦鼧鼨鼩鼪鼫鼬鼭鼮鼯鼰鼱鼲鼳鼴鼵鼶鼷鼸鼴鼺鼻鼼鼽鼾鼿齀齁齂齃齄齅齆齇齈齉齊齋齌齍齎齏齊齏齒齓齔齕齖齗齘齙齚齛齜齝齞齟齠齡齢齣齤齥齦齧齨齩齪齫齬齭齮齯齰齱齲齳齴齵齶齷齸齹齺齻齼齽齾齒齔龁龂齟齡齙齠齜齦齬齪齲齷龍龎龏龐龑龒龓龔龕龖龗龘龍龔龕龜龝龞龜龠龡龢龣龤龥";

            string strSource;
            string strTarget;
            if (format == 1)
            {
                strSource = Traditional;
                strTarget = Simplified;
            }
            else
            {
                strSource = Simplified;
                strTarget = Traditional;
            }
            if (text.Length <= 0)
            {
                return text;
            }
            var arrText = text.ToCharArray();
            for (var i = 0; i < arrText.Length; i++)
            {
                var intChar = Convert.ToInt32(arrText[i]);
                if (intChar < 0x4e00 || intChar > 0x9fa5)
                {
                    continue;
                }

                intChar = strSource.IndexOf(arrText[i]);
                if (intChar >= 0)
                {
                    arrText[i] = strTarget[intChar];
                }
            }
            return new string(arrText);
        }

        /// <summary>
        /// 数字转中文金额大写
        /// </summary>
        /// <param name="number">22.22</param>
        /// <returns></returns>
        public static string ToChineseMoney(this double number)
        {
            string s = number.ToString("#L#E#D#C#K#E#D#C#J#E#D#C#I#E#D#C#H#E#D#C#G#E#D#C#F#E#D#C#.0B0A");
            string d = Regex.Replace(s, @"((?<=-|^)[^1-9]*)|((?'z'0)[0A-E]*((?=[1-9])|(?'-z'(?=[F-L\.]|$))))|((?'b'[F-L])(?'z'0)[0A-L]*((?=[1-9])|(?'-z'(?=[\　　.]|$))))", "${b}${z}");
            return Regex.Replace(d, ".", m => "负元空零壹贰叁肆伍陆柒捌玖空空空空空空空分角拾佰仟萬億兆京垓秭穰"[m.Value[0] - '-'].ToString());
        }

        /// <summary>
        /// 字符串转byte[]
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static byte[] ToUtf8Bytes(this string text)
        {
            return System.Text.Encoding.UTF8.GetBytes(text);
        }

        /// <summary>
        /// GB2312转byte[]
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static byte[] ToDefaultByte(this string text)
        {
            return System.Text.Encoding.Default.GetBytes(text);
        }

        /// <summary>
        /// 实现各进制数间的转换。ConvertBase("15",10,16)表示将十进制数15转换为16进制的数。
        /// </summary>
        /// <param name="value">要转换的值,即原值</param>
        /// <param name="from">原值的进制,只能是2,8,10,16四个值。</param>
        /// <param name="to">要转换到的目标进制，只能是2,8,10,16四个值。</param>
        public static string ToBase(this string value, int from, int to)
        {
            try
            {
                int intValue = Convert.ToInt32(value, from);  //先转成10进制
                string result = Convert.ToString(intValue, to);  //再转成目标进制
                if (to == 2)
                {
                    int resultLength = result.Length;  //获取二进制的长度
                    switch (resultLength)
                    {
                        case 7:
                            result = "0" + result;
                            break;
                        case 6:
                            result = "00" + result;
                            break;
                        case 5:
                            result = "000" + result;
                            break;
                        case 4:
                            result = "0000" + result;
                            break;
                        case 3:
                            result = "00000" + result;
                            break;
                    }
                }
                return result;
            }
            catch
            {
                return "";
            }
        }

        /// <summary>
        /// 图片音频等文件转成Base64字符串
        /// </summary>
        /// <param name="path"></param>
        /// <returns>bool 是否成果  base64</returns>
        public static (bool, string) ToFileToBase64String(this string path)
        {
            try
            {
                string result = "";
                using (FileStream stream = new FileStream(path, FileMode.Open))
                {
                    byte[] bt = new byte[stream.Length];
                    stream.Read(bt, 0, bt.Length);
                    result = Convert.ToBase64String(bt);
                }
                return (true, result);
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        /// <summary>
        /// Base64字符串转换成图片音频等文件
        /// </summary>
        /// <param name="source">base64</param>
        /// <param name="path">文件路径</param>
        /// <returns>bool 是否成果  结果提示 </returns>
        public static (bool, string) ToBase64StringToFile(this string source, string path)
        {
            try
            {
                byte[] bytes = Convert.FromBase64String(source);
                using (var fs = new FileStream(path, FileMode.Create, FileAccess.Write))
                {
                    fs.Write(bytes, 0, bytes.Length);
                    fs.Flush();
                }
                return (true, "success");
            }
            catch (Exception e)
            {
                return (false, e.Message);
            }
        }

        /// <summary>
        /// 将字节大小的值转成人类易读的字符串
        /// </summary>
        /// <param name="bytes">字节大小</param>
        public static string ToByteToSize(this double bytes)
        {
            double KiloBytes = 1024;
            double MegaBytes = KiloBytes * KiloBytes;
            double GigaBytes = KiloBytes * MegaBytes;
            var abs = System.Math.Abs(bytes);
            if (abs == 0) return $"0 B";
            if (abs >= GigaBytes)
            {
                return $"{bytes / GigaBytes:#,#.##} GB";
            }
            if (abs >= MegaBytes)
            {
                return $"{bytes / MegaBytes:#,#.##} MB";
            }
            if (abs >= KiloBytes)
            {
                return $"{bytes / KiloBytes:#,#.##} KB";
            }
            return $"{bytes:#,#} B";
        }


        /// <summary>
        /// 判断当前字符串是否在目标字符串数组中
        /// </summary>
        /// <param name="this">字符串</param>
        /// <param name="values">字符串数组</param>
        /// <returns></returns>
        public static bool In(this string @this, params string[] values)
        {
            return Array.IndexOf(values, @this) != -1;
        }

        /// <summary>
        /// 判断当前字符串是否不在目标字符串数组中
        /// </summary>
        /// <param name="this">字符串</param>
        /// <param name="values">字符串数组</param>
        /// <returns></returns>
        public static bool NotIn(this string @this, params string[] values)
        {
            return Array.IndexOf(values, @this) == -1;
        }

        /// <summary>
        /// 将数值四舍五入，保留指定小数位数
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="decimals">小数位数</param>
        /// <returns></returns>
        public static decimal ToDecimalRound(this decimal value, int decimals = 2)
        {
            return Math.Round(value, decimals);
        }

        /// <summary>
        /// 判断当前值是否在指定范围内
        /// </summary>
        /// <param name="value">double</param>
        /// <param name="minValue">最小值</param>
        /// <param name="maxValue">最大值</param>
        /// <returns>bool</returns>
        public static bool InNumberRange(this decimal value, decimal minValue, decimal maxValue)
        {
            return (value >= minValue && value <= maxValue);
        }

        /// <summary>
        /// 判断值是否在指定范围内，否则返回默认值
        /// </summary>
        /// <param name="value">double</param>
        /// <param name="minValue">最小值</param>
        /// <param name="maxValue">最大值</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>double</returns>
        public static decimal InNumberRange(this decimal value, decimal minValue, decimal maxValue, decimal defaultValue)
        {
            return value.InNumberRange(minValue, maxValue) ? value : defaultValue;
        }

        /// <summary>
        /// 以指定字符串作为分隔符将指定字符串分隔成数组
        /// </summary>
        /// <param name="value">要分割的字符串</param>
        /// <param name="strSplit">字符串类型的分隔符</param>
        /// <param name="removeEmptyEntries">是否移除数据中元素为空字符串的项</param>
        /// <returns>分割后的数据</returns>
        public static string[] SplitArray(this string value, string strSplit = ",", bool removeEmptyEntries = false)
        {
            return value.Split(new[] { strSplit }, removeEmptyEntries ? StringSplitOptions.RemoveEmptyEntries : StringSplitOptions.None);
        }

        public static List<int> SplitIntList(this string value, string strSplit = ",", bool removeEmptyEntries = false)
        {
            var listValue = value.SplitArray(strSplit, removeEmptyEntries);
            return listValue.Select(t => t.ToInt()).ToList();
        }

        public static List<long> SplitLongList(this string value, string strSplit = ",", bool removeEmptyEntries = false)
        {
            var listValue = value.SplitArray(strSplit, removeEmptyEntries);
            return listValue.Select(t => t.ToLong()).ToList();
        }

        /// <summary>
        /// 将byte[]转换成字符串，默认字符编码：<see cref="Encoding.UTF8"/>
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="encoding">字符编码</param>
        public static string ToEncodingString(this byte[] value, Encoding encoding)
        {
            encoding = (encoding ?? Encoding.UTF8);
            return encoding.GetString(value);
        }

        /// <summary>
        /// 三则运算
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="value1"></param>
        /// <param name="value2"></param>
        /// <returns></returns>
        public static string ToTrueOrFalseString(this bool condition, string value1 = "", string value2 = "")
        {
            if (condition)
            {
                return value1;
            }
            return value2;
        }

        /// <summary>
        /// 三则运算
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="value1"></param>
        /// <param name="value2"></param>
        /// <returns></returns>
        public static int ToTrueOrFalseInt(this bool condition, int value1 = 0, int value2 = 0)
        {
            if (condition)
            {
                return value1;
            }
            return value2;
        }

        /// <summary>
        /// 三则运算
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="value1"></param>
        /// <param name="value2"></param>
        /// <returns></returns>
        public static long ToTrueOrFalseLong(this bool condition, long value1 = 0, long value2 = 0)
        {
            if (condition)
            {
                return value1;
            }
            return value2;
        }

        /// <summary>
        /// 三则运算
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="value1"></param>
        /// <param name="value2"></param>
        /// <returns></returns>
        public static decimal ToTrueOrFalseDecimal(this bool condition, decimal value1 = 0, decimal value2 = 0)
        {
            if (condition)
            {
                return value1;
            }
            return value2;
        }

        /// <summary>
        /// 三则运算
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="value1"></param>
        /// <param name="value2"></param>
        /// <returns></returns>
        public static bool ToTrueOrFalseBool(this bool condition, bool value1 = true, bool value2 = false)
        {
            if (condition)
            {
                return value1;
            }
            return value2;
        }

        /// <summary>
        /// 数字相等
        /// </summary>
        /// <param name="value"></param>
        /// <param name="value2"></param>
        /// <returns></returns>
        public static bool EqInt(this int value, int value2)
        {
            return value == value2;
        }

        /// <summary>
        /// 数字相等
        /// </summary>
        /// <param name="value"></param>
        /// <param name="value2"></param>
        /// <returns></returns>
        public static bool EqLong(this long value, long value2)
        {
            return value == value2;
        }

        /// <summary>
        /// 数字相等
        /// </summary>
        /// <param name="value"></param>
        /// <param name="value2"></param>
        /// <returns></returns>
        public static bool EqDecimal(this decimal value, decimal value2)
        {
            return value == value2;
        }

        /// <summary>
        /// 数字大于
        /// </summary>
        /// <param name="value"></param>
        /// <param name="value2"></param>
        /// <returns></returns>
        public static bool GtInt(this int value, int value2)
        {
            return value > value2;
        }

        /// <summary>
        /// 数字大于
        /// </summary>
        /// <param name="value"></param>
        /// <param name="value2"></param>
        /// <returns></returns>
        public static bool GtLong(this long value, long value2)
        {
            return value > value2;
        }

        /// <summary>
        /// 数字大于
        /// </summary>
        /// <param name="value"></param>
        /// <param name="value2"></param>
        /// <returns></returns>
        public static bool GtDecimal(this decimal value, decimal value2)
        {
            return value > value2;
        }

        /// <summary>
        /// 数字大于等于
        /// </summary>
        /// <param name="value"></param>
        /// <param name="value2"></param>
        /// <returns></returns>
        public static bool GtEqInt(this int value, int value2)
        {
            return value >= value2;
        }

        /// <summary>
        /// 数字大于等于
        /// </summary>
        /// <param name="value"></param>
        /// <param name="value2"></param>
        /// <returns></returns>
        public static bool GtEqLong(this long value, long value2)
        {
            return value >= value2;
        }

        /// <summary>
        /// 数字大于等于
        /// </summary>
        /// <param name="value"></param>
        /// <param name="value2"></param>
        /// <returns></returns>
        public static bool GtEqDecimal(this decimal value, decimal value2)
        {
            return value >= value2;
        }

        /// <summary>
        /// 数字小于
        /// </summary>
        /// <param name="value"></param>
        /// <param name="value2"></param>
        /// <returns></returns>
        public static bool LtInt(this int value, int value2)
        {
            return value < value2;
        }

        /// <summary>
        /// 数字小于
        /// </summary>
        /// <param name="value"></param>
        /// <param name="value2"></param>
        /// <returns></returns>
        public static bool LtLong(this long value, long value2)
        {
            return value < value2;
        }

        /// <summary>
        /// 数字小于
        /// </summary>
        /// <param name="value"></param>
        /// <param name="value2"></param>
        /// <returns></returns>
        public static bool LtDecimal(this decimal value, decimal value2)
        {
            return value < value2;
        }

        /// <summary>
        /// 数字小于等于
        /// </summary>
        /// <param name="value"></param>
        /// <param name="value2"></param>
        /// <returns></returns>
        public static bool LtEqInt(this int value, int value2)
        {
            return value <= value2;
        }

        /// <summary>
        /// 数字小于等于
        /// </summary>
        /// <param name="value"></param>
        /// <param name="value2"></param>
        /// <returns></returns>
        public static bool LtEqLong(this long value, long value2)
        {
            return value <= value2;
        }

        /// <summary>
        /// 数字小于等于
        /// </summary>
        /// <param name="value"></param>
        /// <param name="value2"></param>
        /// <returns></returns>
        public static bool LtEqDecimal(this decimal value, decimal value2)
        {
            return value <= value2;
        }

        /// <summary>
        /// 分转元
        /// </summary>
        /// <param name="points">金额，单位分</param>
        /// <returns></returns>
        public static decimal PointsToYuan(this int points)
        {
            return points / 100.00M;
        }

        /// <summary>
        /// 元转分
        /// </summary>
        /// <param name="yuan">金额，单位元</param>
        /// <returns></returns>
        public static int YuanToPoints(this decimal yuan)
        {
            return (int)(yuan * 100);
        }

        #endregion

        #region 数组操作

        /// <summary>
        /// 数组差集
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="val1">数组1</param>
        /// <param name="val2">数组2</param>
        /// <returns></returns>
        public static T[] Except<T>(this T[] val1, T[] val2)
        {
            return val1.Except<T>(val2).ToArray();
        }

        /// <summary>
        /// 数组交集
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="val1">数组1</param>
        /// <param name="val2">数组2</param>
        /// <returns></returns>
        public static T[] Intersect<T>(this T[] val1, T[] val2)
        {
            return val1.Intersect<T>(val2).ToArray();
        }

        /// <summary>
        /// 数组并集
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="val1">数组1</param>
        /// <param name="val2">数组2</param>
        /// <returns></returns>
        public static T[] Union<T>(this T[] val1, T[] val2)
        {
            return val1.Union<T>(val2).ToArray();
        }

        /// <summary>
        /// 数组转换成字符串拼接
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="val1">数组 string[]</param>
        /// <param name="chat">分隔符</param>
        /// <returns></returns>
        public static string ToArrayToString<T>(this T val1, string chat = ",")
        {
            return string.Join(chat, val1);
        }

        /// <summary>
        /// 将byte[]转换成16进制字符串表示形式
        /// </summary>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static string ToHexString(this byte[] value)
        {
            var sb = new StringBuilder();
            foreach (var b in value)
            {
                sb.AppendFormat(" {0}", b.ToString("X2").PadLeft(2, '0'));
            }
            return sb.Length > 0 ? sb.ToString().Substring(1) : sb.ToString();
        }

        /// <summary>
        /// 将byte[]转换成Base64字符串
        /// </summary>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static string ToByteToBase64String(this byte[] value)
        {
            return Convert.ToBase64String(value);
        }

        /// <summary>
        /// 将byte[]转换成内存流
        /// </summary>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static MemoryStream ToMemoryStream(this byte[] value)
        {
            return new MemoryStream(value);
        }

        /// <summary>
        /// 字符串验证
        /// </summary>
        /// <param name="value"></param>
        /// <param name="defaultval"></param>
        /// <returns></returns>
        public static string ToDefaultValue(this string value, string defaultval = "")
        {
            if (string.IsNullOrEmpty(value))
            {
                return defaultval;
            }
            return value;
        }

        /// <summary>
        /// 获取带前缀
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="prefix">前缀</param>
        /// <returns></returns>
        public static string ToDefaultPrefixValue(this string value, string prefix)
        {
            if (string.IsNullOrEmpty(value))
            {
                return "";
            }
            if (value.ToLower().IndexOf(prefix) > -1)
            {
                return value;
            }
            return prefix + value;
        }

        /// <summary>
        /// 替换前缀图片
        /// </summary>
        /// <param name="urls"></param>
        /// <returns></returns>
        public static string ToSetReplacePrefixValue(this string value, string prefix)
        {
            if (string.IsNullOrEmpty(value))
            {
                return "";
            }
            if (value.ToLower().IndexOf(prefix) > -1)
            {
                return value.Replace(prefix, "");
            }
            return value;
        }

        /// <summary>
        /// 字符串验证
        /// </summary>
        /// <param name="value"></param>
        /// <param name="defaultval"></param>
        /// <returns></returns>
        public static string ToDefaultObjectValue(this object value, string outValue, string defaultval = "")
        {
            if (value == null)
            {
                return defaultval;
            }
            return outValue;
        }

        /// <summary>
        /// 整形验证
        /// </summary>
        /// <param name="value"></param>
        /// <param name="defaultval"></param>
        /// <returns></returns>
        public static int ToDefaultObjectIntValue(this object value, int outValue, int defaultval = 0)
        {
            if (value == null)
            {
                return defaultval;
            }
            return outValue;
        }

        /// <summary>
        /// 数值验证
        /// </summary>
        /// <param name="value"></param>
        /// <param name="defaultval"></param>
        /// <returns></returns>
        public static decimal ToDefaultObjectDecimalValue(this object value, decimal outValue, decimal defaultval = 0)
        {
            if (value == null)
            {
                return defaultval;
            }
            return outValue;
        }

        #endregion

        #region 操作

        private static Snowflake Snowflake = new Snowflake();

        /// <summary>
        /// 获取身份证信息
        /// </summary>
        /// <param name="card"></param>
        /// <returns></returns>
        public static IDCardHelper ToIdCard(this string card)
        {
            return IDCardHelper.TryParse(card);
        }

        /// <summary>
        /// 去除html标签
        /// </summary>
        /// <param name="Htmlstring">字符串</param>
        /// <param name="Length">长度，默认：int.MaxValue</param>
        /// <returns></returns>
        public static string ToNoHTML(this string Htmlstring, int Length = int.MaxValue)
        {
            Htmlstring = Regex.Replace(Htmlstring, @"<script[\s\S]*?</script>", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"<noscript[\s\S]*?</noscript>", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"<style[\s\S]*?</style>", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"<.*?>", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"<(.[^>]*)>", " ", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"([\r\n])[\s]+", " ", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"-->", " ", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"<!--.*", " ", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(quot|#34);", "\"", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(amp|#38);", "&", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(lt|#60);", "<", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(gt|#62);", ">", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(nbsp|#160);", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(iexcl|#161);", "\xa1", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(cent|#162);", "\xa2", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(pound|#163);", "\xa3", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(copy|#169);", "\xa9", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&#(\d+);", " ", RegexOptions.IgnoreCase);
            if (Htmlstring.Length > Length)
            {
                return Htmlstring.Substring(0, Length);
            }
            return Htmlstring;
        }

        /// <summary>
        /// 生成验证码
        /// </summary>
        /// <param name="length">指定验证码的长度</param>
        /// <returns>验证码字符串</returns>
        public static string ToCreateValidateCode(this int length)
        {
            string ch = "abcdefghjkmnpqrstuvwxyzABCDEFGHJKMNPQRSTUVWXYZ1234567890";
            byte[] b = new byte[4];
            new RNGCryptoServiceProvider().GetBytes(b);
            var r = new Random(BitConverter.ToInt32(b, 0));
            var sb = new StringBuilder();
            for (int i = 0; i < length; i++)
            {
                sb.Append(ch[r.Next(ch.Length)]);
            }
            return sb.ToString();
        }

        /// <summary>
        /// 生成唯一识别号
        /// </summary>
        /// <param name="start"></param>
        /// <returns></returns>
        public static long ToUniqueNo(this int start)
        {
            string value = DateTime.Now.ToString("yyMMddHHmmss") + (DateTime.Now.ToString("FFF").PadLeft(3, '0'));
            Random random = new Random();
            if (start <= 0)
            {
                value += random.Next(1000, 9999).ToString();
                return Convert.ToInt64(value) + Math.Abs(Guid.NewGuid().GetHashCode());
            }
            else if (start < 10)
            {
                value = start.ToString() + value;
                value += random.Next(100, 999).ToString();
                return Convert.ToInt64(value) + Math.Abs(Guid.NewGuid().GetHashCode());
            }
            else if (start < 100 && start >= 10)
            {
                value = start.ToString() + value;
                value += random.Next(10, 99).ToString();
                return Convert.ToInt64(value) + Math.Abs(Guid.NewGuid().GetHashCode());
            }
            return DateTime.Now.Ticks;
        }

        /// <summary>
        /// 雪花算法，分布式唯一ID
        /// </summary>
        /// <returns></returns>
        public static long SnowflakeId()
        {
            return Snowflake.GetId();
        }

        /// <summary>
        /// 汉字转换成全拼的拼音  https://github.com/toolgood/ToolGood.Words
        /// </summary>
        /// <param name="Chstr"></param>
        /// <returns></returns>
        public static string ToPingYin(this string Chstr)
        {
            string r = string.Empty;
            foreach (char obj in Chstr)
            {
                try
                {
                    r += WordsHelper.GetAllPinyin(obj);
                }
                catch
                {
                    r += obj.ToString();
                }
            }
            return r;
        }

        /// <summary>
        /// 汉字转换成首字母
        /// </summary>
        /// <param name="Chstr"></param>
        /// <returns></returns>
        public static string ToFirstPinyin(this string Chstr)
        {
            return WordsHelper.GetFirstPinyin(Chstr);
        }

        /// <summary>
        /// 汉字转换拼音带音调
        /// </summary>
        /// <param name="Chstr"></param>
        /// <returns></returns>
        public static string ToPinyinForName(this string Chstr)
        {
            return WordsHelper.GetPinyinForName(Chstr, true);
        }

        #endregion

    }
}
