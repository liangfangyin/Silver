using Silver.Basic;
using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Net.Mime;
using System.Threading.Tasks;

namespace Silver.Email
{
    /// <summary>
    /// 阿里邮件发送
    /// </summary>
    public class AliPayMailExtension
    {

        #region 基础参数

        /// <summary>
        /// 邮件服务器地址
        /// </summary>
        private string MailServer { get; set; } = "";

        /// <summary>
        /// 用户名
        /// </summary>
        private string MailUserName { get; set; } = "";
        /// <summary>
        /// 密码
        /// </summary>
        private string MailPassword { get; set; } = "";

        /// <summary>
        /// 名称
        /// </summary>
        private string MailSubject { get; set; } = "";

        /// <summary>
        /// 端口
        /// </summary>
        private int MailPort = 0;

        #endregion

        public AliPayMailExtension()
        {
            MailServer= ConfigurationUtil.GetSection("AliPay:Email:SmtpServer");
            MailUserName = ConfigurationUtil.GetSection("AliPay:Email:SmtpUserName");
            MailPassword = ConfigurationUtil.GetSection("AliPay:Email:SmtpPassword");
            MailSubject = ConfigurationUtil.GetSection("AliPay:Email:SmtpSubject");
            MailPort = ConfigurationUtil.GetSection("AliPay:Email:SmtpPort").ToInt();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mailServer">邮件服务器地址</param>
        /// <param name="mailUserName">用户名</param>
        /// <param name="mailPassword">密码</param>
        /// <param name="mailSubject">名称</param>
        /// <param name="mailPort">端口</param>
        public AliPayMailExtension(string mailServer, string mailUserName, string mailPassword, string mailSubject, int mailPort)
        {
            MailServer = mailServer;
            MailUserName = mailUserName;
            MailPassword = mailPassword;
            MailSubject = mailSubject;
            MailPort = mailPort;
        }


        /// <summary>
        /// 邮件发送
        /// </summary>
        /// <param name="to">收件人</param>
        /// <param name="title">标题</param>
        /// <param name="body">内容</param>
        /// <param name="attpaths">附件地址(如：F:\\dir.txt)</param>
        /// <returns></returns>
        public bool Send(string to, string title, string body, List<string> attpaths = null)
        {
            try
            { 
                MailMessage mailMsg = new MailMessage();
                mailMsg.To.Add(new MailAddress(to));
                mailMsg.From = new MailAddress(MailUserName, MailSubject);
                //可选，设置回信地址 
                //mailMsg.ReplyTo.Add("***");
                // 邮件主题
                mailMsg.Subject = title;
                // 邮件正文内容
                //string text = "欢迎使用阿里云邮件推送";
                string html = body;
                //mailMsg.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(text, null, MediaTypeNames.Text.Plain));
                mailMsg.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(html, null, MediaTypeNames.Text.Html));
                // 添加附件
                if (attpaths != null)
                {
                    foreach (var item in attpaths)
                    {
                        Attachment data = new Attachment(item, MediaTypeNames.Application.Octet);
                        mailMsg.Attachments.Add(data);
                    }
                }
                //邮件推送的SMTP地址和端口
                SmtpClient smtpClient = new SmtpClient(MailServer, MailPort);
                //C#官方文档介绍说明不支持隐式TLS方式，即465端口，需要使用25或者80端口(ECS不支持25端口)，另外需增加一行 smtpClient.EnableSsl = true; 故使用SMTP加密方式需要修改如下：
                //SmtpClient smtpClient = new SmtpClient("smtpdm.aliyun.com", 80);
                smtpClient.EnableSsl = true;
                // 使用SMTP用户名和密码进行验证
                System.Net.NetworkCredential credentials = new System.Net.NetworkCredential(MailUserName, MailPassword);
                smtpClient.Credentials = credentials;
                smtpClient.Send(mailMsg);
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("AliPayMail.Send" + ex.Message);  
            }
        }

        /// <summary>
        /// 邮件发送异步
        /// </summary>
        /// <param name="to">收件人</param>
        /// <param name="title">标题</param>
        /// <param name="body">内容</param>
        /// <param name="attpaths">附件地址(如：F:\\dir.txt),多个邮件用英文逗号隔开</param>
        /// <returns></returns>
        public async Task<bool> SendAsync(string to, string title, string body, List<string> attpaths = null)
        {
            return await Task.Factory.StartNew(() => {
                try
                { 
                    MailMessage mailMsg = new MailMessage();
                    mailMsg.To.Add(new MailAddress(to));
                    mailMsg.From = new MailAddress(MailUserName, MailSubject);
                    //可选，设置回信地址 
                    //mailMsg.ReplyTo.Add("***");
                    // 邮件主题
                    mailMsg.Subject = title;
                    // 邮件正文内容
                    //string text = "欢迎使用阿里云邮件推送";
                    string html = body;
                    //mailMsg.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(text, null, MediaTypeNames.Text.Plain));
                    mailMsg.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(html, null, MediaTypeNames.Text.Html));
                    if (attpaths != null)
                    {
                        foreach (var item in attpaths)
                        {
                            Attachment data = new Attachment(item, MediaTypeNames.Application.Octet);
                            mailMsg.Attachments.Add(data);
                        }
                    }
                    //邮件推送的SMTP地址和端口
                    SmtpClient smtpClient = new SmtpClient(MailServer, MailPort);
                    //C#官方文档介绍说明不支持隐式TLS方式，即465端口，需要使用25或者80端口(ECS不支持25端口)，另外需增加一行 smtpClient.EnableSsl = true; 故使用SMTP加密方式需要修改如下：
                    //SmtpClient smtpClient = new SmtpClient("smtpdm.aliyun.com", 80);
                    smtpClient.EnableSsl = true;
                    // 使用SMTP用户名和密码进行验证
                    System.Net.NetworkCredential credentials = new System.Net.NetworkCredential(MailUserName, MailPassword);
                    smtpClient.Credentials = credentials;
                    smtpClient.Send(mailMsg);
                    return true;
                }
                catch (Exception ex)
                {
                    throw new Exception("AliPayMail.SendAsync" + ex.Message); 
                }
            });
        }
         

    }
}
