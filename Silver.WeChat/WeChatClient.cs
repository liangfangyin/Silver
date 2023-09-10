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
    /// 微信公众号
    /// </summary>
    public class WeChatClient
    {

        #region 基础属性
 
        /// <summary>
        /// 
        /// </summary>
        public WechatApiClient client;

        /// <summary>
        /// 令牌(Token)
        /// </summary>
        private string token { get; set; } = "Token";

        /// <summary>
        /// 消息加解密密钥(EncodingAESKey)
        /// </summary>
        private string encodingAESKey { get; set; } = "";

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

        public WeChatClient()
        {
            token = ConfigurationUtil.GetSection("WeChat:Token");
            encodingAESKey = ConfigurationUtil.GetSection("WeChat:EncodingAESKey");
            appID = ConfigurationUtil.GetSection("WeChat:AppId");
            appSecrt = ConfigurationUtil.GetSection("WeChat:AppSecret");

            var options = new WechatApiClientOptions()
            {
                AppId = appID,
                AppSecret = appSecrt
            };
            client = new WechatApiClient(options);
        }

        public WeChatClient(string token, string encodingAESKey, string appID, string appSecrt)
        {
            this.token = token;
            this.encodingAESKey = encodingAESKey;
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
        /// 获取用户授权Code
        /// </summary>
        /// <param name="RedirectUri">回调地址</param>
        /// <returns></returns>
        public string ExecuteCodeUrl(string RedirectUri)
        {
            return $"https://open.weixin.qq.com/connect/oauth2/authorize?appid={this.appID}&redirect_uri={RedirectUri}&response_type=code&scope=snsapi_userinfo&state=STATE#wechat_redirect";
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
         
        /// <summary>
        /// 获取授权页Ticket-- JS API的临时票据
        /// </summary>
        /// <returns></returns>
        public async Task<(bool, string, string)> ExecuteCgibinTicketGetTicketAsync()
        {
            string token = await ExecuteCgibinTokenAsync();
            var response = await client.ExecuteCgibinTicketGetTicketAsync(new CgibinTicketGetTicketRequest() { AccessToken = token });
            if (!response.IsSuccessful())
            {
                return (false, "", response.ErrorMessage);
            }
            return (response.ErrorCode == 0, response.Ticket, response.ErrorMessage);
        }

        /// <summary>
        /// 微信扫码登录，二维码链接
        /// </summary>
        /// <param name="redirect_uri"></param>
        /// <returns></returns>
        public string GetAuthorizeUrlQrCode(string redirect_uri)
        {
            var _url = "https://open.weixin.qq.com/connect/qrconnect?appid=" + this.appID
                  + "&scope=snsapi_login&redirect_uri=" + redirect_uri
                  + "&state=" + DateTime.Now.Ticks
                  + "&login_type=jssdk&style=black&self_redirect=default&href=";
            return _url;
        }

        /// <summary>
        /// 获取用户Token--用户信息
        /// </summary>
        /// <param name="Code"></param>
        /// <returns></returns>
        public async Task<(bool, SnsOAuth2AccessTokenResponse, string)> ExecuteSnsOAuth2AccessTokenAsync(string Code)
        {
            var response = await client.ExecuteSnsOAuth2AccessTokenAsync(new SnsOAuth2AccessTokenRequest()
            {
                Code = Code,
                AccessToken = await ExecuteCgibinTokenAsync()
            });
            if (!response.IsSuccessful())
            {
                return (false, null, response.ErrorMessage);
            }
            if (response.ErrorCode != 0)
            {
                return (false, null, response.ErrorMessage);
            }
            return (true, response, "ok");
        }

        /// <summary>
        /// 获取用户详情
        /// </summary>
        /// <param name="OpenId">用户OpenId</param>
        /// <param name="Language"></param>
        /// <returns></returns>
        public async Task<(bool, CgibinUserInfoResponse, string)> ExecuteCgibinUserInfoAsync(string OpenId, string Language = "zh_CN")
        {
            string token = await ExecuteCgibinTokenAsync();
            string cacheKey = $"wechat:userinfo:{OpenId}";
            var redisdb = new RedisUtil().GetClient();
            {
                if (redisdb.Exists(cacheKey))
                {
                    return (true, redisdb.Get<CgibinUserInfoResponse>(cacheKey), "success");
                }
                var response = await client.ExecuteCgibinUserInfoAsync(new CgibinUserInfoRequest()
                {
                    OpenId = OpenId,
                    Language = Language,
                    AccessToken = token
                });
                if (!response.IsSuccessful())
                {
                    return (false, null, response.ErrorMessage);
                }
                if (response.ErrorCode != 0)
                {
                    return (false, null, response.ErrorMessage);
                }
                redisdb.Set(cacheKey, response, RedisUtil.RandomWeek());
                return (true, response, "ok");
            }
        }

        /// <summary>
        /// 批量获取用户基本信息
        /// </summary>
        /// <param name="UserIds">用户OpenIds ,多个用英文逗号隔开</param>
        /// <param name="Language"></param>
        /// <returns></returns>
        public async Task<(bool, CgibinUserInfoBatchGetResponse, string)> ExecuteCgibinUserInfoBatchGetAsync(string UserIds, string Language = "zh_CN")
        {
            if (string.IsNullOrEmpty(UserIds))
            {
                return (false, null, "用户OpenId不能为空");
            }
            string[] listUserId = UserIds.Split(",");
            IList<CgibinUserInfoBatchGetRequest.Types.User> UserList = new List<CgibinUserInfoBatchGetRequest.Types.User>();
            foreach (string userId in listUserId)
            {
                UserList.Add(new CgibinUserInfoBatchGetRequest.Types.User() { OpenId = userId, Language = Language });
            }
            string token = await ExecuteCgibinTokenAsync();
            var response = await client.ExecuteCgibinUserInfoBatchGetAsync(new CgibinUserInfoBatchGetRequest()
            {
                AccessToken = token,
                UserList = UserList
            });
            if (!response.IsSuccessful())
            {
                return (false, null, response.ErrorMessage);
            }
            return (true, response, response.ErrorMessage);
        }

        /// <summary>
        /// 创建小程序二维码
        /// </summary>
        /// <param name="Path">小程序跳转地址</param>
        /// <param name="Width">宽度 默认430</param>
        /// <returns></returns>
        public async Task<(bool, CgibinWxaappCreateWxaQrcodeResponse, string)> ExecuteCgibinWxaappCreateWxaQrcodeAsync(string Path, int Width = 430)
        {
            string token = await ExecuteCgibinTokenAsync();
            var response = await client.ExecuteCgibinWxaappCreateWxaQrcodeAsync(new CgibinWxaappCreateWxaQrcodeRequest()
            {
                AccessToken = token,
                Path = Path,
                Width = Width
            });
            if (!response.IsSuccessful())
            {
                return (false, null, response.ErrorMessage);
            }
            if (response.ErrorCode != 0)
            {
                return (false, null, response.ErrorMessage);
            }
            return (true, response, response.ErrorMessage);
        }

        #endregion

        #region 订阅

        /// <summary>
        /// 公众号订阅网址
        /// </summary>
        /// <param name="TemplateId"></param>
        /// <param name="RedirectUrl"></param>
        /// <returns></returns>
        public string ExecuteTemplateUrl(string TemplateId, string RedirectUrl)
        {
            return $"https://mp.weixin.qq.com/mp/subscribemsg?action=get_confirm&appid={this.appID}&scene=1000&template_id={TemplateId}&redirect_url={RedirectUrl}&reserved=test#wechat_redirect";
        }

        /// <summary>
        /// 公众号模板消息发送
        /// </summary> 
        /// <param name="UserOpenId">用户OpenId</param>
        /// <param name="TemplateId">模板Id</param>
        /// <param name="Data">模板数据</param>
        /// <param name="Page">公众号跳转  可选</param>
        /// <param name="AppletPage">小程序跳转  可选</param>
        /// <returns></returns>
        public async Task<(bool, long, string)> ExecuteCgibinMessageTemplateSendAsync(string UserOpenId, string TemplateId, IDictionary<string, CgibinMessageTemplateSendRequest.Types.DataItem> Data, string Page = "", string AppletPage = "")
        {
            string token = await ExecuteCgibinTokenAsync();
            var request = new CgibinMessageTemplateSendRequest()
            {
                AccessToken = token,
                ToUserOpenId = UserOpenId,
                TemplateId = TemplateId,
                Data = Data,
            };
            if (!string.IsNullOrEmpty(Page))
            {
                request.Url = Page;
            }
            if (!string.IsNullOrEmpty(AppletPage))
            {
                request.MiniProgram = new CgibinMessageTemplateSendRequest.Types.MiniProgram()
                {
                    AppId = this.appID,
                    PagePath = AppletPage
                };
            }
            var response = await client.ExecuteCgibinMessageTemplateSendAsync(request);
            if (!response.IsSuccessful())
            {
                return (false, 0, response.ErrorMessage);
            }
            return (response.ErrorCode == 0, response.MessageId, response.ErrorMessage);
        }

        /// <summary>
        /// 公众号模板信息列表
        /// </summary>
        /// <returns></returns>
        public async Task<(bool, CgibinTemplateGetAllPrivateTemplateResponse.Types.Template[], string)> ExecuteCgibinTemplateGetAllPrivateTemplateAsync()
        {
            string token = await ExecuteCgibinTokenAsync();
            var request = new CgibinTemplateGetAllPrivateTemplateRequest()
            {
                AccessToken = token
            };
            var response = await client.ExecuteCgibinTemplateGetAllPrivateTemplateAsync(request);
            if (!response.IsSuccessful())
            {
                return (false, null, response.ErrorMessage);
            }
            return (response.ErrorCode == 0, response.TemplateList, response.ErrorMessage);
        }

        /// <summary>
        /// 公众号订阅模板列表
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
        /// 公众号订阅消息发送
        /// </summary> 
        /// <param name="UserOpenId">用户OpenId</param>
        /// <param name="TemplateId">模板Id</param>
        /// <param name="Data">模板数据</param>
        /// <param name="Page">公众号跳转  可选</param>
        /// <param name="PagePath">小程序跳转  可选</param>
        /// <returns></returns>
        public async Task<(bool,  string)> ExecuteCgibinMessageSubscribeBusinessSendAsync(string UserOpenId, string TemplateId, IDictionary<string, CgibinMessageSubscribeBusinessSendRequest.Types.DataItem> Data, string Page = "", string PagePath = "")
        {
            string token = await ExecuteCgibinTokenAsync();
            var request = new CgibinMessageSubscribeBusinessSendRequest()
            {
                AccessToken = token,
                ToUserOpenId = UserOpenId,
                TemplateId = TemplateId,
                Data = Data,
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
            return (response.ErrorCode == 0,  response.ErrorMessage);
        }

        #endregion

        #region 智能接口

        /// <summary>
        /// 身份证OCR
        /// </summary>
        /// <param name="ImageUrl">图片url 地址 ，与ImageFileBytes二选一 </param>
        /// <param name="ImageFileBytes">图片字节数组，与ImageUrl二选一</param>
        /// <returns></returns>
        public async Task<(bool, CVOCRIdCardResponse, string)> ExecuteCVOCRIdCardAsync(string ImageUrl, byte[] ImageFileBytes)
        {
            string token = await ExecuteCgibinTokenAsync();
            var response = await client.ExecuteCVOCRIdCardAsync(new CVOCRIdCardRequest()
            {
                AccessToken = token,
                ImageUrl = ImageUrl,
                ImageFileBytes = ImageFileBytes
            });
            if (!response.IsSuccessful())
            {
                return (false, null, response.ErrorMessage);
            }
            if (response.ErrorCode != 0)
            {
                return (false, null, response.ErrorMessage);
            }
            return (true, response, response.ErrorMessage);
        }

        /// <summary>
        /// 银行卡OCR
        /// </summary>
        /// <param name="ImageUrl">图片url 地址 ，与ImageFileBytes二选一 </param>
        /// <param name="ImageFileBytes">图片字节数组，与ImageUrl二选一</param>
        /// <returns></returns>
        public async Task<(bool, CVOCRBankCardResponse, string)> ExecuteCVOCRBankCardAsync(string ImageUrl, byte[] ImageFileBytes)
        {
            string token = await ExecuteCgibinTokenAsync();
            var response = await client.ExecuteCVOCRBankCardAsync(new CVOCRBankCardRequest()
            {
                AccessToken = token,
                ImageUrl = ImageUrl,
                ImageFileBytes = ImageFileBytes
            });
            if (!response.IsSuccessful())
            {
                return (false, null, response.ErrorMessage);
            }
            if (response.ErrorCode != 0)
            {
                return (false, null, response.ErrorMessage);
            }
            return (true, response, response.ErrorMessage);
        }


        /// <summary>
        /// 行驶证 OCR 
        /// </summary>
        /// <param name="ImageUrl">图片url 地址 ，与ImageFileBytes二选一 </param>
        /// <param name="ImageFileBytes">图片字节数组，与ImageUrl二选一</param>
        /// <returns></returns>
        public async Task<(bool, CVOCRDrivingResponse, string)> ExecuteCVOCRDrivingAsync(string ImageUrl, byte[] ImageFileBytes)
        {
            string token = await ExecuteCgibinTokenAsync();
            var response = await client.ExecuteCVOCRDrivingAsync(new CVOCRDrivingRequest()
            {
                AccessToken = token,
                ImageUrl = ImageUrl,
                ImageFileBytes = ImageFileBytes
            });
            if (!response.IsSuccessful())
            {
                return (false, null, response.ErrorMessage);
            }
            if (response.ErrorCode != 0)
            {
                return (false, null, response.ErrorMessage);
            }
            return (true, response, response.ErrorMessage);
        }


        /// <summary>
        /// 驾驶证 OCR 
        /// </summary>
        /// <param name="ImageUrl">图片url 地址 ，与ImageFileBytes二选一 </param>
        /// <param name="ImageFileBytes">图片字节数组，与ImageUrl二选一</param>
        /// <returns></returns>
        public async Task<(bool, CVOCRDrivingLicenseResponse, string)> ExecuteCVOCRDrivingLicenseAsync(string ImageUrl, byte[] ImageFileBytes)
        {
            string token = await ExecuteCgibinTokenAsync();
            var response = await client.ExecuteCVOCRDrivingLicenseAsync(new CVOCRDrivingLicenseRequest()
            {
                AccessToken = token,
                ImageUrl = ImageUrl,
                ImageFileBytes = ImageFileBytes
            });
            if (!response.IsSuccessful())
            {
                return (false, null, response.ErrorMessage);
            }
            if (response.ErrorCode != 0)
            {
                return (false, null, response.ErrorMessage);
            }
            return (true, response, response.ErrorMessage);
        }

        /// <summary>
        /// 营业执照 OCR
        /// </summary>
        /// <param name="ImageUrl">图片url 地址 ，与ImageFileBytes二选一 </param>
        /// <param name="ImageFileBytes">图片字节数组，与ImageUrl二选一</param>
        /// <returns></returns>
        public async Task<(bool, CVOCRBusinessLicenseResponse, string)> ExecuteCVOCRBusinessLicenseAsync(string ImageUrl, byte[] ImageFileBytes)
        {
            string token = await ExecuteCgibinTokenAsync();
            var response = await client.ExecuteCVOCRBusinessLicenseAsync(new CVOCRBusinessLicenseRequest()
            {
                AccessToken = token,
                ImageUrl = ImageUrl,
                ImageFileBytes = ImageFileBytes
            });
            if (!response.IsSuccessful())
            {
                return (false, null, response.ErrorMessage);
            }
            if (response.ErrorCode != 0)
            {
                return (false, null, response.ErrorMessage);
            }
            return (true, response, response.ErrorMessage);
        }

        /// <summary>
        /// 通用印刷体 OCR
        /// </summary>
        /// <param name="ImageUrl">图片url 地址 ，与ImageFileBytes二选一 </param>
        /// <param name="ImageFileBytes">图片字节数组，与ImageUrl二选一</param>
        /// <returns></returns>
        public async Task<(bool, CVOCRCommonResponse, string)> ExecuteCVOCRCommonAsync(string ImageUrl, byte[] ImageFileBytes)
        {
            string token = await ExecuteCgibinTokenAsync();
            var response = await client.ExecuteCVOCRCommonAsync(new CVOCRCommonRequest()
            {
                AccessToken = token,
                ImageUrl = ImageUrl,
                ImageFileBytes = ImageFileBytes
            });
            if (!response.IsSuccessful())
            {
                return (false, null, response.ErrorMessage);
            }
            if (response.ErrorCode != 0)
            {
                return (false, null, response.ErrorMessage);
            }
            return (true, response, response.ErrorMessage);
        }

        /// <summary>
        /// 车牌识别 OCR
        /// </summary>
        /// <param name="ImageUrl">图片url 地址 ，与ImageFileBytes二选一 </param>
        /// <param name="ImageFileBytes">图片字节数组，与ImageUrl二选一</param>
        /// <returns></returns>
        public async Task<(bool, CVOCRPlateNumberResponse, string)> ExecuteCVOCRPlateNumberAsync(string ImageUrl, byte[] ImageFileBytes)
        {
            string token = await ExecuteCgibinTokenAsync();
            var response = await client.ExecuteCVOCRPlateNumberAsync(new CVOCRPlateNumberRequest()
            {
                AccessToken = token,
                ImageUrl = ImageUrl,
                ImageFileBytes = ImageFileBytes
            });
            if (!response.IsSuccessful())
            {
                return (false, null, response.ErrorMessage);
            }
            if (response.ErrorCode != 0)
            {
                return (false, null, response.ErrorMessage);
            }
            return (true, response, response.ErrorMessage);
        }

        #endregion

        #region 自定义菜单

        /// <summary>
        /// 创建菜单
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public async Task<(bool, string)> CreateMenu(List<CgibinMenuCreateRequest.Types.Button> list)
        {
            var response = await client.ExecuteCgibinMenuCreateAsync(new CgibinMenuCreateRequest()
            {
                ButtonList = list,
                AccessToken = await ExecuteCgibinTokenAsync()
            });
            if (response.IsSuccessful())
            {
                return (true, "ok");
            }
            return (false, response.ErrorMessage);
        }

        /// <summary>
        /// 获取菜单
        /// </summary>
        /// <returns></returns>
        public async Task<(CgibinGetCurrentSelfMenuInfoResponse.Types.Menu, string)> GetMenu()
        {
            var response = await client.ExecuteCgibinGetCurrentSelfMenuInfoAsync(new CgibinGetCurrentSelfMenuInfoRequest()
            {
                AccessToken = await ExecuteCgibinTokenAsync()
            });
            if (response.IsSuccessful())
            {
                return (response.SelfMenu, "ok");
            }
            return (null, response.ErrorMessage);
        }

        /// <summary>
        /// 删除菜单
        /// </summary>
        /// <returns></returns>
        public async Task<(bool, string)> DeleteMenu()
        {
            var response = await client.ExecuteCgibinMenuDeleteAsync(new CgibinMenuDeleteRequest()
            {
                AccessToken = await ExecuteCgibinTokenAsync()
            });
            if (response.IsSuccessful())
            {
                return (true, "ok");
            }
            return (false, response.ErrorMessage);
        }

        /// <summary>
        /// 获取个性化菜单
        /// </summary>
        /// <returns></returns>
        public async Task<(CgibinMenuGetResponse.Types.ConditionalMenu[], CgibinMenuGetResponse.Types.Menu, string)> GetCustomMenu()
        {
            var response = await client.ExecuteCgibinMenuGetAsync(new CgibinMenuGetRequest()
            {
                AccessToken = await ExecuteCgibinTokenAsync()
            });
            if (response.IsSuccessful())
            {
                return (response.ConditionalMenuList, response.Menu, "ok");
            }
            return (null, null, response.ErrorMessage);
        }

        /// <summary>
        /// 创建个性化菜单
        /// </summary>
        /// <param name="list"></param>
        /// <param name="matchrule"></param>
        /// <returns></returns>
        public async Task<(bool, int, string)> CreateCustomMenu(List<CgibinMenuAddConditionalRequest.Types.Button> list, CgibinMenuAddConditionalRequest.Types.MatchRule matchrule)
        {
            var response = await client.ExecuteCgibinMenuAddConditionalAsync(new CgibinMenuAddConditionalRequest()
            {
                ButtonList = list,
                MatchRule = matchrule,
                AccessToken = await ExecuteCgibinTokenAsync()
            });
            if (response.IsSuccessful())
            {
                return (true, response.MenuId, "ok");
            }
            return (false, 0, response.ErrorMessage);
        }

        /// <summary>
        /// 删除个性化菜单
        /// </summary>
        /// <param name="menuid"></param>
        /// <returns></returns>
        public async Task<(bool, string)> DeleteCustomMenu(int menuid)
        {
            var response = await client.ExecuteCgibinMenuDeleteConditionalAsync(new CgibinMenuDeleteConditionalRequest()
            {
                MenuId = menuid,
                AccessToken = await ExecuteCgibinTokenAsync()
            });
            if (response.IsSuccessful())
            {
                return (true, "ok");
            }
            return (false, response.ErrorMessage);
        }

        #endregion

        #region 卡券

        /// <summary>
        /// 创建卡券
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public async Task<(bool, string, string)> CreateCard(CardCreateRequest.Types.Card info)
        {
            var response = await client.ExecuteCardCreateAsync(new CardCreateRequest()
            {
                Card = info,
                AccessToken = await ExecuteCgibinTokenAsync()
            });
            if (response.IsSuccessful())
            {
                return (true, response.CardId, "ok");
            }
            return (false, "", response.ErrorMessage);
        }

        /// <summary>
        /// 查看卡券详情
        /// </summary>
        /// <param name="CardId"></param>
        /// <returns></returns>
        public async Task<(bool, CardGetResponse.Types.Card, string)> InfoCard(string CardId)
        {
            var response = await client.ExecuteCardGetAsync(new CardGetRequest()
            {
                CardId = CardId,
                AccessToken = await ExecuteCgibinTokenAsync()
            });
            if (response.IsSuccessful())
            {
                return (true, response.Card, "ok");
            }
            return (false, null, response.ErrorMessage);
        }

        /// <summary>
        /// 批量查询卡券列表
        /// </summary>
        /// <param name="Offset"></param>
        /// <param name="Limit"></param>
        /// <returns></returns>
        public async Task<(bool, string[], string)> BatchCard(int Offset, int Limit)
        {
            var response = await client.ExecuteCardBatchGetAsync(new CardBatchGetRequest()
            {
                Offset = Offset,
                Limit = Limit,
                AccessToken = await ExecuteCgibinTokenAsync()
            });
            if (response.IsSuccessful())
            {
                return (true, response.CardIdList, "ok");
            }
            return (false, null, response.ErrorMessage);
        }

        /// <summary>
        /// 批量查询卡券列表
        /// </summary>
        /// <param name="info"></param> 
        /// <returns></returns>
        public async Task<(bool, string)> UpdateCard(CardUpdateRequest info)
        {
            info.AccessToken = await ExecuteCgibinTokenAsync();
            var response = await client.ExecuteCardUpdateAsync(info);
            if (response.IsSuccessful())
            {
                return (true, "ok");
            }
            return (false, response.ErrorMessage);
        }

        /// <summary>
        /// 批量查询卡券列表
        /// </summary>
        /// <param name="info"></param> 
        /// <returns></returns>
        public async Task<(bool, string)> ModifyStockCard(string CardId, int IncreaseStockValue, int ReduceStockValue)
        {
            var response = await client.ExecuteCardModifyStockAsync(new CardModifyStockRequest()
            {
                CardId = CardId,
                IncreaseStockValue = IncreaseStockValue,
                ReduceStockValue = ReduceStockValue,
                AccessToken = await ExecuteCgibinTokenAsync()
            });
            if (response.IsSuccessful())
            {
                return (true, "ok");
            }
            return (false, response.ErrorMessage);
        }

        /// <summary>
        /// 删除卡券
        /// </summary>
        /// <param name="CardId"></param> 
        /// <returns></returns>
        public async Task<(bool, string)> DeleteCard(string CardId)
        {
            var response = await client.ExecuteCardDeleteAsync(new CardDeleteRequest()
            {
                CardId = CardId,
                AccessToken = await ExecuteCgibinTokenAsync()
            });
            if (response.IsSuccessful())
            {
                return (true, "ok");
            }
            return (false, response.ErrorMessage);
        }

        /// <summary>
        /// 设置买单
        /// </summary>
        /// <param name="CardId"></param>
        /// <param name="IsOpen"></param>
        /// <returns></returns>
        public async Task<(bool, string)> SetPayCellCard(string CardId, bool IsOpen)
        {
            var response = await client.ExecuteCardPayCellSetAsync(new CardPayCellSetRequest()
            {
                CardId = CardId,
                IsOpen = IsOpen,
                AccessToken = await ExecuteCgibinTokenAsync()
            });
            if (response.IsSuccessful())
            {
                return (true, "ok");
            }
            return (false, response.ErrorMessage);
        }

        /// <summary>
        /// 设置自助核销
        /// </summary>
        /// <param name="info"></param> 
        /// <returns></returns>
        public async Task<(bool, string)> SelfConsumeCellCard(CardSelfConsumeCellSetRequest info)
        {
            info.AccessToken = await ExecuteCgibinTokenAsync();
            var response = await client.ExecuteCardSelfConsumeCellSetAsync(info);
            if (response.IsSuccessful())
            {
                return (true, "ok");
            }
            return (false, response.ErrorMessage);
        }

        /// <summary>
        /// 创建二维码卡券
        /// </summary>
        /// <param name="info"></param> 
        /// <returns></returns>
        public async Task<(bool, CardQrcodeCreateResponse, string)> CreateQrCodeCard(CardQrcodeCreateRequest info)
        {
            info.AccessToken = await ExecuteCgibinTokenAsync();
            var response = await client.ExecuteCardQrcodeCreateAsync(info);
            if (response.IsSuccessful())
            {
                return (true, response, "ok");
            }
            return (false, null, response.ErrorMessage);
        }

        #endregion

    }
}
