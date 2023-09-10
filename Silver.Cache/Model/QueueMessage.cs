using System;
using System.Collections.Generic;
using System.Text;

namespace Silver.Cache.Model
{
    public class QueueMessage
    {

        public string name { get; set; }

        public int typeid { get; set; }

        public string type { get; set; }

        public string val { get; set; }

        public DateTime endTime { get; set; } = DateTime.Now;

    }
}
