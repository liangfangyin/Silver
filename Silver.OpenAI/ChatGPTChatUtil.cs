using OpenAI_API;
using OpenAI_API.Chat;
using OpenAI_API.Completions;
using OpenAI_API.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Silver.OpenAI
{
    public class ChatGPTChatUtil
    {
        private OpenAIAPI openAIAPI;
        private Conversation openChat;

        public ChatGPTChatUtil(string apiKeys)
        {
            openAIAPI = new OpenAIAPI(apiKeys);
            openChat = openAIAPI.Chat.CreateConversation();
        }

        public ChatGPTChatUtil(string apiKeys, ChatRequest defaultChatRequestArgs)
        {
            openAIAPI = new OpenAIAPI(apiKeys);
            openChat = openAIAPI.Chat.CreateConversation(defaultChatRequestArgs);
        }
          
        /// <summary>
        /// 通过API列出所有型号
        /// </summary>
        /// <returns></returns>
        public List<Model> GetAllModels()
        {
            return openAIAPI.Models.GetModelsAsync().Result;
        }


        /// <summary>
        /// 创建一个角色为<see cref=“ChatMessageRole.System”/>的聊天室并将其附加到聊天室。系统消息有助于设置助手的行为。
        /// </summary>
        /// <param name="content"></param>
        public void AppendSystemMessage(string content)
        {
            openChat.AppendSystemMessage(content);
        }

        /// <summary>
        /// 发送信息并返回信息
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public async Task<string> AppendUserInputResultMessAge(string content)
        {
            openChat.AppendUserInput(content);
            return await openChat.GetResponseFromChatbotAsync();
        }

        /// <summary>
        /// 发送信息并返回信息
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public async Task AppendUserInputResultMessAgeStream(string content, Action<string> resultHandler)
        {
            var listReset = new List<string>();
            openChat.AppendUserInput(content);
            await openChat.StreamResponseFromChatbotAsync(resultHandler);
        }

        /// <summary>
        /// 指定角色发送信息，并返回信息
        /// </summary>
        /// <param name="role"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public async Task<string> AppendMessage(ChatMessageRole role, string content)
        {
            openChat.AppendMessage(role, content);
            return await openChat.GetResponseFromChatbotAsync();
        }

        /// <summary>
        /// 指定角色发送信息，并返回信息
        /// </summary>
        /// <param name="role"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public async Task AppendMessageStream(ChatMessageRole role, string content, Action<string> resultHandler)
        {
            var listReset = new List<string>();
            openChat.AppendMessage(role, content);
            await openChat.StreamResponseFromChatbotAsync(resultHandler);
        }

    }
}
