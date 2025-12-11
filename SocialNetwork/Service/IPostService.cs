using SocialNetwork.Common;
using SocialNetwork.ViewModel;

namespace SocialNetwork.Service
{
    public interface IPostService
    {
        Task<PayLoad<PostDTO>> Add(PostDTO postDTO);
        Task<PayLoad<object>> FindAll(string? name, int page = 1, int pageSize = 20);
    }
}
