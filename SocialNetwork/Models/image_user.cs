using SocialNetwork.Common;

namespace SocialNetwork.Models
{
    public class image_user : BaseEntity
    {
        public int? user_id { get; set; }
        public User? user { get; set; }
        public string? image { get; set; }
        public string? public_id { get; set; }
        public ICollection<Post_Image>? post_Images { get; set; }

    }
}
