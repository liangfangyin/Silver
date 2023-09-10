using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace Silver.NLog.Core
{
    public class SqlSugarExtension
    {

        public SqlSugarClient GetCustomInstance()
        {
            //创建数据库对象 (用法和EF Dappper一样通过new保证线程安全)
            SqlSugarClient Db = new SqlSugarClient(new ConnectionConfig()
            {
                ConnectionString = NLogConfig.ConnectionString,
                DbType = NLogConfig.DbBaseType,
                IsAutoCloseConnection = true
            });
            return Db;
        }

    }
}
