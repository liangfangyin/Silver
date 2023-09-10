using Silver.Basic.Core;
using Silver.Basic.Model.Enums;
using System;
using System.Diagnostics;
using System.Globalization;
using static Silver.Basic.Core.CalendarHelper;

namespace Silver.Basic
{
    /// <summary>
    /// 日期帮助类
    /// </summary>
    public static class DateUtil
    {
        #region 格式化

        /// <summary>
        /// 日期转换字符串
        /// </summary>
        /// <param name="dt">日期</param>
        /// <param name="pattern">显示格式 yyyy-MM-dd HH:mm:ss</param>
        /// <returns>时间格式化 如2023-09-09 05:00:11</returns>
        public static string FormatDate(this DateTime dt, string pattern = DatePattern.YEAR_MONTH_DAY_HOUR_MINUTE_SECOND)
        {
            return dt.ToString(pattern);
        }

        /// <summary>
        /// 日期转数字
        /// </summary>
        /// <param name="dt">日期</param>
        /// <param name="pattern">显示格式 yyyyMMdd</param>
        /// <returns></returns>
        public static int FormatDateInt(this DateTime dt, string pattern = DatePatternNumber.NUM_YEAR_MONTH_DAY)
        {
            return dt.ToString(pattern).ToInt();
        }

        /// <summary>
        /// 日期转数字
        /// </summary>
        /// <param name="dt">日期</param>
        /// <param name="pattern">显示格式 yyyyMMddHHmmss</param>
        /// <returns></returns>
        public static long FormatDateLong(this DateTime dt, string pattern = DatePatternNumber.NUM_YEAR_MONTH_DAY)
        {
            return dt.ToString(pattern).ToLong();
        }

        /// <summary>
        /// 数字日期转字符串日期
        /// </summary>
        /// <param name="date">日期 20230909</param>
        /// <returns>返回 2023-09-09</returns>
        public static string ToIntDateToString(this int date)
        {
            string dateString = date.ToString();
            return dateString.Substring(0, 4) + "-" + dateString.Substring(4, 2) + "-" + dateString.Substring(6, 2);
        }

        /// <summary>
        /// 数字日期转字日期
        /// </summary>
        /// <param name="date">日期 20230909</param>
        /// <returns>返回 日期</returns>
        public static DateTime ToIntDateToDate(this int date)
        {
            return ToIntDateToString(date).ToDateTime();
        }

        /// <summary>
        /// 日期转星期(中文)
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static string ToWeek(this DateTime dt)
        {
            if (dt == null)
            {
                dt = DateTime.Now;
            }
            switch (dt.DayOfWeek)
            {
                case DayOfWeek.Sunday:
                    return "周日";
                case DayOfWeek.Monday:
                    return "周一";
                case DayOfWeek.Tuesday:
                    return "周二";
                case DayOfWeek.Wednesday:
                    return "周三";
                case DayOfWeek.Thursday:
                    return "周四";
                case DayOfWeek.Friday:
                    return "周五";
                case DayOfWeek.Saturday:
                    return "周六";
                default:
                    break;
            }
            return "";
        }

        /// <summary>
        /// 获取13位时间戳 
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static long ToUnixTimeMilliseconds(this DateTime dateTime)
        {
            return new DateTimeOffset(dateTime).ToUnixTimeMilliseconds();
        }

        /// <summary>
        /// 获取10位时间戳 
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static long ToUnixTimeSeconds(this DateTime dateTime)
        {
            return new DateTimeOffset(dateTime).ToUnixTimeSeconds();
        }

        /// <summary>
        /// 昨天
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static DateTime ToYesterday(this DateTime dateTime)
        {
            return dateTime.AddDays(-1);
        }

        /// <summary>
        /// 明天
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static DateTime ToTomorrow(this DateTime dateTime)
        {
            return dateTime.AddDays(1);
        }

        /// <summary>
        /// 上一周
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns>（星期一，星期日）</returns>
        public static (DateTime Monday, DateTime Sunday) ToLastWeek(this DateTime dateTime)
        {
            int daysToMonday = (int)dateTime.DayOfWeek - 1; // Sunday = 0, Monday = 1, ..., Saturday = 6  
            DateTime mondayOfPreviousWeek = dateTime.AddDays(-daysToMonday - 7); 
            DateTime sundayOfPreviousWeek = mondayOfPreviousWeek.AddDays(6);
            return (mondayOfPreviousWeek, sundayOfPreviousWeek);
        }

        /// <summary>
        /// 下一周
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static (DateTime Monday, DateTime Sunday) ToNextWeek(DateTime date)
        {
            int daysToMonday = (int)date.DayOfWeek - 1; // Sunday = 0, Monday = 1, ..., Saturday = 6  
            DateTime mondayOfNextWeek = date.AddDays(7 - daysToMonday); 
            DateTime sundayOfNextWeek = mondayOfNextWeek.AddDays(6); 
            return (mondayOfNextWeek, sundayOfNextWeek);
        }
        
        /// <summary>
        /// 指定日期一天得开始和结束时间
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static (DateTime BeginDate,DateTime EndDate) ToDayFirstAndLast(this DateTime date) 
        {
            DateTime firstDate = date.FormatDate(DatePattern.YEAR_MONTH_DAY).ToDateTime();
            DateTime lastDate = firstDate.AddDays(1).AddMilliseconds(-1);
            return (firstDate,lastDate);
        }

        /// <summary>
        /// 星座
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static string ToZodiac(this DateTime date)
        {
            int month = date.Month;
            int day = date.Day;
            if ((month == 1 && day >= 20) || (month == 2 && day <= 18))
            {
                return "水瓶座";
            }
            else if ((month == 2 && day >= 19) || (month == 3 && day <= 20))
            {
                return "双鱼座";
            }
            else if ((month == 3 && day >= 21) || (month == 4 && day <= 19))
            {
                return "白羊座";
            }
            else if ((month == 4 && day >= 20) || (month == 5 && day <= 20))
            {
                return "金牛座";
            }
            else if ((month == 5 && day >= 21) || (month == 6 && day <= 21))
            {
                return "双子座";
            }
            else if ((month == 6 && day >= 22) || (month == 7 && day <= 22))
            {
                return "巨蟹座";
            }
            else if ((month == 7 && day >= 23) || (month == 8 && day <= 22))
            {
                return "狮子座";
            }
            else if ((month == 8 && day >= 23) || (month == 9 && day <= 22))
            {
                return "处女座";
            }
            else if ((month == 9 && day >= 23) || (month == 10 && day <= 23))
            {
                return "天秤座";
            }
            else if ((month == 10 && day >= 24) || (month == 11 && day <= 22))
            {
                return "天蝎座";
            }
            else if ((month == 11 && day >= 23) || (month == 12 && day <= 21))
            {
                return "射手座";
            }
            else
            {
                return "摩羯座";
            }
        }

        /// <summary>
        /// 生肖
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static string ToChineseZodiac(this DateTime date)
        {
            int year = date.Year;
            int zodiacIndex = (year - 4) % 12;  
            string[] zodiacs = new string[] { "鼠", "牛", "虎", "兔", "龙", "蛇", "马", "羊", "猴", "鸡", "狗", "猪" };
            return zodiacs[zodiacIndex];
        }

        /// <summary>
        /// 农历日期
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static StructDateFullInfo ToChineseDate(this DateTime dt)
        {
            CalendarHelper calendar = new CalendarHelper();
            return calendar.GetDateTidyInfo(dt);
        }

        #endregion

        #region 应用

        /// <summary>
        /// 指定日期月份第一天
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static DateTime ToDateFirst(this DateTime dt)
        {
            return (dt.FormatDate(DatePattern.YEAR_MONTH) + "-01").ToDateTime();
        }

        /// <summary>
        /// 指定日期月份最后一天
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static DateTime ToDateLast(this DateTime dt)
        {
            return (dt.FormatDate(DatePattern.YEAR_MONTH) + "-01").ToDateTime().AddDays(1).AddMilliseconds(-1);
        }

        private static object lockorder = new object();
        private static long orderCurrent = 0;
        private static long orderIndentity = 0;
        /// <summary>
        /// 根据时间生成订单号(唯一)
        /// </summary>
        /// <param name="prefix">前缀，无前缀可以转长整型</param>
        /// <returns></returns>
        public static string ToOrderNo(string prefix = "")
        {
            lock (lockorder)
            {
                Random rdom = new Random();
                long timer = DateTime.Now.FormatDateLong(DatePatternNumber.NUM_YEAR_MONTH_DAY_HOUR_MINUTE_SECOND);
                if (timer == orderCurrent)
                {
                    orderIndentity++;
                    return $"{prefix}{timer}{orderIndentity.ToString().PadLeft(4, '0')}{rdom.Next(0, 999).ToString().PadLeft(3, '0')}";
                }
                orderCurrent = timer;
                orderIndentity = 1;
                return $"{prefix}{timer}{orderIndentity.ToString().PadLeft(4, '0')}{rdom.Next(0, 999).ToString().PadLeft(3, '0')}";
            }
        }

        /// <summary>
        /// 时间块： 10天前   12小时前   5分前   刚刚
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static string ToTimeFormatDayPiece(this DateTime dt)
        {
            DateTime edate = DateTime.Now;
            if (dt == null)
            {
                return "刚刚";
            }
            if ((edate - dt).TotalDays >= 365)
            {
                return Convert.ToInt32((edate - dt).TotalDays / 365) + "年";
            }
            else if ((edate - dt).TotalDays >= 1)
            {
                return Convert.ToInt32((edate - dt).TotalDays) + "天";
            }
            else if ((edate - dt).TotalHours >= 1)
            {
                return Convert.ToInt32((edate - dt).TotalHours) + "小时";
            }
            else if ((edate - dt).TotalMinutes >= 1)
            {
                return Convert.ToInt32((edate - dt).TotalMinutes) + "分钟";
            }
            else if ((edate - dt).TotalSeconds >= 1)
            {
                return Convert.ToInt32((edate - dt).TotalMinutes) + "秒";
            }
            else if ((edate - dt).TotalMilliseconds >= 1)
            {
                return Convert.ToInt32((edate - dt).TotalMinutes) + "毫秒";
            }
            else
            {
                return "刚刚";
            }
        }

        /// <summary>
        /// 时间块：今天  明天  后天   昨天  前天
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static string ToFriendlyDate(this DateTime? date)
        {
            if (!date.HasValue) return string.Empty;

            string strDate = date.Value.ToString("yyyy-MM-dd");
            string vDate = string.Empty;
            if (DateTime.Now.ToString("yyyy-MM-dd") == strDate)
            {
                vDate = "今天";
            }
            else if (DateTime.Now.AddDays(1).ToString("yyyy-MM-dd") == strDate)
            {
                vDate = "明天";
            }
            else if (DateTime.Now.AddDays(2).ToString("yyyy-MM-dd") == strDate)
            {
                vDate = "后天";
            }
            else if (DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd") == strDate)
            {
                vDate = "昨天";
            }
            else if (DateTime.Now.AddDays(-2).ToString("yyyy-MM-dd") == strDate)
            {
                vDate = "前天";
            }
            else
            {
                vDate = strDate;
            }
            return vDate;
        }

        /// <summary>
        /// 返回年度第几个星期   默认星期日是第一天
        /// </summary>
        /// <param name="date">时间</param>
        /// <returns>第几周</returns>
        public static int ToWeekOfYear(this in DateTime date)
        {
            var gc = new GregorianCalendar();
            return gc.GetWeekOfYear(date, CalendarWeekRule.FirstDay, DayOfWeek.Sunday);
        }

        /// <summary>
        /// 方法运行时间
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public static TimeSpan Run(Action action)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            action?.Invoke();
            stopwatch.Stop();
            return stopwatch.Elapsed;
        }

        #endregion

    }
   
}
