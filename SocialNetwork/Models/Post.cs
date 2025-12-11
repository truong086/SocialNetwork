using SocialNetwork.Common;

namespace SocialNetwork.Models
{
    public class Post : BaseEntity
    {
        public string? title { get; set; }
        public string? description { get; set; }
        public bool? isBackground { get; set; }
        public int? category_id { get; set; }
        public Category? category { get; set; }
        public int totalLine { get; set; }
        public int totalComment { get; set; }
        public  int? user_id { get; set; }
        public User? user { get; set; }
        public bool? isHide { get; set; }
        public bool? isAdminHide { get; set; }
        public ICollection<Post_Image>? post_Images { get; set; }
        public ICollection<Like>? likes { get; set; }
        public ICollection<Comment>? comments { get; set; }
    }
}
