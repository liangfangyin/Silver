using Ocelot.Values;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Silver.Ocelot.Nacos.Model
{
    public class NacosCacheService
    {


        /// <summary>
        /// 当前时间
        /// </summary>
        public DateTime NowDate { get; set; } = DateTime.Now;

        /// <summary>
        /// 服务名称
        /// </summary>
        public string ServiceName { get; set; } = "";

        /// <summary>
        /// 分组名称
        /// </summary>
        public string GroupName { get; set; } = "";

        /// <summary>
        /// 服务列表
        /// </summary>
        public List<Service> Services { get; set; } = new List<Service>();


    }
}
