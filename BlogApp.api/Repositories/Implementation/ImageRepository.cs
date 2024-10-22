using BlogApp.API.Data;
using BlogApp.API.Models.Domain;
using BlogApp.API.Repositories.Interface;
using Microsoft.EntityFrameworkCore;
using OpenAI.Images;

namespace BlogApp.API.Repositories.Implementation
{
    public class ImageRepository : IImageRepository
    {
        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly ApplicationDbContext dbContext;
        private readonly IOpenAiRepository openAiRepository;

        public ImageRepository(IOpenAiRepository openAiRepository,
            IWebHostEnvironment webHostEnvironment,
            IHttpContextAccessor httpContextAccessor,
            ApplicationDbContext dbContext)
        {
            this.openAiRepository = openAiRepository;
            this.webHostEnvironment = webHostEnvironment;
            this.httpContextAccessor = httpContextAccessor;
            this.dbContext = dbContext;
        }

        public async Task<IEnumerable<BlogImage>> GetAll()
        {
            return await dbContext.BlogImages.ToListAsync();
        }

        public async Task<BlogImage> Upload(IFormFile? file, BlogImage blogImage)
        {
            string defaultFileName = "default.png";

            if (file.FileName.Equals(defaultFileName, StringComparison.OrdinalIgnoreCase) || file==null) {
                GeneratedImage image =await openAiRepository.GetImageCompletion(blogImage.Title);
                if (image.ImageUri != null)
                {
                    blogImage.Url=image.ImageUri.AbsoluteUri;
                }
            }
            else
            {
                var localPath = Path.Combine(webHostEnvironment.ContentRootPath, "Images", $"{blogImage.FileName}{blogImage.FileExtension}");
                using var stream = new FileStream(localPath, FileMode.Create);
                await file.CopyToAsync(stream);
                var httpRequest = httpContextAccessor.HttpContext.Request;
                var urlPath = $"{httpRequest.Scheme}://{httpRequest.Host}{httpRequest.PathBase}/Images/{blogImage.FileName}{blogImage.FileExtension}";

                blogImage.Url = urlPath;
            }

            await dbContext.BlogImages.AddAsync(blogImage);
            await dbContext.SaveChangesAsync();

            return blogImage;
        }

    }
}
