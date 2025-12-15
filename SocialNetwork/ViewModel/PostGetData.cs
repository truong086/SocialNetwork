namespace SocialNetwork.ViewModel
{
    public class PostGetData
    {
        public int? id_user { get; set; }
        public int? id_post { get; set; }
        public string? fullname { get; set; }
        public string? image_image { get; set; }
        public string? title { get; set; }
        public string? description { get; set; }
        public bool? isBackground { get; set; } 
        public int? category_id { get; set; }
        public string? category_name { get; set; }
        public int totalComment { get; set; }
        public int totalLine { get; set; }
        public DateTimeOffset? created_at { get; set; }
        public List<string>? images { get; set; }
    }
}