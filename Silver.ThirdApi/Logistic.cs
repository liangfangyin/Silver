using System;
using System.IO;
using System.Net;
using System.Text;

namespace Silver.ThirdApi
{
    /// <summary>
    /// 快递基本业务
    /// </summary>
    public class Logistic
    {

        /// <summary>
        /// 快递信息查询(原始接口)
        /// </summary> 
        /// <param name="companyPy">快递公司（拼音）</param>
        /// <param name="orderNo">物流单据号</param>
        /// <returns></returns>
        public static string LogisticInfo(string companyPy, string orderNo)
        {
            try
            {
                string Urls = "http://www.kuaidi100.com/query?type=" + companyPy + "&postid=" + orderNo;
                WebRequest request = WebRequest.Create(Urls);
                WebResponse response = request.GetResponse();
                Stream stream = response.GetResponseStream();
                Encoding encode = Encoding.UTF8;
                StreamReader reader = new StreamReader(stream, encode);
                string detail = reader.ReadToEnd();
                return detail;
            }
            catch (Exception ex)
            {
                throw new Exception("Logistic.LogisticInfo" + ex.Message);
            }
        }


        /// <summary>
        /// 快递所属公司查询(原始接口)
        /// </summary>  
        /// <param name="orderNo">物流单据号</param>
        /// <returns></returns>
        public static string Logisticauto(string orderNo)
        {
            try
            {
                string Urls = "http://www.kuaidi100.com/autonumber/autoComNum?resultv2=1&text=" + orderNo;
                WebRequest request = WebRequest.Create(Urls);
                WebResponse response = request.GetResponse();
                Stream stream = response.GetResponseStream();
                Encoding encode = Encoding.UTF8;
                StreamReader reader = new StreamReader(stream, encode);
                string detail = reader.ReadToEnd();
                return detail;
            }
            catch (Exception ex)
            {
                throw new Exception("Logistic.Logisticauto" + ex.Message);
            }
        }

        /// <summary>
        /// 指定区域快递公司
        /// </summary>
        /// <param name="area"></param>
        /// <param name="keyword"></param>
        /// <param name="companyNo"></param>
        /// <returns></returns>
        public static string NearbyLogisticauto(string area, string keyword, string companyNo)
        {
            try
            {
                string Urls = "https://www.kuaidi100.com/network/www/searchapi.do?method=searchnetwork&area=" + area + "&company=" + companyNo + "&keyword=" + keyword + "&offset=0&size=80&from=&channel=2&auditStatus=0";
                WebRequest request = WebRequest.Create(Urls);
                WebResponse response = request.GetResponse();
                Stream stream = response.GetResponseStream();
                Encoding encode = Encoding.UTF8;
                StreamReader reader = new StreamReader(stream, encode);
                string detail = reader.ReadToEnd();
                return detail;
            }
            catch (Exception ex)
            {
                throw new Exception("Logistic.NearbyLogisticauto" + ex.Message);
            }
        }


    }
}
