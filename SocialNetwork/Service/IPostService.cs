using SocialNetwork.Common;
using SocialNetwork.ViewModel;

namespace SocialNetwork.Service
{
    public interface IPostService
    {
        Task<PayLoad<PostDTO>> Add(PostDTO postDTO);
        Task<PayLoad<object>> AddLike(int post_id);
        Task<PayLoad<PostEditImageDTO>> AddEditImage(PostEditImageDTO postDTO);
        Task<PayLoad<object>> FindAll(string? name, int? category, int page = 1, int pageSize = 20);
        Task<PayLoad<object>> FindAllPostByUser(string? name, int? category, int page = 1, int pageSize = 20);
        Task<PayLoad<PostDTO>> Update (int id,  PostDTO postDTO);
        Task<PayLoad<object>> DeleteById(int id);
        Task<PayLoad<object>> GetById(int id);
        Task<PayLoad<object>> addImageTestCloud(IFormFile? data);
    }
}
