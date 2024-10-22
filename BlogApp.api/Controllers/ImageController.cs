using BlogApp.API.Repositories.Interface;
using Microsoft.AspNetCore.Mvc;

namespace BlogApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        private readonly IOpenAiRepository openAiRepository;
        public ImageController(IOpenAiRepository openAiRepository)
        {
            this.openAiRepository = openAiRepository;
        }

        [HttpPost]
        public async Task<IActionResult> CreateImage([FromBody] string request)
        {
            var image = await openAiRepository.GetImageCompletion(request);
            return Ok(image.ImageUri.AbsoluteUri);
        }
    }
}
