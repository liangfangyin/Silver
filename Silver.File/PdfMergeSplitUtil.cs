using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf.IO;
using PdfSharpCore.Pdf;
using System;
using System.Collections.Generic;
using System.IO;

namespace Peak.Lib.File
{
    public class PdfMergeSplitUtil
    {


        /// <summary>
        /// PDF文件合并
        /// </summary>
        /// <param name="SourcePath">待合并文件目录</param>
        /// <param name="TargetPath">输出文件路径</param>
        /// <param name="NewFileName">输出文件名称</param>
        /// <returns>bool是否成果  string：结果描述或异常描述 </returns>
        public static (bool, string) MergePDF(string SourcePath, string TargetPath, string NewFileName)
        {
            try
            {
                PdfDocument outputDocument = new PdfDocument();
                string[] pdffiles = Directory.GetFiles(SourcePath);
                foreach (string path in pdffiles)
                {
                    PdfDocument inputDocument1 = PdfReader.Open(path, PdfDocumentOpenMode.Import);
                    for (int i = 0; i < inputDocument1.PageCount; i++)
                    {
                        PdfPage page1 = inputDocument1.Pages[i];
                        outputDocument.AddPage(page1);
                    }
                    inputDocument1.Dispose();
                    inputDocument1.Close();
                }
                if (!Directory.Exists(TargetPath))
                {
                    Directory.CreateDirectory(TargetPath);
                }
                outputDocument.Save(TargetPath + "//" + NewFileName);
                outputDocument.Dispose();
                outputDocument.Close();
            }
            catch (Exception ex)
            {
                return (false, ex.ToString());
            }
            return (true, "success");
        }


        /// <summary>
        /// PDF文件合并
        /// </summary>
        /// <param name="SourcePath">待合并文件目录</param>
        /// <param name="TargetPath">输出文件路径</param>
        /// <param name="NewFileName">输出文件名称</param>
        /// <returns>bool是否成果  string：结果描述或异常描述 </returns>
        public static (bool, string) MergePDF(List<Stream> SourcePath, string TargetPath, string NewFileName)
        {
            try
            {
                PdfDocument outputDocument = new PdfDocument();
                foreach (Stream path in SourcePath)
                {
                    PdfDocument inputDocument1 = PdfReader.Open(path, PdfDocumentOpenMode.Import);
                    for (int i = 0; i < inputDocument1.PageCount; i++)
                    {
                        PdfPage page1 = inputDocument1.Pages[i];
                        outputDocument.AddPage(page1);
                    }
                    inputDocument1.Dispose();
                    inputDocument1.Close();
                }
                if (!Directory.Exists(TargetPath))
                {
                    Directory.CreateDirectory(TargetPath);
                }
                outputDocument.Save(TargetPath + "//" + NewFileName);
                outputDocument.Dispose();
                outputDocument.Close();
            }
            catch (Exception ex)
            {
                return (false, ex.ToString());
            }
            return (true, "success");
        }


        /// <summary>
        /// PDF文件合并,按页面合并
        /// </summary>
        /// <param name="SourcePath">待合并文件目录<路径,合并页面> ，如<"c://1.paf",[1,3,5,6]> </param>
        /// <param name="TargetPath">输出文件路径</param>
        /// <param name="NewFileName">输出文件名称</param>
        /// <returns>bool是否成果  string：结果描述或异常描述 </returns>
        public static (bool, string) MergePDFPage(Dictionary<string, int[]> SourcePath, string TargetPath, string NewFileName)
        {
            try
            {
                PdfDocument outputDocument = new PdfDocument();
                foreach (string path in SourcePath.Keys)
                {
                    int[] pager = SourcePath[path];
                    PdfDocument inputDocument1 = PdfReader.Open(path, PdfDocumentOpenMode.Import);
                    if (pager.Length <= 0)
                    {
                        for (int i = 0; i < inputDocument1.PageCount; i++)
                        {
                            PdfPage page1 = inputDocument1.Pages[i];
                            outputDocument.AddPage(page1);
                        }
                    }
                    else
                    {
                        for (int i = 0; i < inputDocument1.PageCount; i++)
                        {
                            for (int j = 0; j < pager.Length; j++)
                            {
                                if (pager[j] == i + 1)
                                {
                                    PdfPage page1 = inputDocument1.Pages[i];
                                    outputDocument.AddPage(page1);
                                }
                            }
                        }
                    }
                    inputDocument1.Dispose();
                    inputDocument1.Close();
                }
                if (!Directory.Exists(TargetPath))
                {
                    Directory.CreateDirectory(TargetPath);
                }
                outputDocument.Save(TargetPath + "//" + NewFileName);
                outputDocument.Dispose();
                outputDocument.Close();
            }
            catch (Exception ex)
            {
                return (false, ex.ToString());
            }
            return (true, "success");
        }


        /// <summary>
        /// PDF文件合并,按页面合并
        /// </summary>
        /// <param name="SourcePath">待合并文件目录<路径,合并页面> ，如<"c://1.paf",[1,3,5,6]> </param>
        /// <param name="TargetPath">输出文件路径</param>
        /// <param name="NewFileName">输出文件名称</param>
        /// <returns>bool是否成果  string：结果描述或异常描述 </returns>
        public static (bool, string) MergePDFPage(Dictionary<Stream, int[]> SourcePath, string TargetPath, string NewFileName)
        {
            try
            {
                PdfDocument outputDocument = new PdfDocument();
                foreach (Stream path in SourcePath.Keys)
                {
                    int[] pager = SourcePath[path];
                    PdfDocument inputDocument1 = PdfReader.Open(path, PdfDocumentOpenMode.Import);
                    if (pager.Length <= 0)
                    {
                        for (int i = 0; i < inputDocument1.PageCount; i++)
                        {
                            PdfPage page1 = inputDocument1.Pages[i];
                            outputDocument.AddPage(page1);
                        }
                    }
                    else
                    {
                        for (int i = 0; i < inputDocument1.PageCount; i++)
                        {
                            for (int j = 0; j < pager.Length; j++)
                            {
                                if (pager[j] == i + 1)
                                {
                                    PdfPage page1 = inputDocument1.Pages[i];
                                    outputDocument.AddPage(page1);
                                }
                            }
                        }
                    }
                    inputDocument1.Dispose();
                    inputDocument1.Close();
                }
                if (!Directory.Exists(TargetPath))
                {
                    Directory.CreateDirectory(TargetPath);
                }
                outputDocument.Save(TargetPath + "//" + NewFileName);
                outputDocument.Dispose();
                outputDocument.Close();
            }
            catch (Exception ex)
            {
                return (false, ex.ToString());
            }
            return (true, "success");
        }


        /// <summary>
        /// PDF文件拆分
        /// </summary>
        /// <param name="SourcePath">待拆分文件</param>
        /// <param name="TargetPath">输出文件路径</param>
        /// <returns>bool是否成果  List<string>分页文件路径  string：结果描述或异常描述 </returns>
        public static (bool, List<string>, string) SplitPDF(string SourcePath, string TargetPath)
        {
            List<string> list_page = new List<string>();
            try
            {
                if (!Directory.Exists(TargetPath))
                {
                    Directory.CreateDirectory(TargetPath);
                }
                PdfDocument inputDocument1 = PdfReader.Open(SourcePath, PdfDocumentOpenMode.Import);
                for (int i = 0; i < inputDocument1.PageCount; i++)
                {
                    string path = TargetPath + "//" + (i + 1) + ".pdf";
                    PdfPage page1 = inputDocument1.Pages[i];
                    PdfDocument outputDocument = new PdfDocument();
                    outputDocument.AddPage(page1);
                    outputDocument.Save(path);
                    outputDocument.Dispose();
                    outputDocument.Close();
                    list_page.Add(path);
                }
                inputDocument1.Dispose();
                inputDocument1.Close();
            }
            catch (Exception ex)
            {
                return (false, list_page, ex.ToString());
            }
            return (true, list_page, "success");
        }


        /// <summary>
        /// PDF文件拆分
        /// </summary>
        /// <param name="SourcePath">待拆分文件</param>
        /// <param name="TargetPath">输出文件路径</param>
        /// <returns>bool是否成果  List<string>分页文件路径  string：结果描述或异常描述 </returns>
        public static (bool, List<string>, string) SplitPDF(Stream SourcePath, string TargetPath)
        {
            List<string> list_page = new List<string>();
            try
            {
                if (!Directory.Exists(TargetPath))
                {
                    Directory.CreateDirectory(TargetPath);
                }
                PdfDocument inputDocument1 = PdfReader.Open(SourcePath, PdfDocumentOpenMode.Import);
                for (int i = 0; i < inputDocument1.PageCount; i++)
                {
                    string path = TargetPath + "//" + (i + 1) + ".pdf";
                    PdfPage page1 = inputDocument1.Pages[i];
                    PdfDocument outputDocument = new PdfDocument();
                    outputDocument.AddPage(page1);
                    outputDocument.Save(path);
                    outputDocument.Dispose();
                    outputDocument.Close();
                    list_page.Add(path);
                }
                inputDocument1.Dispose();
                inputDocument1.Close();
            }
            catch (Exception ex)
            {
                return (false, list_page, ex.ToString());
            }
            return (true, list_page, "success");
        }


        /// <summary>
        /// PDF文件拆分,输出指定页面
        /// </summary>
        /// <param name="SourcePath">待拆分文件</param>
        /// <param name="TargetPath">输出文件路径</param>
        /// <param name="pages">输出的页码范围,如果不传则为全部页面，如：["1-5","8-12","15-26"] </param>
        public static (bool, List<string>, string) SplitPDFPage(string SourcePath, string TargetPath, string[] pages)
        {
            List<string> list_page = new List<string>();
            if (!Directory.Exists(TargetPath))
            {
                Directory.CreateDirectory(TargetPath);
            }
            try
            {
                PdfDocument inputDocument1 = PdfReader.Open(SourcePath, PdfDocumentOpenMode.Import);
                if (pages.Length <= 0)
                {
                    for (int i = 0; i < inputDocument1.PageCount; i++)
                    {
                        string path = TargetPath + "//" + (i + 1) + ".pdf";
                        PdfPage page1 = inputDocument1.Pages[i];
                        PdfDocument outputDocument = new PdfDocument();
                        outputDocument.AddPage(page1);
                        outputDocument.Save(path);
                        outputDocument.Dispose();
                        outputDocument.Close();
                        list_page.Add(path);
                    }
                }
                else
                {
                    foreach (var item in pages)
                    {
                        string path = TargetPath + "//" + item + ".pdf";
                        PdfDocument outputDocument = new PdfDocument();
                        int start = Convert.ToInt32(item.Split('-')[0]);
                        int end = Convert.ToInt32(item.Split('-')[1]);
                        for (int i = 0; i < inputDocument1.PageCount; i++)
                        {
                            if (i + 1 >= start && i + 1 <= end)
                            {
                                PdfPage page1 = inputDocument1.Pages[i];
                                outputDocument.AddPage(page1);
                            }
                        }
                        outputDocument.Save(path);
                        outputDocument.Dispose();
                        outputDocument.Close();
                        list_page.Add(path);
                    }
                }
                inputDocument1.Dispose();
                inputDocument1.Close();
            }
            catch (Exception ex)
            {
                return (false, list_page, ex.ToString());
            }
            return (true, list_page, "success");
        }


        /// <summary>
        /// PDF文件拆分,输出指定页面
        /// </summary>
        /// <param name="SourcePath">待拆分文件</param>
        /// <param name="TargetPath">输出文件路径</param>
        /// <param name="pages">输出的页码范围,如果不传则为全部页面，如：["1-5","8-12","15-26"] </param>
        public static (bool, List<string>, string) SplitPDFPage(Stream SourcePath, string TargetPath, string[] pages)
        {
            List<string> list_page = new List<string>();
            if (!Directory.Exists(TargetPath))
            {
                Directory.CreateDirectory(TargetPath);
            }
            try
            {
                PdfDocument inputDocument1 = PdfReader.Open(SourcePath, PdfDocumentOpenMode.Import);
                if (pages.Length <= 0)
                {
                    for (int i = 0; i < inputDocument1.PageCount; i++)
                    {
                        string path = TargetPath + "//" + (i + 1) + ".pdf";
                        PdfPage page1 = inputDocument1.Pages[i];
                        PdfDocument outputDocument = new PdfDocument();
                        outputDocument.AddPage(page1);
                        outputDocument.Save(path);
                        outputDocument.Dispose();
                        outputDocument.Close();
                        list_page.Add(path);
                    }
                }
                else
                {
                    foreach (var item in pages)
                    {
                        string path = TargetPath + "//" + item + ".pdf";
                        PdfDocument outputDocument = new PdfDocument();
                        int start = Convert.ToInt32(item.Split('-')[0]);
                        int end = Convert.ToInt32(item.Split('-')[1]);
                        for (int i = 0; i < inputDocument1.PageCount; i++)
                        {
                            if (i + 1 >= start && i + 1 <= end)
                            {
                                PdfPage page1 = inputDocument1.Pages[i];
                                outputDocument.AddPage(page1);
                            }
                        }
                        outputDocument.Save(path);
                        outputDocument.Dispose();
                        outputDocument.Close();
                        list_page.Add(path);
                    }
                }
                inputDocument1.Dispose();
                inputDocument1.Close();
            }
            catch (Exception ex)
            {
                return (false, list_page, ex.ToString());
            }
            return (true, list_page, "success");
        }

        /// <summary>
        /// 图片导出PDF
        /// </summary>
        /// <param name="ImagePath">图片列表</param>
        /// <param name="TargetPath">输出文件路径</param>
        /// <returns></returns>
        public static (bool, string) ImageToPDF(List<string> ImagePath, string TargetPath)
        {
            string TargerFolder = "";
            try
            {
                TargerFolder = TargetPath.Substring(0, TargetPath.LastIndexOf("\\"));
            }
            catch
            {
                TargerFolder = TargetPath.Substring(0, TargetPath.LastIndexOf("/"));
            }
            if (!Directory.Exists(TargerFolder))
            {
                Directory.CreateDirectory(TargerFolder);
            }
            try
            {
                using (PdfDocument inputDocument1 = new PdfDocument())
                {
                    foreach (var path in ImagePath)
                    {
                        inputDocument1.AddPage(new PdfPage());
                        var page = inputDocument1.Pages[inputDocument1.PageCount - 1];
                        XGraphics graphPage = XGraphics.FromPdfPage(page);
                        XImage ximage = XImage.FromFile(path);
                        double height = page.Width / (ximage.PointWidth / ximage.PixelHeight);
                        graphPage.DrawImage(ximage, 0, 0, page.Width, height);
                    }
                    inputDocument1.Save(TargetPath);
                }
                return (true, "success");
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

    }
}
