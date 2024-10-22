using BlogApp.API.Models.DTO;
using BlogApp.API.Repositories.Interface;
using Microsoft.AspNetCore.Mvc;

namespace BlogApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChatController : ControllerBase
    {
        private readonly IOpenAiRepository openAiRepository;
        public ChatController(IOpenAiRepository openAiRepository)
        {
            this.openAiRepository = openAiRepository;
        }

        [HttpPost]
        public async Task<IActionResult> Chat([FromBody] ChatRequestDto request)
        {
            var completion= await openAiRepository.GetChatCompletion(request.SystemMessage, request.UserMessage);

            return Ok(completion.Content[0].Text);
        }
    }
}
