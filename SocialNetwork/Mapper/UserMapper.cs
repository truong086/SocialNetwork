using AutoMapper;
using SocialNetwork.Models;
using SocialNetwork.ViewModel;

namespace SocialNetwork.Mapper
{
    public class UserMapper : Profile
    {
        public UserMapper()
        {
            CreateMap<UserDTO, User>()
                .ForMember(x => x.image, b => b.Ignore())
                .ForMember(x => x.role_id, b => b.Ignore());

            CreateMap<User, UserDTO>()
                .ForMember(x => x.role_id, b => b.Ignore())
                .ForMember(x => x.image, b => b.Ignore());
        }
    }
}
