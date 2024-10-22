namespace BlogApp.API.Models.DTO
{
    public class UpdateBlogPostLanguageDto
    {
        public string SelectedLanguage { get; set; }
        public UpdateBlogPostRequestDto UpdateBlogPost { get; set; }
    }
}
