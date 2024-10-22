

using OpenAI.Chat;
using OpenAI.Images;

namespace BlogApp.API.Repositories.Interface
{
    public interface IOpenAiRepository 
    {
        Task<ChatCompletion> GetChatCompletion(string systemMessage, string userMessage);

        Task<GeneratedImage> GetImageCompletion(string userMessage);
    }
}
