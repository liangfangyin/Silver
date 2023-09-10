using System;
using System.Collections.Generic;
using System.Text;

namespace Silver.ArcSoft.Model
{
    public class Disting
    {
        public int left { get; set; }
        public int top { get; set; }
        public int right { get; set; }
        public int bottom { get; set; }
        public int orient { get; set; }
        public float roll { get; set; }
        public float pitch { get; set; }
        public float yaw { get; set; }
        public int age { get; set; }
        public int gender { get; set; }
        public int face3DStatus { get; set; }
        public string features { get; set; } = "";

    }
}
