using System;
using System.Collections.Generic;
using System.Text;

namespace Silver.ThirdApi.Model
{
    public class ResponeResult
    {
        public int resultcode { get; set; }
        public string reason { get; set; }
        public int error_code { get; set; }
    }

    public enum RequestMethod
    {
        POST, GET
    }

}
