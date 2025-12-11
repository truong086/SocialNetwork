using SocialNetwork.Common;

namespace SocialNetwork.Models
{
    public class Category : BaseEntity
    {
        public string? name { get; set; }
        public string? description { get; set; }    
        public int? user_id { get; set; }   
        public User? user { get; set; }
        public ICollection<Post>? posts { get; set; }
    }
}
