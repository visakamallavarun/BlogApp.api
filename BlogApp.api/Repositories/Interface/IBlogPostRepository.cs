using BlogApp.API.Models.Domain;
using BlogApp.API.Models.DTO;

namespace BlogApp.API.Repositories.Interface
{
    public interface IBlogPostRepository
    {
        Task<BlogPost> CreateAsync(BlogPost blogPost);

        Task<IEnumerable<BlogPost>> GetAllAsync();

        Task<string> ContentGeneration(GenerateContentDto generateContentDto);

        Task<Category> AssignCategories(string content);

        Task<BlogPost?> GetByIdAsync(Guid id);

        Task<IEnumerable<BlogPost>> GetByCategoriesIdAsync(Guid id);

        Task<BlogPost?> GetByUrlHandleAsync(string urlHandle);

        Task<BlogPost?> UpdateAsync(BlogPost blogPost);

        Task<BlogPost?> UpdateLanguageAsync(string language, BlogPost blogPost);

        Task<BlogPost?> DeleteAsync(Guid id);
    }
}
