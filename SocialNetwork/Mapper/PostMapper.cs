using AutoMapper;
using SocialNetwork.Models;
using SocialNetwork.ViewModel;

namespace SocialNetwork.Mapper
{
    public class PostMapper : Profile
    {
        public PostMapper()
        {
            CreateMap<PostDTO, Post>()
                .ForMember(x => x.category_id, b => b.Ignore());

            CreateMap<Post, PostDTO>()
                .ForMember(x => x.category_id, b => b.Ignore());
        }
    }
}
