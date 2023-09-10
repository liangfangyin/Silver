using Silver.Basic;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Silver.Elasticsearch.Core
{
    public class QueryParam
    {
        public string query { get; set; }
    }

    public class HttpHelper
    {
        private static HttpClient _httpClient = new HttpClient();
        public static string Post(QueryParam param, string url)
        {
            HttpContent content = new StringContent(param.ToJson());
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            string result = _httpClient.PostAsync(url, content).Result.Content.ReadAsStringAsync().Result;
            content.Dispose();
            return result;
        }
    }
}
