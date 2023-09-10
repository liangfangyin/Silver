using Silver.Basic;
using Silver.SqlSugar.Model;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.IO;

namespace Silver.SqlSugar
{
    public class SqlSugarUtil : IDisposable
    { 
        public static List<SqlSugarDataSetting> listSettingDate = new List<SqlSugarDataSetting>();
        public static object islock = new object();
        public SqlSugarClient db;
        public SqlSugarClient customDB;


        /// <summary>
        /// 数据库连接
        /// </summary>
        /// <returns></returns>
        public SqlSugarClient GetInstance()
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
                DbType = (DbType)(Convert.ToInt32(ConfigurationUtil.GetSection("SqlSugar:DbType"))),
                IsAutoCloseConnection = true,
                InitKeyType = InitKeyType.Attribute,
                SlaveConnectionConfigs = listConnectionConfig,
            });
            db.Aop.OnLogExecuting = (sql, pars) =>
            {
                //Console.WriteLine(sql + "\r\n" + db.Utilities.SerializeObject(pars.ToDictionary(it => it.ParameterName, it => it.Value)));
                //Console.WriteLine();
            };
            db.Aop.OnLogExecuted = (sql, pars) =>
            {

            };
            db.Aop.OnError = (pars) =>
            {

            };
            return db;
        }
         
        /// <summary>
        /// 多库切换(多租户)
        /// </summary>
        /// <returns></returns>
        public SqlSugarClient GetSubInstance()
        {
            var listConnection = new List<ConnectionConfig>();
            foreach (var item in listSettingDate)
            {
                var infoConnection = new ConnectionConfig();
                infoConnection.ConfigId = item.ConfigId;
                infoConnection.DbType = (DbType)item.DataType;
                infoConnection.ConnectionString = item.Data;
                infoConnection.InitKeyType = InitKeyType.Attribute;
                infoConnection.IsAutoCloseConnection = true;
                listConnection.Add(infoConnection);
            } 
            db = new SqlSugarClient(listConnection);
            db.Aop.OnLogExecuting = (sql, pars) =>
            {
                //Console.WriteLine(sql + "\r\n" + db.Utilities.SerializeObject(pars.ToDictionary(it => it.ParameterName, it => it.Value)));
                //Console.WriteLine();
            };
            db.Aop.OnLogExecuted = (sql, pars) =>
            {

            };
            db.Aop.OnError = (pars) =>
            {
                Writelogs(pars.ToString());
            }; 
            return db;
        }

        /// <summary>
        /// 自定义数据库
        /// </summary>
        /// <param name="ConnectionString">数据库地址</param>
        /// <param name="dataType">操作数据库类型</param>
        /// <returns></returns>
        public SqlSugarClient GetCustomInstance(SqlSugarCustomSetting info)
        {
            List<SlaveConnectionConfig> listConnectionConfig = new List<SlaveConnectionConfig>();
            if (info.dataSetting.Length > 0)
            {
                foreach (var item in info.dataRead)
                {
                    SlaveConnectionConfig infoConnectionConfig = new SlaveConnectionConfig();
                    infoConnectionConfig.ConnectionString = item.Data;
                    infoConnectionConfig.HitRate = item.HitRate;
                    listConnectionConfig.Add(infoConnectionConfig);
                }
            }
            customDB = new SqlSugarClient(new ConnectionConfig()
            {
                ConnectionString = info.dataSetting,
                DbType = info.dataType,
                IsAutoCloseConnection = true,
                InitKeyType = InitKeyType.Attribute,
                SlaveConnectionConfigs = listConnectionConfig,
            });
            customDB.Aop.OnLogExecuting = (sql, pars) =>
            {
                //Console.WriteLine(sql + "\r\n" + db.Utilities.SerializeObject(pars.ToDictionary(it => it.ParameterName, it => it.Value)));
                //Console.WriteLine();
            };
            customDB.Aop.OnLogExecuted = (sql, pars) =>
            {

            };
            customDB.Aop.OnError = (pars) =>
            {
                Writelogs(pars.ToString());
            };
            return customDB;
        }
         
        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            try
            {
                if (db != null)
                {
                    db.Dispose();
                    db.Close();
                }
                
            }
            catch { }
            try
            {
                if (customDB != null)
                {
                    customDB.Dispose();
                    customDB.Close();
                }
            }
            catch { }
        }
         
        /// <summary>
        /// 数据库错误日志记录
        /// </summary>
        /// <param name="message"></param>
        /// <param name="method"></param>
        private static void Writelogs(string message, string method = "SqlSugarExtension")
        {
            try
            {
                string path = Directory.GetCurrentDirectory() + "//logs//";
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                path += DateTime.Now.ToString("yyyy-MM-dd") + ".log";
                string content = $"时间：{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}  \r\n  方法：{method}  \r\n  内容：{message} \r\n\r\n ";
                lock (islock)
                {
                    File.AppendAllText(path, content);
                }
            }
            catch { }
        }

    }
}
