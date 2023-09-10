using Silver.Basic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;

namespace Silver.Email
{
    /// <summary>
    /// 通用邮件
    /// </summary>
    public class MailExtension
    {

        #region 基础参数

        /// <summary>
        /// 邮件服务器地址
        /// </summary>
        private string smtpServer { get; set; } = "";

        /// <summary>
        /// 端口
        /// </summary>
        private int smtpPort { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        private string smtpSubject { get; set; } = "";

        /// <summary>
        /// 用户名
        /// </summary>
        private string smtpUserName { get; set; } = "";

        /// <summary>
        /// 密码
        /// </summary>
        private string smtpPassword { get; set; } = "";

        #endregion


        public MailExtension()
        {
            smtpServer = ConfigurationUtil.GetSection("Email:SmtpServer");
            smtpPort = ConfigurationUtil.GetSection("Email:SmtpPort").ToInt();
            smtpSubject = ConfigurationUtil.GetSection("Email:SmtpSubject");
            smtpUserName = ConfigurationUtil.GetSection("Email:SmtpUserName");
            smtpPassword = ConfigurationUtil.GetSection("Email:SmtpPassword");
        }

        public MailExtension(string smtpServer, int smtpPort, string smtpSubject, string smtpUserName, string smtpPassword)
        {
            this.smtpServer = smtpServer;
            this.smtpPort = smtpPort;
            this.smtpSubject = smtpSubject;
            this.smtpUserName = smtpUserName;
            this.smtpPassword = smtpPassword;
        }

        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="toEmail">发送的邮件地址,多个邮件用英文逗号隔开</param>
        /// <param name="Title">邮件标题</param>
        /// <param name="body">邮件内容</param>
        /// <param name="ccEmail">抄送地址,多个邮件用英文逗号隔开</param>
        /// <param name="attpaths">附件地址(如：F:\\dir.txt)</param>
        /// <returns></returns>
        public bool SendEmail(string toEmail, string Title, string body, string ccEmail = "", List<string> attpaths = null)
        { 
            List<string> toemails = new List<string>();
            List<string> ccemails = new List<string>(); 
            if (!string.IsNullOrEmpty(toEmail))
            {
                toemails = toEmail.Split(',').ToList();
            }
            if (!string.IsNullOrEmpty(ccEmail))
            {
                ccemails = ccEmail.Split(',').ToList();
            } 
            return SendEmail(toemails, Title, body, ccemails, attpaths);
        }

        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="toEmailList">要发送的邮件地址列表</param>
        /// <param name="Title">邮件标题，UTF8格式</param>
        /// <param name="body">邮件内容，UTF8格式</param>
        /// <param name="ccEmailList">抄送地址列表</param>
        /// <param name="AttPathList">附件地址列表(如：F:\\dir.txt)</param>
        /// <returns></returns>
        public bool SendEmail(List<string> toEmailList, string Title, string body, List<string> ccEmailList = null, List<string> AttPathList = null)
        { 
            try
            { 
                //声明一个Mail对象
                MailMessage mymail = new MailMessage();
                //发件人地址：如是自己，在此输入自己的邮箱
                mymail.From = new MailAddress(smtpUserName, smtpSubject);
                //收件人地址
                if (toEmailList != null && toEmailList.Count > 0)
                {
                    foreach (string toEmail in toEmailList)
                    {
                        mymail.To.Add(new MailAddress(toEmail));
                    }
                }
                //邮件主题
                mymail.Subject = Title;
                //邮件标题编码
                mymail.SubjectEncoding = System.Text.Encoding.UTF8;
                //发送邮件的内容
                mymail.Body = body;
                //邮件内容编码
                mymail.BodyEncoding = System.Text.Encoding.UTF8;
                //添加附件
                if (AttPathList != null && AttPathList.Count > 0)
                {
                    foreach (string AttPath in AttPathList)
                    {
                        Attachment myfiles = new Attachment(AttPath);
                        mymail.Attachments.Add(myfiles);
                    }
                }
                //抄送到其他邮箱
                if (ccEmailList != null && ccEmailList.Count > 0)
                {
                    foreach (string ccEmail in ccEmailList)
                    {
                        mymail.CC.Add(new MailAddress(ccEmail));
                    }
                }
                //是否是HTML邮件
                mymail.IsBodyHtml = true;
                //邮件优先级
                mymail.Priority = MailPriority.High;
                //创建一个邮件服务器类
                SmtpClient myclient = new SmtpClient();
                myclient.Host = smtpServer;
                //SMTP服务端口
                myclient.Port = smtpPort;
                //验证登录
                myclient.Credentials = new NetworkCredential(smtpUserName, smtpPassword);
                myclient.Send(mymail); 
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("MailExtension.SendEmail" + ex.Message); 
            }
        }
         

    }
}
