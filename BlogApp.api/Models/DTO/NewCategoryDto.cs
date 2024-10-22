namespace BlogApp.API.Models.DTO
{
    public class NewCategoryDto
    {
        public CategoryDto newCategory { get; set; }

        public List<CategoryDto> allCategories { get; set; }
    }
}
