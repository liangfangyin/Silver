using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace Silver.NLog.Core.ORM
{
    /// <summary>
    /// 服务日志
    ///</summary>
    [SplitTable(SplitType.Month)]
    [SugarTable("logs_service_{year}{month}{day}")]
    public class LogsService
    {

        /// <summary>
        /// 
        ///</summary>
        [SugarColumn(ColumnName = "id", IsPrimaryKey = true)]
        public long Id { get; set; }

        /// <summary>
        /// 
        ///</summary>
        [SplitField]
        [SugarColumn(ColumnName = "create_date")]
        public DateTime CreateDate { get; set; } = DateTime.Now;

        /// <summary>
        /// 
        ///</summary>
        [SugarColumn(ColumnName = "create_ticker")]
        public long CreateTicker { get; set; } = DateTime.Now.Ticks;

        /// <summary>
        /// 方法名称
        ///</summary>
        [SugarColumn(ColumnName = "methods")]
        public string Methods { get; set; } = "";

        /// <summary>
        /// 用时
        ///</summary>
        [SugarColumn(ColumnName = "timer")]
        public int Timer { get; set; } = 0;

        /// <summary>
        /// 日志等级
        ///</summary>
        [SugarColumn(ColumnName = "code")]
        public string Code { get; set; } = "";

        /// <summary>
        /// 返回
        ///</summary>
        [SugarColumn(ColumnName = "result")]
        public string Result { get; set; } = "";

        /// <summary>
        /// 服务名称
        ///</summary>
        [SugarColumn(ColumnName = "service_id")]
        public string ServiceId { get; set; } = "";

        /// <summary>
        /// 平台类型     operation：运维平台  card：一卡通   merchant：商户平台
        ///</summary>
        [SugarColumn(ColumnName = "platform")]
        public string Platform { get; set; } = "";



    }
}
