using Silver.Basic;
using Silver.Dapper.Model;
using SQLBuilder.Core.Enums;
using SQLBuilder.Core.Repositories;
using System;
using System.Collections.Generic;

namespace Silver.Dapper
{
    public class DapperUtil : IDisposable
    {

        private IRepository connection;

        /// <summary>
        /// 默认
        /// </summary>
        /// <returns></returns>
        public IRepository GetInstance()
        {
            DatabaseType dataType = GetDatabaseType();
            switch (dataType)
            {
                case DatabaseType.MySql:
                    connection = new MySqlRepository(ConfigurationUtil.GetSection("Dapper:Connection"));
                    break;
                case DatabaseType.SqlServer:
                    connection = new SqlRepository(ConfigurationUtil.GetSection("Dapper:Connection"));
                    break;
                case DatabaseType.Sqlite:
                    connection = new SqliteRepository(ConfigurationUtil.GetSection("Dapper:Connection"));
                    break;
                case DatabaseType.PostgreSql:
                    connection = new NpgsqlRepository(ConfigurationUtil.GetSection("Dapper:Connection"));
                    break;
                case DatabaseType.Oracle:
                    connection = new OracleRepository(ConfigurationUtil.GetSection("Dapper:Connection"));
                    break;
                default:
                    connection = new MySqlRepository(ConfigurationUtil.GetSection("Dapper:Connection"));
                    break;
            }
            var listDataRead= ConfigurationUtil.GetSection("Dapper:DataRead").JsonToObject<List<DataRead>>(); 
            //读写分离
            if (listDataRead.Count > 0)
            {
                connection.UseMasterOrSlave(false);
                connection.MasterConnectionString = ConfigurationUtil.GetSection("Dapper:Connection");
                (string, int)[] slave = new (string, int)[listDataRead.Count]; 
                for (int i = 0; i < listDataRead.Count; i++)
                {
                    slave[i] = (listDataRead[i].Connection, listDataRead[i].HitRate);
                }
                connection.SlaveConnectionStrings = slave;
                connection.LoadBalancer = new SQLBuilder.Core.LoadBalancer.RoundRobinLoadBalancer();
            }
            return connection;
        }

        /// <summary>
        /// 自定义数据库
        /// </summary>
        /// <param name="ConnectionString">数据库地址</param>
        /// <param name="dataType">操作数据库类型</param>
        /// <returns></returns>
        public IRepository GetCustomInstance(string ConnectionString, DatabaseType dataType)
        {
            switch (dataType)
            {
                case DatabaseType.MySql:
                    connection = new MySqlRepository(ConnectionString);
                    break;
                case DatabaseType.SqlServer:
                    connection = new SqlRepository(ConnectionString);
                    break;
                case DatabaseType.Sqlite:
                    connection = new SqliteRepository(ConnectionString);
                    break;
                case DatabaseType.PostgreSql:
                    connection = new NpgsqlRepository(ConnectionString);
                    break;
                case DatabaseType.Oracle:
                    connection = new OracleRepository(ConnectionString);
                    break;
                default:
                    connection = new MySqlRepository(ConnectionString);
                    break;
            }
            return connection;
        }


        /// <summary>
        /// 关闭服务
        /// </summary>
        public void Dispose()
        {
            if (connection != null)
            {
                connection.Dispose();
            }
        }

        /// <summary>
        /// 数据库类型
        /// </summary>
        /// <returns></returns>
        private DatabaseType GetDatabaseType()
        {
            DatabaseType dataType = DatabaseType.MySql;
            switch (ConfigurationUtil.GetSection("Dapper:DataType").ToInt())
            {
                case 0:
                    dataType = DatabaseType.MySql;
                    break;
                case 1:
                    dataType = DatabaseType.SqlServer;
                    break;
                case 2:
                    dataType = DatabaseType.Sqlite;
                    break;
                case 3:
                    dataType = DatabaseType.Oracle;
                    break;
                case 4:
                    dataType = DatabaseType.PostgreSql;
                    break;
                default:
                    dataType = DatabaseType.MySql;
                    break;
            }
            return dataType;
        }

    }
}
