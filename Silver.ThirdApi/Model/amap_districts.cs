using System;
using System.Collections.Generic;
using System.Text;

namespace Silver.ThirdApi.Model
{
    public class amap_districts
    {
        public string adcode { get; set; }
        public string name { get; set; }
        public string center { get; set; }
        public string level { get; set; }
        public List<amap_districts> districts { get; set; }

    }
}
