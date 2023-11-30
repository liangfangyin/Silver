using System;
using System.Collections.Generic;
using System.Text;

namespace Silver.Pay.Model
{
    public class PayResult
    {
        public int StatusCode { get; set; }

        public bool IsSuccess { get; set; } = true;

        public string Message { get; set; } = "";

        public string Result { get; set; }

    }
}
