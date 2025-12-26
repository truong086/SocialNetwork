using SocialNetwork.Common;
using SocialNetwork.ViewModel;

namespace SocialNetwork.Service
{
    public interface IUserService
    {
        Task<PayLoad<object>> FindAll(string? name, int page = 1, int pageSize = 20);
        Task<PayLoad<UserDTO>> Add (UserDTO userDTO);
        Task<PayLoad<object>> Login(LoginDTO loginDTO);
        Task<PayLoad<UserDTO>> Update(int id, UserDTO data);
        Task<PayLoad<string>> Delete (int id);
        Task<PayLoad<object>> uploadImageUser (ImageUserUpload data);
        Task<PayLoad<object>> FindAlluploadImageUser ();
    }
}
