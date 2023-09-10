using System.Threading.Tasks;

namespace Silver.ThirdApi
{
    public class Weather
    {
        /// <summary>
        /// 查询城市温度 如  萧山
        /// </summary>
        /// <param name="city"></param>
        /// <returns></returns>
        public static string temperatureAsync(string city)
        { 
            return Baidu.GetData("http://wthrcdn.etouch.cn/weather_mini?city=" + city);
        }

    }
}
