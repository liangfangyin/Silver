using Nest;
using Silver.Elasticsearch.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Silver.Elasticsearch
{
    public class ElasticsearchUtil
    {

        //创建连接client
        private static ElasticClient client;
        private static string url = ElasticSearchOptions.Url;

        public ElasticsearchUtil()
        {
            client = new ElasticClient(new ConnectionSettings(new Uri(ElasticSearchOptions.Url)).BasicAuthentication(ElasticSearchOptions.UserName, ElasticSearchOptions.PassWord));
        }

        public ElasticsearchUtil(string urls, string userName, string passWord)
        {
            url = urls;
            client = new ElasticClient(new ConnectionSettings(new Uri(urls)).BasicAuthentication(userName, passWord));
        }

        #region 基础

        /// <summary>
        /// 客户端
        /// </summary>
        /// <returns></returns>
        public ElasticClient Client()
        {
            return client;
        }

        /// <summary>
        /// 执行
        /// </summary>
        /// <param name="sql"></param>
        public void ExecuteSql(string sql)
        {
            QueryParam queryParam = new QueryParam();
            queryParam.query = sql;
            string urls = url + "/_xpack/sql?format=csv";
            var result = HttpHelper.Post(queryParam, urls);
            Console.WriteLine(result);
        }

        #endregion

        #region 索引

        /// <summary>
        /// 创建索引
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="indexName"></param>
        public void CreateIndex<T>(string indexName) where T : class
        {
            client.Indices.Create(indexName, c => c.Map<T>(m => m.AutoMap()));
        }

        /// <summary>
        /// 删除索引
        /// </summary>
        /// <param name="indexName"></param>
        public void DeleteIndex(string indexName)
        {
            client.Indices.Delete(indexName);
        }

        #endregion

        #region 插入

        /// <summary>
        /// 插入
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name">数据集名称</param>
        /// <param name="value">数据对象</param>
        /// <returns></returns>
        public bool Insert<T>(string name, T value) where T : class
        {
            return client.Index(value, t => t.Index(name)).IsValid;
        }

        /// <summary>
        /// 插入
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name">数据集名称</param>
        /// <param name="value">数据对象</param>
        /// <returns></returns>
        public async Task<bool> InsertAsync<T>(string name, T value) where T : class
        {
            var result = await client.IndexAsync(value, t => t.Index(name));
            return result.IsValid;
        }

        /// <summary>
        /// 批量插入
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name">数据集名称</param>
        /// <param name="value">数据对象</param>
        /// <returns></returns>
        public bool InsertMany<T>(string name, List<T> value) where T : class
        {
            return client.Bulk(t => t.Index(name).IndexMany(value)).IsValid;
        }

        /// <summary>
        /// 批量插入
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name">数据集名称</param>
        /// <param name="value">数据对象</param>
        /// <returns></returns>
        public async Task<bool> InsertManyAsync<T>(string name, List<T> value) where T : class
        {
            var result = await client.BulkAsync(t => t.Index(name).IndexMany(value));
            return result.IsValid;
        }

        #endregion

        #region 查询

        /// <summary>
        /// 根据Id查询单个实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name">数据集名称</param>
        /// <param name="id">Id</param>
        /// <returns></returns>
        public T GetById<T>(string name, string id) where T : class
        {
            var result = client.Get<T>(id, t => t.Index(name));
            if (!result.IsValid)
            {
                return default(T);
            }
            return result.Source;
        }

        /// <summary>
        /// 根据Id查询单个实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<T> GetByIdAsync<T>(string name, string id) where T : class
        {
            var result = await client.GetAsync<T>(id, t => t.Index(name));
            if (!result.IsValid)
            {
                return default(T);
            }
            return result.Source;
        }

        /// <summary>
        /// 查询，默认10条,支持分页,首页为0
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name">数据集名称</param>
        /// <param name="query">查询条件</param>
        /// <returns></returns>
        public List<T> Query<T>(SearchRequest query = null) where T : class
        {
            if (query == null)
            {
                query = new SearchRequest()
                {
                    Query = new MatchAllQuery()
                };
            }
            var searchResponse = client.Search<T>(query);
            List<T> listDocument = searchResponse.Documents.ToList();
            return listDocument;
        }

        /// <summary>
        /// 查询，默认10条
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name">数据集名称</param>
        /// <param name="query">查询条件</param>
        /// <returns></returns>
        public List<T> Query<T>(string name, QueryContainer query = null) where T : class
        {
            if (query == null)
            {
                query = new MatchAllQuery();
            }
            var searchResponse = client.Search<T>(t => t.Index(name).Query(q => query));
            List<T> listDocument = searchResponse.Documents.ToList();
            return listDocument;
        }

        /// <summary>
        /// 分页查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name">数据集名称</param>
        /// <param name="query">查询条件</param>
        /// <returns></returns>
        public (List<T>, int) QueryPagerTotal<T>(string name, int pageIndex = 1, int pageSize = 15, QueryContainer query = null) where T : class
        {
            if (query == null)
            {
                query = new MatchAllQuery();
            }
            var searchResponse = client.Search<T>(t => t.Index(name).Query(q => query).From((pageIndex - 1) * pageSize).Size(pageSize));
            var searchTotal = client.Search<T>(t => t.Index(name).Query(q => query).From(0).Size(10000));
            List<T> listDocument = searchResponse.Documents.ToList();
            int total = searchTotal.Documents.Count;
            return (listDocument, total);
        }


        #endregion

        #region 删除

        /// <summary>
        /// 删除
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name">数据集名称</param>
        /// <param name="id">Id</param>
        /// <returns></returns>
        public bool Delete<T>(string name, string id) where T : class
        {
            return client.Delete<T>(id, x => x.Index(name)).IsValid;
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name">数据集名称</param>
        /// <param name="id">Id</param>
        /// <returns></returns>
        public async Task<bool> DeleteAsync<T>(string name, string id) where T : class
        {
            return (await client.DeleteAsync<T>(id, x => x.Index(name))).IsValid;
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name">数据集名称</param>
        /// <param name="id">Id</param>
        /// <returns></returns>
        public bool Delete<T>(string name, T value) where T : class
        {
            return client.Delete<T>(value, x => x.Index(name)).IsValid;
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name">数据集名称</param>
        /// <param name="id">Id</param>
        /// <returns></returns>
        public async Task<bool> DeleteAsync<T>(string name, T value) where T : class
        {
            return (await client.DeleteAsync<T>(value, x => x.Index(name))).IsValid;
        }

        /// <summary>
        /// 批量删除
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name">数据集名称</param>
        /// <param name="id">Id</param>
        /// <returns></returns>
        public bool DeleteMany<T>(string name, List<string> value) where T : class
        {
            return client.Bulk(t => t.Index(name).DeleteMany(value)).IsValid;
        }

        /// <summary>
        /// 批量删除
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name">数据集名称</param>
        /// <param name="id">Id</param>
        /// <returns></returns>
        public async Task<bool> DeleteManyAsync<T>(string name, List<string> value) where T : class
        {
            return (await client.BulkAsync(t => t.Index(name).DeleteMany(value))).IsValid;
        }

        /// <summary>
        /// 批量删除
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name">数据集名称</param>
        /// <param name="id">Id</param>
        /// <returns></returns>
        public bool DeleteMany<T>(string name, List<T> value) where T : class
        {
            return client.Bulk(t => t.Index(name).DeleteMany(value)).IsValid;
        }

        /// <summary>
        /// 批量删除
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name">数据集名称</param>
        /// <param name="id">Id</param>
        /// <returns></returns>
        public async Task<bool> DeleteManyAsync<T>(string name, List<T> value) where T : class
        {
            return (await client.BulkAsync(t => t.Index(name).DeleteMany(value))).IsValid;
        }

        #endregion

        #region 更新

        /// <summary>
        /// 更新
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name">数据集名称</param>
        /// <param name="id">Id</param>
        /// <param name="value">新内容</param>
        /// <returns></returns>
        public bool Update<T>(string name, string id, T value) where T : class
        {
            return client.Update<T>(id, x => x.Index(name).Doc(value)).IsValid;
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name">数据集名称</param>
        /// <param name="id">Id</param>
        /// <param name="value">新内容</param>
        /// <returns></returns>
        public async Task<bool> UpdateAsync<T>(string name, string id, T value) where T : class
        {
            return (await client.UpdateAsync<T>(id, x => x.Index(name).Doc(value))).IsValid;
        }

        #endregion


    }
}
