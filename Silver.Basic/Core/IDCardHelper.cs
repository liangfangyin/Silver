using System;
using System.Collections.Generic;
using System.Text;

namespace Silver.Basic.Core
{
    /// <summary>
    /// 身份证信息获取
    /// </summary>
    public class IDCardHelper
    {

        /// <summary>
        /// 尝试解析身份证号
        /// </summary>
        /// <param name="cardno"></param>
        /// <param name="idCard"></param>
        /// <returns></returns>
        public static IDCardHelper TryParse(string cardno)
        { 
            if (!StringUtil.IsCardID(cardno))
            {
                return null;
            }
            var inst = new IDCardHelper();
            inst.CardNo = cardno;
            inst.parseSex();
            inst.parseAge();
            return inst;
        }

        /// <summary>
        /// 身份证号
        /// </summary>
        public string CardNo { get; internal set; }

        /// <summary>
        /// 行政区划(标准县级: 6位)
        /// </summary>
        public string RegionCode => CardNo.Substring(0, 6);

        /// <summary>
        /// 性别
        /// </summary>
        public int Sex { get; internal set; }

        /// <summary>
        /// 年龄
        /// </summary>
        public int Age { get; internal set; }

        /// <summary>
        /// 生日
        /// </summary>
        public DateTime BirthDay { get; internal set; }

        /// <summary>
        /// 获取性别
        /// </summary>
        /// <returns> 1:男 2:女</returns>
        private void parseSex()
        {
            string tmp;
            tmp = this.CardNo.Substring(this.CardNo.Length - 4);
            tmp = tmp.Substring(0, 3);
            Math.DivRem(Convert.ToInt32(tmp), 2, out int outNum);
            Sex = outNum;
        }

        /// <summary>
        /// 获取年龄
        /// </summary>
        /// <returns>年龄</returns>
        private void parseAge()
        {
            parseBrithday();
            DateTime nowDateTime = DateTime.Now;
            int age = nowDateTime.Year - this.BirthDay.Year;
            // 再考虑月、天的因素
            if (nowDateTime.Month < this.BirthDay.Month || (nowDateTime.Month == this.BirthDay.Month && nowDateTime.Day < this.BirthDay.Day))
            {
                age--;
            }
            this.Age = age;
        }

        /// <summary>
        /// 解析生日
        /// </summary>
        /// <returns></returns>
        private void parseBrithday()
        {
            var rtn = this.CardNo.Substring(6, 8).Insert(6, "-").Insert(4, "-");
            var birth = DateTime.Parse(rtn);
            this.BirthDay = birth;
        }

    }
}
