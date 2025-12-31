using SocialNetwork.Common;
using SocialNetwork.ViewModel;

namespace SocialNetwork.Service
{
    public interface ICommentService
    {
        Task<PayLoad<object>> AddComment(CommentDTO data);
    }
}
