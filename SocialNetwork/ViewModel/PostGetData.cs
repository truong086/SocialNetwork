namespace SocialNetwork.ViewModel
{
    public class PostGetData
    {
        public int? id_user { get; set; }
        public int? id { get; set; }
        public string? name { get; set; }
        public string? avatar { get; set; }
        public string? title { get; set; }
        public string? content { get; set; }
        public bool? isBackground { get; set; } 
        public int? category_id { get; set; }
        public string? category_name { get; set; }
        public int totalComment { get; set; }
        public int likes { get; set; }
        public bool? isUserLike { get; set; }
        public DateTimeOffset? time { get; set; }
        public List<string>? images { get; set; }
    }
}