using Silver.Basic;
using Silver.SqlSugar.Model;
using SqlSugar;
using System;
using System.Collections.Generic;

namespace Silver.SqlSugar
{
    public class SqlSugarCapUtil : IDisposable
    {
        private SqlSugarClient db;
        private System.Data.IDbConnection dbConnection; 

        /// <summary>
        /// 数据库连接
        /// </summary>
        /// <returns></returns>
        private SqlSugarClient GetInstance()
        {
            var listDataRead = ConfigurationUtil.GetSection("SqlSugar:DataRead").JsonToObject<List<DataRead>>();
            List<SlaveConnectionConfig> listConnectionConfig = new List<SlaveConnectionConfig>();
            if (listDataRead.Count > 0)
            {
                foreach (var item in listDataRead)
                {
                    SlaveConnectionConfig infoConnectionConfig = new SlaveConnectionConfig();
                    infoConnectionConfig.ConnectionString = item.Connection;
                    infoConnectionConfig.HitRate = item.HitRate;
                    listConnectionConfig.Add(infoConnectionConfig);
                }
            }
            db = new SqlSugarClient(new ConnectionConfig()
            {
                ConnectionString = ConfigurationUtil.GetSection("SqlSugar:Connection"),
                DbType = DbType.MySqlConnector,
                IsAutoCloseConnection = true,
                InitKeyType = InitKeyType.Attribute,
                SlaveConnectionConfigs = listConnectionConfig,
            });
            db.Aop.OnLogExecuting = (sql, pars) =>
            {

            };
            db.Aop.OnLogExecuted = (sql, pars) =>
            {

            };
            db.Aop.OnError = (pars) =>
            {
                Console.WriteLine(pars.ToString());
            };
            return db;
        }
           
        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            if (dbConnection != null)
            {
                dbConnection.Dispose();
                dbConnection.Close();
            } 
            if (db != null)
            {
                db.Dispose();
                db.Close();
            } 
        }
    
    }
}
