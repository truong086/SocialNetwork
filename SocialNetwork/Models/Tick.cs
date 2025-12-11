using SocialNetwork.Common;

namespace SocialNetwork.Models
{
    public class Tick : BaseEntity
    {
        public string? name { get; set; }
        public string? image { get; set; }
        public string? public_id { get; set; }
        public ICollection<User>? users { get; set; }
    }
}
