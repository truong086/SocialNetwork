using SocialNetwork.Common;
using SocialNetwork.ViewModel;

namespace SocialNetwork.Service
{
    public interface IRoleService
    {
        Task<PayLoad<roleDTO>> Add(roleDTO data);
        Task<PayLoad<object>> FindAll(string? name, int page = 10, int pageSize = 20);
        Task<PayLoad<object>> FindOne(int id);
        Task<PayLoad<roleDTO>> Update(int id,  roleDTO data);
        Task<PayLoad<string>> Delete(int id);
    }
}
