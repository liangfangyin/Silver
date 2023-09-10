using System;
using System.Collections.Generic;
using System.Text;

namespace Silver.ArcSoft.Model
{
    /// <summary>
    /// 年龄结果结构体
    /// </summary>
    public struct ASF_AgeInfo
    {
        /// <summary>
        /// 年龄检测结果集合
        /// </summary>
        public IntPtr ageArray;
        /// <summary>
        /// 结果集大小
        /// </summary>
        public int num;
    }
}
