using SocialNetwork.Common;

namespace SocialNetwork.Models
{
    public class Post_Image : BaseEntity
    {
        public string? image { get; set; }
        public string? public_id { get; set; }  
        public int? image_user_id { get; set; }
        public image_user? image_users { get; set; }
        public int? post_id { get; set; }
        public Post? post { get; set; }
        public int? user_id { get; set; }
        public User? user { get; set; }

    }
}
