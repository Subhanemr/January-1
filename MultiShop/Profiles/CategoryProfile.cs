using AutoMapper;
using MultiShop.Areas.MultiShopAdmin.ViewModels;
using MultiShop.Models;
using MultiShop.ViewModels;

namespace MultiShop.Profiles
{
    public class CategoryProfile : Profile
    {
        public CategoryProfile()
        {
            CreateMap<Category, CreateCategoryVM>().ReverseMap();
            CreateMap<Category, UpdateCategoryVM>().ReverseMap();
            CreateMap<Category, IncludeCategoryVM>().ReverseMap();
            CreateMap<Category, CategoryVM>().ReverseMap();
        }
    }
}
