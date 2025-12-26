namespace SocialNetwork.ViewModel
{
    public class PostDTO
    {
        public List<imageCloud>? images { get; set; }
        public string? title { get; set; }
        public string? description { get; set; }
        public bool? isBackground { get; set; }
        public int? category_id { get; set; }
    }

    public class PostEditImageDTO
    {
        public IFormFile? images { get; set; }
        public int? category_id { get; set; }
    }

    public class imageCloud
    {
        public int? id { get; set; }
        public string? image { get; set; }
        public string? publicId { get; set; }
    }

}
