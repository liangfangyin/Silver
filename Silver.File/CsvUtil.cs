using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace Silver.File
{
    public class CsvUtil
    {
        /// <summary>  
        /// 导出Csv文件
        /// </summary>  
        /// <param name="tableheader">表头 名称|name,性别|sex </param>  
        /// <param name="data">DataTable</param>  
        /// <param name="filePath">物理路径</param>     
        public static void Write<T>(string tableheader, List<T> data, string filePath)
        {
            using (StreamWriter strmWriterObj = new StreamWriter(filePath, false, System.Text.Encoding.UTF8))
            {
                if (!string.IsNullOrEmpty(tableheader))
                {
                    string header = "";
                    foreach (var item in tableheader.Split(","))
                    {
                        if (header.Length > 0)
                        {
                            header += ",";
                        }
                        header += (item.Split("|")[0]);
                    }
                    strmWriterObj.WriteLine(header);
                }
                foreach (var item in data)
                {
                    Dictionary<string, object> proper_key_val = new Dictionary<string, object>();
                    PropertyInfo[] ProInfo = typeof(T).GetProperties();
                    foreach (var arrInfo in ProInfo)
                    {
                        string name = arrInfo.Name;
                        object value = item.GetType().GetProperty(arrInfo.Name).GetValue(item, null);
                        if (proper_key_val.ContainsKey(name) == false)
                        {
                            proper_key_val.Add(name.ToLower(), value);
                        }
                    }
                    string strBufferLine = "";
                    foreach (var column in tableheader.Split(","))
                    {
                        if (strBufferLine.Length > 0)
                        {
                            strBufferLine += ",";
                        }
                        if (proper_key_val.ContainsKey(column.Split("|")[1].ToLower()))
                        {
                            strBufferLine += proper_key_val[column.Split("|")[1]];
                        }
                        else
                        {
                            strBufferLine += "";
                        }
                    }
                    strmWriterObj.WriteLine(strBufferLine);
                }
            }
        }

        /// <summary>
        /// 动态字段写入
        /// </summary>
        /// <param name="search"></param>
        /// <param name="header"></param>
        /// <param name="data"></param>
        /// <param name="filePath"></param>
        public static void Write(string title,Dictionary<string,object> search,List<string> header,List<Dictionary<string, object>> data, string filePath)
        {
            using (StreamWriter strmWriterObj = new StreamWriter(filePath, false, System.Text.Encoding.UTF8))
            {
                strmWriterObj.WriteLine(title); 
                if (search.Count > 0)
                {
                    StringBuilder sbSearch = new StringBuilder();
                    foreach (var key in search.Keys)
                    {
                        if (sbSearch.Length > 0)
                        {
                            sbSearch.Append(",");
                        }
                        sbSearch.Append($"{key}：{search[key]}");
                    }
                    strmWriterObj.WriteLine(sbSearch.ToString());
                }
                if (header.Count > 0)
                {
                    StringBuilder sbHeader = new StringBuilder();
                    foreach (var value in header)
                    {
                        if (sbHeader.Length > 0)
                        {
                            sbHeader.Append(",");
                        }
                        sbHeader.Append(value);
                    }
                    strmWriterObj.WriteLine(sbHeader.ToString());
                }
                if (data.Count > 0)
                {
                    foreach (Dictionary<string, object> item in data)
                    {
                        StringBuilder sbBody = new StringBuilder();
                        foreach (var key in item.Keys)
                        {
                            if (sbBody.Length > 0)
                            {
                                sbBody.Append(",");
                            }
                            sbBody.Append(item[key]);
                        }
                        strmWriterObj.WriteLine(sbBody.ToString());
                    }
                }
            }
        }


    }
}
