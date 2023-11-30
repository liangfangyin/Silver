using Silver.Pay.Model.Response;
using Silver.Pay.Model.Response.Query;
using System;
using System.Collections.Generic;
using System.Text;

namespace Silver.Pay.Model
{
    public class PayQueryResult
    {

        public int StatusCode { get; set; }

        public bool IsSuccess { get; set; } = true;

        public string Message { get; set; } = "";

        public PayQueryResponse Result { get; set; }

    }
}
