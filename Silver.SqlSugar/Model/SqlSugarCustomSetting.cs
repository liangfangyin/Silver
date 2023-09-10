using SqlSugar;
using System.Collections.Generic;

namespace Silver.SqlSugar.Model
{
    public class SqlSugarCustomSetting
    {

        public string dataSetting { get; set; } = "server=43.142.159.188;port=3306;database=ydthird;uid=liangfy;pwd=123456;Pooling=true;MinPoolSize=5;MaxPoolSize=254;Convert Zero Datetime=True;Allow User Variables=True;ConnectionLifetime=120;Connect Timeout=300;charset=utf8";

        public DbType dataType { get; set; } = DbType.MySql;

        public List<SqlSugarDataSetting> dataRead { get; set; } = new List<SqlSugarDataSetting>();
    }
}
