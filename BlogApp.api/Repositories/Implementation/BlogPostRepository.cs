using BlogApp.API.Data;
using BlogApp.API.Models.Domain;
using BlogApp.API.Models.DTO;
using BlogApp.API.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace BlogApp.API.Repositories.Implementation
{
    public class BlogPostRepository : IBlogPostRepository
    {
        private readonly ApplicationDbContext dbContext;
        private readonly IOpenAiRepository openAiRepository;
        private readonly ICategoryRepository categoryRepository;
        private string systemMessageForContentGeneration = "Generate Content for a Blog in mark down format";
        private string systemMessageForAssignCategories = 
            "Given the content under the 'Content' heading which is in mark-down format and the categories under the 'Categories' heading, " +
            "first check if the content matches any of the existing categories. If a match is found, return the corresponding category name only. " +
            "If no match is found, create a new category with a single-word name and return it. Just the category name";

        public BlogPostRepository(IOpenAiRepository openAiRepository,
            ICategoryRepository categoryRepository,
            ApplicationDbContext dbContext)
        {
            this.openAiRepository = openAiRepository;
            this.dbContext = dbContext;
            this.categoryRepository = categoryRepository;
        }

        public async Task<Category> AssignCategories(string content)
        {
            string blogs = "-blogs";
            var categories= await categoryRepository.GetAllAsync();
            string categoryNames = string.Join("\n", categories.Select(c => c.Name));
            string formattedString = $"Content:\n{content}\n\nCategories:\n{categoryNames}";
            var completion = await openAiRepository.GetChatCompletion(systemMessageForAssignCategories, formattedString);
            string category = completion.Content[0].Text;
            var matchedCategory = categories.FirstOrDefault(c => c.Name.Contains(category, StringComparison.OrdinalIgnoreCase));
            if (matchedCategory == null)
            {
                var newCategory = new Category();
                newCategory.Name = category;
                newCategory.UrlHandle = category+blogs;
                matchedCategory=await categoryRepository.CreateAsync(newCategory);
            }
            return matchedCategory;

        }

        private async Task<string> translate(string language, string content)
        {
            string systemMessage = $"Please translate the content into {language}. " +
                     "If it cannot be translated, return the original content. Only provide the translation; do not include any additional content.";

            var completion = await openAiRepository.GetChatCompletion(systemMessage, content);
            return completion.Content[0].Text;
        }

        public async Task<string> ContentGeneration(GenerateContentDto generateContentDto)
        {
            var completion = await openAiRepository.GetChatCompletion(systemMessageForContentGeneration, generateContentDto.ToString());
            return completion.Content[0].Text;
        }

        public async Task<BlogPost> CreateAsync(BlogPost blogPost)
        {
            await dbContext.BlogPosts.AddAsync(blogPost);
            await dbContext.SaveChangesAsync();
            return blogPost;
        }

        public async Task<BlogPost?> DeleteAsync(Guid id)
        {
            var existingBlogPost = await dbContext.BlogPosts.FirstOrDefaultAsync(x => x.Id == id);

            if (existingBlogPost != null)
            {
                dbContext.BlogPosts.Remove(existingBlogPost);
                await dbContext.SaveChangesAsync();
                return existingBlogPost;
            }

            return null;
        }

        public async Task<IEnumerable<BlogPost>> GetAllAsync()
        {
            return await dbContext.BlogPosts.Include(x => x.Categories).ToListAsync();
        }

        public async Task<IEnumerable<BlogPost>> GetByCategoriesIdAsync(Guid id)
        {
            return await dbContext.BlogPosts.Include(x => x.Categories)
                .Where(bp => bp.Categories.Any(c => c.Id == id))
                .ToListAsync();
        }

        public async Task<BlogPost?> GetByIdAsync(Guid id)
        {
            return await dbContext.BlogPosts.Include(x => x.Categories).FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<BlogPost?> GetByContentAsync(string content)
        {
            return await dbContext.BlogPosts.Include(x => x.Categories).FirstOrDefaultAsync(x => x.Content == content);
        }

        public async Task<BlogPost?> GetByUrlHandleAsync(string urlHandle)
        {
            return await dbContext.BlogPosts.Include(x => x.Categories).FirstOrDefaultAsync(x => x.UrlHandle == urlHandle);
        }

        public async Task<BlogPost?> UpdateAsync(BlogPost blogPost)
        {
            var existingBlogPost = await dbContext.BlogPosts.Include(x => x.Categories)
                .FirstOrDefaultAsync(x => x.Id == blogPost.Id);

            if (existingBlogPost == null)
            {
                return null;
            }

            dbContext.Entry(existingBlogPost).CurrentValues.SetValues(blogPost);
            existingBlogPost.Categories = blogPost.Categories;

            await dbContext.SaveChangesAsync();

            return blogPost;
        }

        public async Task<BlogPost?> UpdateLanguageAsync(string language, BlogPost blogPost)
        {
            blogPost.Title=await translate(language,blogPost.Title);
            blogPost.Author=await translate(language,blogPost.Author);
            blogPost.ShortDescription=await translate(language,blogPost.ShortDescription);
            blogPost.Content=await translate(language,blogPost.Content);
            return blogPost;
        }
    }
}
