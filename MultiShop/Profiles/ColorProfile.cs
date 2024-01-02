using AutoMapper;
using MultiShop.Areas.MultiShopAdmin.ViewModels;
using MultiShop.ViewModels;
using MultiShop.Models;

namespace MultiShop.Profiles
{
    public class ColorProfile : Profile
    {
        public ColorProfile()
        {
            CreateMap<Color, CreateColorVM>().ReverseMap();
            CreateMap<Color, UpdateColorVM>().ReverseMap();
            CreateMap<Color, IncludeColorVM>().ReverseMap();
            CreateMap<ColorVM, Color>().ReverseMap().ForMember(vm => vm.Products, opt => opt.MapFrom(src => src.ProductColors.Select(pc => pc.Product).ToList()));
        }
    }
}
