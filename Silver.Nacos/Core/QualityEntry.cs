using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Text;

namespace Silver.Nacos.Core
{
    public class QualityEntry
    {

        public static string ProjectKeyOfValue<T>(T info)
        {
            StringBuilder sb = new StringBuilder(); 
            PropertyInfo[] listProperty = typeof(T).GetProperties();
            int i = 0;
            foreach (var arrInfo in listProperty)
            {
                AttributeCollection attributes = TypeDescriptor.GetProperties(info.GetType())[i].Attributes;
                if (((System.ComponentModel.BrowsableAttribute)(attributes[typeof(BrowsableAttribute)])).Browsable != false)
                {
                    try
                    {
                        if (sb.Length > 0)
                        {
                            sb.Append("&");
                        }
                        var value = info.GetType().GetProperty(arrInfo.Name).GetValue(info, null);
                        if (value == null)
                        {
                            sb.Append($"{arrInfo.Name}=");
                        }
                        else
                        {
                            sb.Append($"{arrInfo.Name}={value.ToString()}");
                        }
                    }
                    catch { }
                }
                i++;
            }
            return sb.ToString();
        }

    }
}
