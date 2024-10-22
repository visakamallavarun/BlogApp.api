using Azure.AI.OpenAI;
using Azure;
using OpenAI.Chat;
using BlogApp.API.Repositories.Interface;
using OpenAI.Images;

namespace BlogApp.API.Repositories.Implementation
{
    public class OpenAiRepository: IOpenAiRepository
    {
        private readonly AzureOpenAIClient _openAIClient;

        public OpenAiRepository(IConfiguration configuration)
        {
            string key = configuration["AzureOpenAI:ApiKey"];
            string endpoint = configuration["AzureOpenAI:EndpointUrl"];

            AzureKeyCredential credential = new  AzureKeyCredential  (key);
            _openAIClient= new(new Uri(endpoint), credential);
        }

        public async Task<ChatCompletion> GetChatCompletion(string systemMessage, string userMessage)
        {
            var chatMessages = new ChatMessage[]
            {
            new SystemChatMessage(systemMessage),
            new UserChatMessage(userMessage),
            };

            var chatClient = _openAIClient.GetChatClient("demo-Ais");

            var completion=await chatClient.CompleteChatAsync(chatMessages);

            return completion;
        }

        public async Task<GeneratedImage> GetImageCompletion(string userMessage)
        {

            var imageClient = _openAIClient.GetImageClient("Dalle3");

            var imageResult = await imageClient.GenerateImageAsync(userMessage, new()
            {
                Quality = GeneratedImageQuality.Standard,
                Size = GeneratedImageSize.W1024xH1024,
                ResponseFormat = GeneratedImageFormat.Uri
            });
            var image = imageResult.Value;

            return image;
        }
    }
}
