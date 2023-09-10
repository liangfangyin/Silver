using Silver.Basic;

namespace Silver.Elasticsearch.Core
{
    public class ElasticSearchOptions
    {
        /// <summary>
        /// ElasticSearch链接
        /// </summary>
        public static string Url
        {
            get
            {
                return ConfigurationUtil.GetSection("ElasticSearch:Url");
            }
        }

        /// <summary>
        /// 用户名
        /// </summary>
        public static string UserName
        {
            get
            {
                return ConfigurationUtil.GetSection("ElasticSearch:UserName");
            }
        }

        /// <summary>
        /// 密码
        /// </summary>
        public static string PassWord
        {
            get
            {
                return ConfigurationUtil.GetSection("ElasticSearch:PassWord");
            }
        }

    }
}
