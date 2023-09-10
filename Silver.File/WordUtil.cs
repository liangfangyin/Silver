using NPOI.XWPF.UserModel;
using System;
using System.IO;
using System.Reflection;

namespace Peak.Lib.File
{
    /// <summary>
    /// Word操作
    /// </summary>
    public class WordUtil
    {

        /// <summary>
        /// 通用word保存
        /// </summary>
        /// <param name="path"></param>
        /// <param name="doc"></param>
        /// <returns></returns>
        public static (bool, string) SaveWord(string path, XWPFDocument doc)
        {
            try
            {
                using (FileStream sw = System.IO.File.Create(path))
                {
                    doc.Write(sw);
                }
                return (true, "success");
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        /// <summary>
        /// 模板替换生成
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path">导出路径</param>
        /// <param name="templatePath">模板路径</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static (bool, string) SaveTempLate<T>(string path, string templatePath, T value)
        {
            using (FileStream sw = new FileStream(templatePath, FileMode.Open, FileAccess.Read))
            {
                XWPFDocument doc = new XWPFDocument(sw);
                foreach (var para in doc.Paragraphs)
                {
                    ReplaceKey(value, para);
                }
                foreach (var item in doc.Tables)
                {
                    foreach (var row in item.Rows)
                    {
                        foreach (var cell in row.GetTableCells())
                        {
                            foreach (var para in cell.Paragraphs)
                            {
                                ReplaceKey(value, para);
                            }
                        }
                    }
                }
                MemoryStream ms = new MemoryStream();
                doc.Write(ms);
                using (FileStream fs = new FileStream(path, FileMode.Create))
                {
                    using (BinaryWriter bw = new BinaryWriter(fs))
                    {
                        bw.Write(ms.GetBuffer());
                    }
                }
                ms.Dispose();
                ms.Close();
            }
            return (true, "success");
        }

        /// <summary>
        /// word模板内容替换
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="etity">实体数据</param>
        /// <param name="para">段落</param>
        private static void ReplaceKey<T>(T etity, XWPFParagraph para)
        {
            Type entityType = typeof(T);
            PropertyInfo[] properties = entityType.GetProperties();
            string entityName = entityType.Name;//实体类名称
            string paratext = para.ParagraphText;
            var runs = para.Runs;
            string styleid = para.Style;
            string text = "";
            foreach (var run in runs)
            {
                text = run.ToString();
                foreach (var p in properties)
                {
                    string propteryName = "{{" + p.Name + "}}";//Word模板中设定的需要替换的标签
                    object value = p.GetValue(etity);
                    if (value == null)
                    {
                        value = "";
                    }
                    if (text.Contains(propteryName))
                    {
                        text = text.Replace(propteryName, value.ToString());
                    }
                    run.SetText(text);//替换标签文本（重要）
                }
            }
        }


    }
}
