using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Silver.OpenAI.Models
{
    public class CompletionModel
    {
        /// <summary>
        /// 提问文本
        /// </summary>
        public string Prompt { get; set; }

        /// <summary>
        /// token数量
        /// </summary>
        public int MaxTokens { get; set; } = 5;

        /// <summary>
		/// 使用什么样的采样温度。更高的值意味着该模型将承担更多的风险。对于更有创意的应用程序，请尝试0.9，对于具有明确答案的应用程序请尝试0（argmax采样）。通常建议使用此项或<see cref=“TopP”/>，但不能同时使用两者。
		/// </summary> 
        public double? Temperature { get; set; } = 0.1;

        /// <summary>
        /// 提问文本列表
        /// </summary>
        public string[] MultiplePrompts { get; set; }

        /// <summary>
        /// 模型
        /// </summary>
        public string Model { get; set; } = OpenAI_API.Models.Model.DavinciText;

        /// <summary>
        /// 返回结果条数
        /// </summary>
        public int numOutputs { get; set; } = 1;

    }
}
