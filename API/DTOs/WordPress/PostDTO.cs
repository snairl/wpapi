namespace API.DTOs.WordPress
{
    public class PostDTO : BasedWordPressDTO
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Slug { get; set; }
        public DateTime Date { get; set; }
        public string Link { get; set; }
    }
}
