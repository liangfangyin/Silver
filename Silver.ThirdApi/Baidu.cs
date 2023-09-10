using Silver.Basic;
using Silver.ThirdApi.Model;
using System.Net;
using System.Text;

namespace Silver.ThirdApi
{
    public class Baidu
    {

        /// <summary>
        /// 文字转语音,800字以内
        /// </summary>
        /// <param name="value"></param>
        /// <returns>返回语音音频地址</returns>
        public static (bool, string) TextToSpeendAsync(string value,int speend=5,string langdet="zh")
        {
            if (langdet != "zh")
            { 
                string lang_result = PostData("https://fanyi.baidu.com/langdetect", "query="+ value);
                if (string.IsNullOrEmpty(lang_result))
                {
                    return (false, "获取语言失败");
                }
                var langdetects = lang_result.JsonToObject<langdetect>();
                if (langdetects.msg != "success")
                {
                    return (false, "获取语言失败");
                }
                string url = $"https://fanyi.baidu.com/gettts?lan={langdetects.lan}&text={value}&spd={speend}&source=web";
                return (true, url);
            }
            else
            {
                string url = $"https://fanyi.baidu.com/gettts?lan={langdet}&text={value}&spd={speend}&source=web";
                return (true, url);
            }
        }

        public static string PostData(string url, string postData)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(postData); 
            using (var client = new WebClient())
            {
                client.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
                client.Headers.Add("ContentLength", postData.Length.ToString());
                byte[] responseData = client.UploadData(url, "POST", bytes);
                return Encoding.UTF8.GetString(responseData);// 解码  
            }
        }

        public static string GetData(string url)
        {
            string data;
            using (var client = new WebClient())
            {
                using (var stream = client.OpenRead(url))
                {
                    using (var reader = new System.IO.StreamReader(stream))
                        data = reader.ReadToEnd();
                }
            }
            return data;
        }


    }
}
