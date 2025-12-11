using AutoMapper;
using SocialNetwork.Models;
using SocialNetwork.ViewModel;

namespace SocialNetwork.Mapper
{
    public class RoleMapper : Profile
    {
        public RoleMapper()
        {
            CreateMap<roleDTO, role>();
            CreateMap<role, roleDTO>();

        }
    }
}
