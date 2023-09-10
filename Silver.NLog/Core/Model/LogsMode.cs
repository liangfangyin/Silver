using System;
using System.Collections.Generic;
using System.Text;

namespace Silver.NLog.Core.Model
{
    public enum LogsMode
    {
        /// <summary>
        /// 本地
        /// </summary>
        Local = 1,
        /// <summary>
        /// 云端
        /// </summary>
        Cloud = 2,
        /// <summary>
        /// 打印输出
        /// </summary>
        Console = 3

    }
}
