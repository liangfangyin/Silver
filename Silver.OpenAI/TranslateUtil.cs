using GTranslate.Translators;
using System.Threading.Tasks;

namespace Silver.OpenAI
{
    /// <summary>
    /// 翻译
    /// </summary>
    public class TranslateUtil
    {
         
        /// <summary>
        /// 微软翻译
        /// </summary>
        /// <param name="content"></param>
        /// <param name="toLanguage"></param>
        /// <param name="fromLanguage"></param>
        /// <returns></returns>
        public static async Task<string> TranslateAsync(string content, string toLanguage = "zh-CN", string? fromLanguage = null)
        {
            var translator = new MicrosoftTranslator();
            var result = await translator.TranslateAsync(content, toLanguage, fromLanguage);
            return result.Translation;
        }
          

    }
}
