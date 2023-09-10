using System;
using System.Collections.Generic;
using System.Text;

namespace Silver.Net.Model.Http
{
    /// <summary>  
    /// Cookie返回类型  
    /// </summary>  
    public enum ResultCookieType
    {
        /// <summary>  
        /// 只返回字符串类型的Cookie  
        /// </summary>  
        String,
        /// <summary>  
        /// CookieCollection格式的Cookie集合同时也返回String类型的cookie  
        /// </summary>  
        CookieCollection
    }
}
