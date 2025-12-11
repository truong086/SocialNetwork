using SocialNetwork.Common;

namespace SocialNetwork.Models
{
    public class role : BaseEntity
    {
        public string? name { get; set; }
        public ICollection<User>? users { get; set; }
    }
}
