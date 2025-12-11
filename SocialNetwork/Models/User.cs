using SocialNetwork.Common;

namespace SocialNetwork.Models
{
    public class User : BaseEntity
    {
        public string? fullname { get; set; }
        public string ? username { get; set; }
        public string? email { get; set; }  
        public string? password { get; set; }
        public string? phone { get; set; }
        public string? image { get; set; }  
        public string? publicid { get; set; }  
        public int? role_id { get; set; }  
        public role? roles { get; set; }
        public int? tick_id { get; set; }
        public Tick? tick { get; set; }
        public ICollection<image_user>? image_Users { get; set; }
        public ICollection<Category>? categories { get; set; }
        public ICollection<Post>? posts { get; set; }
        public ICollection<Post_Image>? post_Images { get; set; }
        public ICollection<Like>? likes { get; set; }
        public ICollection<Comment>? comments { get; set; }
        public ICollection<CommentRep>? commentReps { get; set; }
    }
}
