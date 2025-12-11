using SocialNetwork.Common;

namespace SocialNetwork.Models
{
    public class Like : BaseEntity
    {
        public int? post_id { get; set; }
        public Post? post { get; set; }
        public int? user_id { get; set; }
        public User? user { get; set; }
        public bool? isLiked { get; set; }
    }
}
