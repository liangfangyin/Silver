using Silver.Basic;
using Silver.ThirdApi.Model;
using System;
using System.Collections.Generic; 
using TencentCloud.Common;
using TencentCloud.Common.Profile;
using TencentCloud.Tts.V20190823;
using TencentCloud.Tts.V20190823.Models;

namespace Silver.ThirdApi
{
    public class Tencent
    {
        /// <summary>
        /// 文字合成语音
        /// </summary>
        /// <param name="value">合成语音的源文本</param>
        /// <param name="Volume">音量大小,默认为0</param>
        /// <param name="Speed">语速，范围：[-2，2]，分别对应不同语速，默认为0</param>
        /// <param name="VoiceType">普通音色:https://cloud.tencent.com/document/product/1073/37995 </param>
        /// <param name="PrimaryLanguage">主语言类型：1 - 中文（默认）  2 - 英文</param>
        /// <param name="SampleRate">/音频采样率：16000：16k（默认）  8000：8k</param>
        /// <returns></returns>
        public static (bool, TencentTTS, string) ShortTextToSpeend(string value, float Volume = 0, float Speed = 0, long VoiceType = 1001, long PrimaryLanguage = 1, ulong SampleRate = 16000)
        {
            try
            {
                Credential cred = new Credential
                {
                    SecretId = ConfigurationUtil.GetSection("Tencent:TTS:SecretId"),
                    SecretKey = ConfigurationUtil.GetSection("Tencent:TTS:SecretKey")
                };
                ClientProfile clientProfile = new ClientProfile();
                HttpProfile httpProfile = new HttpProfile();
                httpProfile.Endpoint = ("tts.tencentcloudapi.com");
                clientProfile.HttpProfile = httpProfile;

                TtsClient client = new TtsClient(cred, "ap-shanghai", clientProfile);
                TextToVoiceRequest req = new TextToVoiceRequest();
                //合成语音的源文本
                req.Text = value;
                //一次请求对应一个SessionId
                req.SessionId = Guid.NewGuid().ToString();
                //音量大小，范围：[0，10]，分别对应11个等级的音量，默认为0
                req.Volume = Volume;
                //语速，范围：[-2，2]，分别对应不同语速，默认为0
                req.Speed = Speed;
                //项目id，用户自定义，默认为0。
                req.ProjectId = 0;
                //模型类型，1-默认模型
                req.ModelType = 1;
                //普通音色:https://cloud.tencent.com/document/product/1073/37995
                req.VoiceType = VoiceType;
                //主语言类型：1 - 中文（默认）  2 - 英文
                req.PrimaryLanguage = PrimaryLanguage;
                //音频采样率：16000：16k（默认）  8000：8k
                req.SampleRate = SampleRate;
                //返回音频格式，可取值：wav（默认），mp3，pcm
                req.Codec = "mp3";
                TextToVoiceResponse resp = client.TextToVoiceSync(req);
                string result = AbstractModel.ToJsonString(resp);
                return (true, result.JsonToObject<TencentTTS>(), "success");
            }
            catch (Exception e)
            {
                return (false, new TencentTTS(), e.Message);
            }
        }

        /// <summary>
        /// 长文字合成语音
        /// </summary>
        /// <param name="value">合成语音的源文本</param>
        /// <param name="Volume">音量大小,默认为0</param>
        /// <param name="Speed">语速，范围：[-2，2]，分别对应不同语速，默认为0</param>
        /// <param name="VoiceType">普通音色:https://cloud.tencent.com/document/product/1073/37995 </param>
        /// <param name="PrimaryLanguage">主语言类型：1 - 中文（默认）  2 - 英文</param>
        /// <param name="SampleRate">/音频采样率：16000：16k（默认）  8000：8k</param>
        /// <returns></returns>
        public static (bool, List<TencentTTS>, string) LongTextToSpeend(string value, float Volume = 0, float Speed = 0, long VoiceType = 1001, long PrimaryLanguage = 1, ulong SampleRate = 16000)
        {
            if (value.Length <= 0)
            {
                return (false, default(List<TencentTTS>), "文字不能为空");
            }
            int recoder = value.Length % 100 == 0 ? Convert.ToInt32(value.Length / 100) : Convert.ToInt32(value.Length / 100) + 1;
            List<string> list_value = new List<string>();
            for (int i = 0; i < recoder; i++)
            {
                if ((i + 1) * 100 > value.Length)
                {
                    list_value.Add(value.Substring(i * 100));
                }
                else
                {
                    list_value.Add(value.Substring(i * 100, 100));
                }
            }
            List<TencentTTS> list_tts = new List<TencentTTS>();
            foreach (var item in list_value)
            {
                var result = ShortTextToSpeend(item, Volume, Speed, VoiceType, PrimaryLanguage, SampleRate);
                if (result.Item1 == false)
                {
                    return (false, default(List<TencentTTS>), result.Item3);
                }
                list_tts.Add(result.Item2);
            }
            return (true, list_tts, "success");
        }

    }
}


 
