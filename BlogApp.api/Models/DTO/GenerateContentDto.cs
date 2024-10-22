namespace BlogApp.API.Models.DTO
{
    public class GenerateContentDto
    {
        public string Title { get; set; }
        public string ShortDescription { get; set; }

        public override string ToString()
        {
            return $"Title: {Title}\nShortDescription: {ShortDescription}";
        }
    }

}
