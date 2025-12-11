using SocialNetwork.Common;

namespace SocialNetwork.Models
{
    public class CommentRep : BaseEntity
    {
        public string? comment { get; set; }
        public int? user_id { get; set; }
        public User? user { get; set; }
        public int? comment_Id { get; set; }
        public Comment? comments { get; set; }
        public string? image { get; set; }
        public string? publicId { get; set; }
        public int? comentRep2 { get; set; }
        public CommentRep? commentRep { get; set; }
        public int? commentRep3 { get; set; }
        public ICollection<CommentRep>? commentReps { get; set; }

    }
}
