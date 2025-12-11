using SocialNetwork.Common;

namespace SocialNetwork.Models
{
    public class Comment : BaseEntity
    {
        public string? comment {  get; set; }
        public string? image { get; set; }
        public string? publicId { get; set; }
        public int? user_id { get; set; }
        public User? user { get; set; }
        public int? post_id { get; set; }
        public Post? post { get; set; }
        public ICollection<CommentRep>? commentReps { get; set; }
    }
}
