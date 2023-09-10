using Silver.Basic;
using Silver.ThirdApi.Model;
using System.Collections.Generic;
using System.Net;

namespace Silver.ThirdApi
{
    public class AmapMap
    {


        /// <summary>
        /// 高德地图-IP定位
        /// </summary>
        /// <param name="key"></param>
        /// <param name="ip"></param>
        /// <returns>
        /// status	  返回结果状态值	    值为0或1,0表示失败；1表示成功
        /// info      返回状态说明          返回状态说明，status为0时，info返回错误原因，否则返回“OK”。
        /// infocode  状态码                返回状态说明,1000代表正确,详情参阅info状态表
        /// province  省份名称              若为直辖市则显示直辖市名称； 如果在局域网 IP网段内，则返回“局域网”； 非法IP以及国外IP则返回空
        /// city      城市名称              若为直辖市则显示直辖市名称； 如果为局域网网段内IP或者非法IP或国外IP，则返回空
        /// adcode    城市的adcode编码
        /// rectangle 所在城市矩形区域范围  所在城市范围的左下右上对标对
        /// </returns>
        public static string IPAddress(string ip = "")
        {
            string url = "http://restapi.amap.com/v3/ip?key=" + ConfigurationUtil.GetSection("AmapMap:AppId") + "&ip=" + ip + "&output=JSON";
            return GetData(url);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="code">代码code 或是名称name</param>
        /// <returns>
        /// citycode	城市编码	
        ///adcode 区域编码 
        ///name 行政区名称 
        ///polyline 行政区边界坐标点    当出现一个区域出现多个模块的时候，会用"|"进行区分。例如：北京>朝阳
        ///center 城市中心点
        ///level 行政区划级别 country:国家
        ///province:省份（直辖市会在province和city显示）
        ///city:市（直辖市会在province和city显示）
        /// district:区县
        ///biz_area:商圈（强烈建议利用showbiz参数跳过）
        ///street:街道
        ///districts 下级行政区列表，包含district元素
        /// </returns>
        public static List<amap_districts> Address(string code)
        {
            string url = "http://restapi.amap.com/v3/config/district?key=" + ConfigurationUtil.GetSection("AmapMap:AppId") + "&keywords=" + code;
            return GetData(url).JsonToObject<List<amap_districts>>();
        }

        /// <summary>
        /// 步行路径规划
        /// </summary>
        /// <param name="origin">出发点：lon，lat（经度，纬度）， “,”分割</param>
        /// <param name="destination">目的地： lon，lat（经度，纬度）， “,”分割</param>
        /// <returns></returns>
        public static string Walking(string origin, string destination)
        {
            string url = "https://restapi.amap.com/v3/direction/walking?origin=" + origin + "&destination=" + destination + "&key=" + ConfigurationUtil.GetSection("AmapMap:AppId");
            return GetData(url);
        }

        /// <summary>
        /// 公交路线规划
        /// </summary>
        /// <param name="origin">出发点：lon，lat（经度，纬度）， “,”分割</param>
        /// <param name="destination">目的地： lon，lat（经度，纬度）， “,”分割</param>
        /// <param name="s_city">起始城市</param>
        /// <param name="e_city">目的城市</param>
        /// <param name="strategy">0：最快捷模式;1：最经济模式;2：最少换乘模式;3：最少步行模式;5：不乘地铁模式</param>
        /// <param name="nightflag">是否计算夜班车,1:是；0：否</param>
        /// <returns></returns>
        public static string Integrated(string origin, string destination, string s_city, string e_city, int strategy = 0, int nightflag = 1)
        {
            string url = "https://restapi.amap.com/v3/direction/transit/integrated?key=" + ConfigurationUtil.GetSection("AmapMap:AppId") + "&origin=" + origin + "&destination=" + destination + "&city=" + s_city + "&cityd=" + e_city + "&strategy=" + strategy + "&nightflag=" + nightflag;
            return GetData(url);
        }

        /// <summary>
        /// 驾车路径规划
        /// </summary>
        /// <param name="origin">出发点：lon，lat（经度，纬度）， “,”分割</param>
        /// <param name="destination">目的地： lon，lat（经度，纬度）， “,”分割</param>
        /// <returns></returns>
        public static string Driving(string origin, string destination)
        {
            string url = "https://restapi.amap.com/v3/direction/driving?key=" + ConfigurationUtil.GetSection("AmapMap:AppId") + "&origin=" + origin + "&destination=" + destination + "&originid=&destinationid=&extensions=all&strategy=0&waypoints=&avoidpolygons=&avoidroad=";
            return GetData(url);
        }

        /// <summary>
        /// IPO 关键词搜索
        /// </summary>
        /// <param name="name"></param>
        /// <param name="city"></param>
        /// <returns></returns>
        public static string Keyipo(string name, string city)
        {
            string url = "https://restapi.amap.com/v3/place/text?key=" + ConfigurationUtil.GetSection("AmapMap:AppId") + "&keywords=" + name + "&types=&city=" + city + "&children=&offset=30&page=1&extensions=all";
            return GetData(url);
        }

        /// <summary>
        /// IPO 周边搜索
        /// </summary>
        /// <param name="name">关键词</param>
        /// <param name="location">定位</param>
        /// <param name="typeid">搜索类型</param>
        /// <param name="radius">半径地址</param>
        /// <returns></returns>
        public static string Peripo(string name, string location, string typeid, int radius = 5000)
        {
            string url = "https://restapi.amap.com/v3/place/around?key=" + ConfigurationUtil.GetSection("AmapMap:AppId") + "&location=" + location + "&keywords=" + name + "&types=" + typeid + "&radius=1000&offset=30&page=1&extensions=all";
            return GetData(url);
        }

        /// <summary>
        /// 天气
        /// </summary>
        /// <param name="city">城市编码</param>
        /// <returns></returns>
        public static string Weather(string city)
        {
            string url = "https://restapi.amap.com/v3/weather/weatherInfo?key=" + ConfigurationUtil.GetSection("AmapMap:AppId") + "&city=" + city;
            return GetData(url);
        }

        /// <summary>
        /// 获取内容
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string GetData(string url)
        {
            string data = "";
            using (var client = new WebClient())
            {
                using (var stream = client.OpenRead(url))
                {
                    using (var reader = new System.IO.StreamReader(stream))
                        data = reader.ReadToEnd();
                }
            }
            if (string.IsNullOrEmpty(data))
            {
                return "{}";
            }
            return data;
        }


    }
}
