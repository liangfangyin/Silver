using OpenAI_API;
using OpenAI_API.Completions;
using OpenAI_API.Models;
using Silver.OpenAI.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Silver.OpenAI
{
    public class ChatGPTCompletionUtil
    {
        private OpenAIAPI openAIAPI;

        public ChatGPTCompletionUtil(string apiKeys)
        {
            openAIAPI = new OpenAIAPI(apiKeys);
        }

        /// <summary>
        /// 全文全部返回
        /// </summary>
        /// <param name="prompt"></param>
        /// <returns></returns>
        public List<Choice> CreateCompletionsAsync(string prompt)
        {
            var result = openAIAPI.Completions.CreateCompletionsAsync(new CompletionRequest(prompt, model: Model.CurieText, temperature: 1, max_tokens: 50, numOutputs:1),1).Result;
            return result.Completions;
        }

        /// <summary>
        /// 全文全部返回
        /// </summary>
        /// <param name="prompt"></param>
        /// <returns></returns>
        public List<Choice> CreateCompletionsAsync(CompletionModel completion)
        {
            CompletionRequest request = new CompletionRequest();
            request.Temperature = completion.Temperature;
            request.MaxTokens = completion.MaxTokens;
            if (!string.IsNullOrEmpty(completion.Prompt))
            {
                request.Prompt = completion.Prompt;
            }
            if (completion.MultiplePrompts != null)
            {
                request.MultiplePrompts = completion.MultiplePrompts;
            }
            request.Model = completion.Model;
            var result = openAIAPI.Completions.CreateCompletionsAsync(request, completion.numOutputs).Result;
            return result.Completions;
        }

    }
}
