using MiniExcelLibs;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

namespace Peak.Lib.File
{
    /// <summary>
    /// Excel 操作
    /// </summary>
    public class ExceUtil
    {

        /// <summary>
        /// 查询指定工作簿数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path">路径</param>
        /// <param name="sheetName">工作簿名称</param>
        /// <param name="startCell">开始列</param>
        /// <returns></returns>
        public static List<T> Query<T>(string path, string sheetName = null, string startCell = "A1") where T : class, new()
        {
            return MiniExcel.Query<T>(path, sheetName, startCell: startCell).ToList();
        }

        /// <summary>
        /// 查询指定工作簿数据
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="sheetName">工作簿名称</param>
        /// <param name="startCell">开始列</param>
        /// <returns></returns>
        public static DataTable TableQuery(string path, string sheetName = null, string startCell = "A1")
        {
            return MiniExcel.QueryAsDataTable(path, useHeaderRow: true, sheetName: sheetName, startCell: startCell);
        }

        /// <summary>
        /// 查询所有工作薄数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <returns></returns>
        public static Dictionary<string, List<T>> SheetQuery<T>(string path) where T : class, new()
        {
            Dictionary<string, List<T>> dic_key_val = new Dictionary<string, List<T>>();
            var sheetNames = MiniExcel.GetSheetNames(path);
            foreach (var sheetName in sheetNames)
            {
                var rows = MiniExcel.Query<T>(path, sheetName: sheetName).ToList();
                dic_key_val.Add(sheetName, rows);
            }
            return dic_key_val;
        }

        /// <summary>
        /// 查询工作簿的列
        /// </summary>
        /// <param name="path"></param>
        /// <param name="sheetName"></param>
        /// <returns></returns>
        public static string[] SheetColumn(string path, string sheetName = null)
        {
            return MiniExcel.GetColumns(path, sheetName: sheetName).ToArray();
        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path">文件路径</param>
        /// <param name="val">数据集</param>
        /// <returns></returns>
        public static bool SaveAt<T>(string path, List<T> val) where T : class, new()
        {
            List<Dictionary<string, object>> dict_key_val = new List<Dictionary<string, object>>();
            foreach (var item in val)
            {
                Dictionary<string, object> proper_key_val = new Dictionary<string, object>();
                PropertyInfo[] ProInfo = typeof(T).GetProperties();
                foreach (var arrInfo in ProInfo)
                {
                    string name = arrInfo.Name;
                    object value = item.GetType().GetProperty(arrInfo.Name).GetValue(item, null);
                    if (proper_key_val.ContainsKey(name) == false)
                    {
                        proper_key_val.Add(name, value);
                    }
                }
                dict_key_val.Add(proper_key_val);
            }
            if (dict_key_val.Count <= 0)
            {
                return false;
            }
            MiniExcel.SaveAs(path, dict_key_val);
            return true;
        }

        /// <summary>
        /// 保存 
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <param name="sheets">多个工作簿["users"] = list<T> , "department"]=list<T> </param>
        public static void SheetSaveAt(string path, Dictionary<string, object> sheets)
        {
            MiniExcel.SaveAs(path, sheets);
        }

        /// <summary>
        /// CSV 转成 Xlsx
        /// </summary>
        /// <param name="csvPath"></param>
        /// <param name="xlsxPath"></param>
        public static void CsvToXlsx(string csvPath, string xlsxPath)
        {
            var value = MiniExcel.Query(csvPath, true);
            MiniExcel.SaveAs(xlsxPath, value);
        }

        /// <summary>
        /// Excel 模板 数据替换：https://github.com/shps951023/MiniExcel
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <param name="templatePath"></param>
        /// <param name="value"></param>
        public static void SaveByTemplate<T>(string path, string templatePath, T value)
        {
            MiniExcel.SaveAsByTemplate(path, templatePath, value);
        }

        /// <summary>
        /// Excel 模板 数据替换：https://github.com/shps951023/MiniExcel
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <param name="templatePath"></param>
        /// <param name="value"></param>
        public static void SaveByTemplate<T>(string path, string templatePath, Dictionary<string, object> value)
        {
            MiniExcel.SaveAsByTemplate(path, templatePath, value);
        }


    }
}
