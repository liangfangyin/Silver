using Silver.Basic;
using Silver.Cache;
using Silver.WeChat.Model;
using SKIT.FlurlHttpClient.Wechat.Work;
using SKIT.FlurlHttpClient.Wechat.Work.Models;
using System;
using System.Threading.Tasks;

namespace Ydhp.Lib.WeChat
{
    public class WeWorkClient
    {

        private WechatWorkClientOptions options;
        private WechatWorkClient client;

        public WeWorkClient()
        {
            options = new WechatWorkClientOptions()
            {
                CorpId = ConfigurationUtil.GetSection("WeWork:CorpId"),
                AgentId = ConfigurationUtil.GetSection("WeWork:AgentId").ToInt(),
                AgentSecret= ConfigurationUtil.GetSection("WeWork:AgentSecret"),
                ProviderSecret = ConfigurationUtil.GetSection("WeWork:ProviderSecret"),
                SuiteId = ConfigurationUtil.GetSection("WeWork:SuiteId"),
                SuiteSecret = ConfigurationUtil.GetSection("WeWork:SuiteSecret"),
                PushEncodingAESKey = ConfigurationUtil.GetSection("WeWork:PushEncodingAESKey"),
                PushToken = ConfigurationUtil.GetSection("WeWork:PushToken"),
            };
            client = new WechatWorkClient(options);
        }

        public WeWorkClient(string CorpId, string AgentSecret, int AgentId = 0,string ProviderSecret="",string SuiteId="",string SuiteSecret="",string PushEncodingAESKey="",string PushToken="")
        {
            options = new WechatWorkClientOptions()
            {
                CorpId = CorpId,
                AgentId = AgentId,
                AgentSecret = AgentSecret,
                ProviderSecret = ProviderSecret,
                SuiteId = SuiteId,
                SuiteSecret = SuiteSecret,
                PushEncodingAESKey = PushEncodingAESKey,
                PushToken = PushToken,
            };
            client = new WechatWorkClient(options);
        }

        /// <summary>
        /// 获取token
        /// </summary>
        /// <returns></returns>
        public async Task<string> GetTokenAsync()
        {
            string cacheKey = "wechat:work:token";
            var redisdb = new RedisUtil().GetClient();
            if (redisdb.Exists(cacheKey))
            {
                return redisdb.Get(cacheKey);
            }
            var tokenResult = await client.ExecuteCgibinGetTokenAsync(new SKIT.FlurlHttpClient.Wechat.Work.Models.CgibinGetTokenRequest());
            if (tokenResult.IsSuccessful() == false)
            {
                return "";
            }
            redisdb.Set(cacheKey, tokenResult.AccessToken);
            return tokenResult.AccessToken;
        }


        /// <summary>
        /// 通过网页code获取用户ID和用户票据
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public async Task<CgibinUserGetUserInfoResponse> GetUserInfoTicker(string code)
        {
            string token = await GetTokenAsync();
            var userIdResult = await client.ExecuteCgibinUserGetUserInfoAsync(new SKIT.FlurlHttpClient.Wechat.Work.Models.CgibinUserGetUserInfoRequest()
            {
                AccessToken = token,
                Code = code
            });
            return userIdResult;
        }
         
        /// <summary>
        /// 获取用户信息-老应用使用
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<CgibinUserGetResponse> GetCgibinUserInfoAll(string userId)
        {
            string token = await GetTokenAsync();
            var userIdResult = await client.ExecuteCgibinUserGetAsync(new CgibinUserGetRequest()
            {
                AccessToken = token,
                UserId = userId
            });
            return userIdResult;
        }

        /// <summary>
        /// userid转openid
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<CgibinUserConvertToOpenIdResponse> GetCgibinUserConvertToOpenId(string userId)
        {
            string token = await GetTokenAsync();
            var userIdResult = await client.ExecuteCgibinUserConvertToOpenIdAsync(new CgibinUserConvertToOpenIdRequest()
            {
                AccessToken = token,
                UserId = userId
            });
            return userIdResult;
        }

        /// <summary>
        /// 获取wx.config 参数和密钥
        /// </summary> 
        /// <param name="url"></param> 
        /// <returns></returns>
        public async Task<CgibinTicketGetSignatureResponse> GetCgibinWxConfig(string url)
        {
            string jsapiTicket = "";
            string cacheKey = "wechat:work:jsapi_ticket";
            var redisdb = new RedisUtil().GetClient();
            if (redisdb.Exists(cacheKey))
            {
                jsapiTicket = redisdb.Get(cacheKey);
            }
            else
            {
                string token = await GetTokenAsync();
                var tickerResult = await client.ExecuteCgibinGetJsapiTicketAsync(new CgibinGetJsapiTicketRequest()
                {
                    AccessToken = token
                });
                if (tickerResult.IsSuccessful() == false)
                {
                    return new CgibinTicketGetSignatureResponse()
                    {
                        IsSuccess = tickerResult.IsSuccessful(),
                        ErrorMessage = tickerResult.ErrorMessage
                    };
                }
                jsapiTicket = tickerResult.Ticket;
                redisdb.Set(cacheKey, jsapiTicket, 3600);
            }
            var dictResult = client.GenerateParametersForJSSDKConfig(jsapiTicket, url);
            CgibinTicketGetSignatureResponse response = new CgibinTicketGetSignatureResponse();
            response.IsSuccess = true;
            response.Signature = dictResult["signature"];
            response.NonceStr = dictResult["nonceStr"];
            response.AppId = dictResult["appId"];
            response.Timestamp = Convert.ToInt64(dictResult["timestamp"]);
            response.ErrorMessage = "成功";
            return response;
        }

        /// <summary>
        /// 获取wx.agentconfig 参数和密钥
        /// </summary> 
        /// <param name="url"></param> 
        /// <returns></returns>
        public async Task<CgibinTicketGetSignatureAgentResponse> GetCgibinWxAgentConfig(string url)
        {
            string jsapiTicket = "";
            string cacheKey = "wechat:work:jsapi_ticket_agent";
            var redisdb = new RedisUtil().GetClient();
            if (redisdb.Exists(cacheKey))
            {
                jsapiTicket = redisdb.Get(cacheKey);
            }
            else
            {
                string token = await GetTokenAsync();
                var tickerResult = await client.ExecuteCgibinTicketGetAsync(new CgibinTicketGetRequest()
                {
                    AccessToken = token,
                    Type = "agent_config"
                });
                if (tickerResult.IsSuccessful() == false)
                {
                    return new CgibinTicketGetSignatureAgentResponse()
                    {
                        IsSuccess = tickerResult.IsSuccessful(),
                        ErrorMessage = tickerResult.ErrorMessage
                    };
                }
                jsapiTicket = tickerResult.Ticket;
                redisdb.Set(cacheKey, jsapiTicket, 3600);
            }
            var dictResult = client.GenerateParametersForJSSDKAgentConfig(jsapiTicket, url);
            CgibinTicketGetSignatureAgentResponse response = new CgibinTicketGetSignatureAgentResponse();
            response.IsSuccess = true;
            response.Signature = dictResult["signature"];
            response.NonceStr = dictResult["nonceStr"];
            response.CorpId = dictResult["corpid"];
            response.AgentId = dictResult["agentid"];
            response.Timestamp = Convert.ToInt64(dictResult["timestamp"]);
            response.ErrorMessage = "成功";
            return response;
        }





    }
}
