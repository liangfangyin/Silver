using SqlSugar;
using System;

namespace Silver.NLog.Core.ORM
{
    /// <summary>
    /// 接口日志
    ///</summary> 
    [SplitTable(SplitType.Month)]
    [SugarTable("logs_cmd_{year}{month}{day}")]
    public class LogsCmd
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
        /// 接口名称
        ///</summary>
        [SugarColumn(ColumnName = "cmd")]
        public string Cmd { get; set; } = "";

        /// <summary>
        /// 发送参数
        ///</summary>
        [SugarColumn(ColumnName = "data")]
        public string Data { get; set; } = "";

        /// <summary>
        /// 餐厅id
        ///</summary>
        [SugarColumn(ColumnName = "shop_id")]
        public long ShopId { get; set; } = 0;

        /// <summary>
        /// 档口id
        ///</summary>
        [SugarColumn(ColumnName = "stall_id")]
        public long StallId { get; set; } = 0;

        /// <summary>
        /// 终端id
        ///</summary>
        [SugarColumn(ColumnName = "term_id")]
        public long TermId { get; set; } = 0;

        /// <summary>
        /// 用时
        ///</summary>
        [SugarColumn(ColumnName = "timer")]
        public int Timer { get; set; } = 0;

        /// <summary>
        /// 状态码
        ///</summary>
        [SugarColumn(ColumnName = "code")]
        public int Code { get; set; } = 0;

        /// <summary>
        /// 返回
        ///</summary>
        [SugarColumn(ColumnName = "result")]
        public string Result { get; set; } = "";

        /// <summary>
        /// 操作员
        ///</summary>
        [SugarColumn(ColumnName = "oper")]
        public string Oper { get; set; } = "";

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
