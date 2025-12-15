using AutoMapper;
using SocialNetwork.Models;
using SocialNetwork.ViewModel;

namespace SocialNetwork.Mapper
{
    public class CategoryMapper : Profile
    {
        public CategoryMapper()
        {
            CreateMap<CategoryDTO, Category>();
            CreateMap<Category, CategoryDTO>();

        }
    }
}
