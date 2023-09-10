using OpenAI_API;
using OpenAI_API.Images;
using System.Collections.Generic;

namespace Silver.OpenAI
{
    public class ChatGPTImageUtil
    {
        private OpenAIAPI openAIAPI;

        public ChatGPTImageUtil(string apiKeys)
        {
            openAIAPI = new OpenAIAPI(apiKeys);
        }

        /// <summary>
        /// 根据提示语创建图片
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public List<Data> CreateImageGenerations(string content)
        {
            var result= openAIAPI.ImageGenerations.CreateImageAsync(new ImageGenerationRequest(content, size: ImageSize._256, responseFormat: ImageResponseFormat.B64_json)).Result;
            return result.Data;
        }

    }
}
