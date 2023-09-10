using Peak.Lib.Sms.Model;
using Silver.Basic;
using Silver.Cache;
using System;
using System.Collections.Generic;
using Tea;

namespace Peak.Lib.Sms
{
    public class AliPaySmsUtil
    {

        #region  参数配置

        /// <summary>
        /// 产品名称:云通信短信API产品,开发者无需替换
        /// </summary>
        private const String product = "Dysmsapi";

        /// <summary>
        /// 产品域名,开发者无需替换
        /// </summary>
        private const String domain = "dysmsapi.aliyuncs.com";

        /// <summary>
        /// appid
        /// </summary>
        private string accessKeyId { get; set; } = "";

        /// <summary>
        /// 秘钥
        /// </summary>
        private string accessKeySecret { get; set; } = "";

        /// <summary>
        /// 短信签名-可在短信控制台中找到
        /// </summary>
        private string signName { get; set; } = "";

        /// <summary>
        /// 短信发送间隔时间
        /// </summary>
        private int sendTimeOut { get; set; } = 60;

        #endregion

        public AliPaySmsUtil()
        {
            this.accessKeyId = ConfigurationUtil.GetSection("SMS:AppID");
            this.accessKeySecret = ConfigurationUtil.GetSection("SMS:AppSecret");  
            this.signName = ConfigurationUtil.GetSection("SMS:SignName"); 
        }

        public AliPaySmsUtil(string accessKeyId, string accessKeySecret, string signName, int sendTimeOut = 60)
        {
            this.accessKeyId = accessKeyId;
            this.accessKeySecret = accessKeySecret;
            this.signName = signName;
            this.sendTimeOut = sendTimeOut;
        }

        #region 应用

        /// <summary>
        /// 发送验证码  模板参数，必须为 code
        /// </summary>
        /// <param name="template">模板编码</param>
        /// <param name="mobile">手机号码</param>
        /// <param name="code">验证码</param>
        /// <param name="ip">IP地址</param>
        /// <returns></returns>
        public (bool, string) SmsCode(string template, string mobile, int code)
        {
            SmsAliPayResult result = new SmsAliPayResult();
            string cacheKey = $"sms:code:{mobile}";
            string cacheValidKey = $"sms:code:valid:{mobile}";
            var redisDB = new RedisUtil().GetClient();
            if (redisDB.Exists(cacheKey))
            {
                return (false, "验证码发送过于频繁");
            }
            var resultBody = new AliPaySmsUtil().SendSms(mobile, template, "{\"code\":\"" + code + "\"}");
            if (resultBody.IsSuccess == false)
            {
                return (false, resultBody.Message);
            }
            redisDB.Set(cacheKey, code, sendTimeOut);
            redisDB.Set(cacheValidKey, code, 60 * 5);
            return (true, "发送成功");
        }

        /// <summary>
        /// 短信验证码验证
        /// </summary>
        /// <param name="mobile">手机号码</param>
        /// <param name="code">验证码</param>
        /// <returns></returns>
        public (bool, string) SmsCheck(string mobile, string code)
        {
            string cacheValidKey = $"sms:code:valid:{mobile}";
            var redisDB = new RedisUtil().GetClient();
            if (!redisDB.Exists(cacheValidKey))
            {
                return (false, "验证码不正确");
            }
            if (redisDB.Get<string>(cacheValidKey) != code)
            {
                return (false, "验证码不正确");
            }
            return (true, "验证码正确");
        }

        /// <summary>
        /// 短信内容发送
        /// </summary>
        /// <param name="mobile">手机号码</param>
        /// <param name="template">模板编码</param>
        /// <param name="templateParam">短信内容json {\"name\":\"Tom\",\"code\":\"123\"} </param>
        /// <returns></returns>
        public (bool, string) SmsSubstance(string mobile, string template, string templateParam)
        {
            string cacheKey = $"sms:substance:{mobile}";
            var redisDB = new RedisUtil().GetClient();
            if (redisDB.Exists(cacheKey))
            {
                return (false, "验证码发送过于频繁");
            }
            var resultBody = new AliPaySmsUtil().SendSms(mobile, template, templateParam);
            if (resultBody.IsSuccess == false)
            {
                return (false, resultBody.Message);
            }
            redisDB.Set(cacheKey, templateParam, sendTimeOut);
            return (true, "发送成功");
        }

        #endregion

        #region  基础方法

        /// <summary>
        /// 短信发送
        /// </summary>
        /// <param name="mobile">手机号：1870112111 </param>
        /// <param name="template">模板代码:SMS_201471356</param>
        /// <param name="param">模板：{\"name\":\"Tom\",\"code\":\"123\"}  "{\"code\":\"123467\"}"</param>
        /// <returns></returns>
        private SmsAliPayResult SendSms(string mobile, string template, string param)
        {
            SmsAliPayResult result = new SmsAliPayResult();
            AlibabaCloud.SDK.Dysmsapi20170525.Client client = CreateClient(accessKeyId, accessKeySecret);
            AlibabaCloud.SDK.Dysmsapi20170525.Models.SendSmsRequest sendSmsRequest = new AlibabaCloud.SDK.Dysmsapi20170525.Models.SendSmsRequest
            {
                PhoneNumbers = mobile,
                SignName = signName,
                TemplateCode = template,
                TemplateParam = param,
                OutId = "code",
            };
            AlibabaCloud.TeaUtil.Models.RuntimeOptions runtime = new AlibabaCloud.TeaUtil.Models.RuntimeOptions();
            try
            {
                // 复制代码运行请自行打印 API 的返回值
                var response = client.SendSmsWithOptions(sendSmsRequest, runtime);
                if (response.StatusCode != 200)
                {
                    result.IsSuccess = false;
                    result.Code = response.Body.Code;
                    result.Message = response.Body.Message;
                    return result;
                }
                result.IsSuccess = true;
                result.Code = response.Body.Code;
                result.Message = response.Body.Message;
                result.BizId = response.Body.BizId;
                result.RequestId = response.Body.RequestId;
                return result;
            }
            catch (TeaException error)
            {
                result.IsSuccess = false;
                result.Code = "error";
                result.Message = error.Message;
                return result;
            }
            catch (Exception _error)
            {
                TeaException error = new TeaException(new Dictionary<string, object>
                {
                    { "message", _error.Message }
                });
                result.IsSuccess = false;
                result.Code = "error";
                result.Message = error.Message;
                return result;
            }
        }

        /**
        * 使用AK&SK初始化账号Client
        * @param accessKeyId
        * @param accessKeySecret
        * @return Client
        * @throws Exception
        */
        public static AlibabaCloud.SDK.Dysmsapi20170525.Client CreateClient(string accessKeyId, string accessKeySecret)
        {
            AlibabaCloud.OpenApiClient.Models.Config config = new AlibabaCloud.OpenApiClient.Models.Config
            {
                // 您的 AccessKey ID
                AccessKeyId = accessKeyId,
                // 您的 AccessKey Secret
                AccessKeySecret = accessKeySecret,
            };
            // 访问的域名
            config.Endpoint = "dysmsapi.aliyuncs.com";
            return new AlibabaCloud.SDK.Dysmsapi20170525.Client(config);
        }

        #endregion
    }
}
