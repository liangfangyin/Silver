using Silver.Basic;
using Silver.Cache;
using SKIT.FlurlHttpClient.Wechat.Api;
using SKIT.FlurlHttpClient.Wechat.Api.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Silver.WeChat
{
    /// <summary>
    /// 微信小程序
    /// </summary>
    public class WeAppletClient
    {
        #region 基础属性
 
        /// <summary>
        /// 
        /// </summary>
        public WechatApiClient client;
         

        /// <summary>
        /// 应用号:如：微信公众平台AppId、微信开放平台AppId、微信小程序AppId、企业微信CorpId等
        /// </summary>
        private string appID { get; set; } = "";

        /// <summary>
        /// 应用秘钥，微信AppSecret
        /// </summary>
        private string appSecrt { get; set; } = "";

        #endregion

        #region 构造函数

        public WeAppletClient()
        { 
            appID = ConfigurationUtil.GetSection("WeChat:AppId");  
            appSecrt = ConfigurationUtil.GetSection("WeChat:AppSecret"); 

            var options = new WechatApiClientOptions()
            {
                AppId = appID,
                AppSecret = appSecrt
            };
            client = new WechatApiClient(options);
        }

        public WeAppletClient(string token, string encodingAESKey, string appID, string appSecrt)
        {
            this.appID = appID;
            this.appSecrt = appSecrt;
            var options = new WechatApiClientOptions()
            {
                AppId = appID,
                AppSecret = appSecrt
            };
            client = new WechatApiClient(options);
        }

        #endregion

        #region 基础接口
         
        /// <summary>
        /// 根据jscode 获取临时凭证
        /// </summary>
        /// <returns></returns>
        public async Task<(bool, SnsJsCode2SessionResponse, string)> ExecuteSnsJsCode2SessionAsync(string jsCode)
        {
            string token = await ExecuteCgibinTokenAsync();
            var response = await client.ExecuteSnsJsCode2SessionAsync(new SnsJsCode2SessionRequest()
            {
                AccessToken = token,
                JsCode = jsCode
            });
            if (!response.IsSuccessful())
            {
                return (false, null, response.ErrorMessage);
            }
            return (response.ErrorCode == 0, response, response.ErrorMessage);
        }

        /// <summary>
        /// 获取基础Token-无用户信息
        /// </summary>
        /// <returns></returns>
        public async Task<string> ExecuteCgibinTokenAsync()
        { 
            string cacheKey = "wechat:token";
            var redisdb = new RedisUtil().GetClient();
            {
                if (redisdb.Exists(cacheKey))
                { 
                    return redisdb.Get<string>(cacheKey);
                }
                var response = await client.ExecuteCgibinTokenAsync(new CgibinTokenRequest());
                if (!response.IsSuccessful())
                {
                    return "";
                }
                if (response.ErrorCode != 0)
                {
                    return "";
                }
                redisdb.Set(cacheKey, response.AccessToken, TimeSpan.FromSeconds(response.ExpiresIn - 10)); 
                return response.AccessToken;
            }
        }

        #endregion
         
        #region 订阅

        /// <summary>
        /// 小程序订阅模板列表
        /// </summary>
        /// <returns></returns>
        public async Task<(bool, WxaApiNewTemplateGetTemplateResponse.Types.Template[], string)> ExecuteWxaApiNewTemplateGetTemplateAsync()
        {
            string token = await ExecuteCgibinTokenAsync();
            var request = new WxaApiNewTemplateGetTemplateRequest()
            {
                AccessToken = token
            };
            var response = await client.ExecuteWxaApiNewTemplateGetTemplateAsync(request);
            if (!response.IsSuccessful())
            {
                return (false, null, response.ErrorMessage);
            }
            return (response.ErrorCode == 0, response.TemplateList, response.ErrorMessage);
        }

        /// <summary>
        /// 小程序发送订阅通知
        /// </summary>
        /// <param name="UserOpenId">用户OpenId</param>
        /// <param name="TemplateId">模板Id</param>
        /// <param name="Data">模板数据</param>
        /// <param name="Page">公众号跳转  可选</param>
        /// <param name="PagePath">小程序跳转  可选</param>
        /// <returns></returns>
        public async Task<(bool, string)> ExecuteCgibinMessageSubscribeBusinessSendAsync(string UserOpenId, string TemplateId, IDictionary<string, CgibinMessageSubscribeBusinessSendRequest.Types.DataItem> Data, string Page = "", string PagePath = "")
        {
            string token = await ExecuteCgibinTokenAsync();
            var request = new CgibinMessageSubscribeBusinessSendRequest()
            {
                AccessToken = token,
                Data = Data,
                ToUserOpenId = UserOpenId,
                TemplateId = TemplateId
            };
            if (!string.IsNullOrEmpty(Page))
            {
                request.Page = Page;
            }
            if (!string.IsNullOrEmpty(PagePath))
            {
                request.MiniProgram = new CgibinMessageSubscribeBusinessSendRequest.Types.MiniProgram()
                {
                    AppId = this.appID,
                    PagePath = PagePath
                };
            }
            var response = await client.ExecuteCgibinMessageSubscribeBusinessSendAsync(request);
            if (!response.IsSuccessful())
            {
                return (false, response.ErrorMessage);
            }
            return (response.ErrorCode == 0, response.ErrorMessage);
        }

        #endregion

        #region 二维码

        /// <summary>
        /// 小程序二维码生成-数量有限
        /// </summary>
        /// <param name="Path">小程序路径</param>
        /// <param name="Width">宽度，默认430</param>
        /// <returns></returns>
        public async Task<(bool, byte[], string)> ExecuteCgibinWxaappCreateWxaQrcodeAsync(string Path, int Width=430)
        {
            string token = await ExecuteCgibinTokenAsync(); 
            var response = await client.ExecuteCgibinWxaappCreateWxaQrcodeAsync(new CgibinWxaappCreateWxaQrcodeRequest()
            {
                AccessToken = token,
                Width = Width,
                Path = Path
            });
            if (!response.IsSuccessful())
            {
                return (false, null ,response.ErrorMessage);
            }
            return (response.ErrorCode == 0, response.RawBytes, response.ErrorMessage);
        }

        /// <summary>
        /// 小程序二维码生成-无数量限制
        /// </summary>
        /// <param name="Path">小程序路径</param>
        /// <param name="Width">宽度，默认430</param>
        /// <returns></returns>
        public async Task<(bool, byte[], string)> ExecuteWxaGetWxaCodeUnlimitAsync(string Path, int Width = 430)
        {
            string token = await ExecuteCgibinTokenAsync();
            var response = await client.ExecuteWxaGetWxaCodeUnlimitAsync(new WxaGetWxaCodeUnlimitRequest()
            {
                AccessToken = token,
                IsAutoColor = true,
                PagePath = Path,
                Width = Width,
                Scene = DateTime.Now.Ticks.ToString()
            });
            if (!response.IsSuccessful())
            {
                return (false, null, response.ErrorMessage);
            }
            return (response.ErrorCode == 0, response.RawBytes, response.ErrorMessage);
        }

        #endregion


    }
}
