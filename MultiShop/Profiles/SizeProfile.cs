using AutoMapper;
using MultiShop.Areas.MultiShopAdmin.ViewModels;
using MultiShop.Models;
using MultiShop.ViewModels;

namespace MultiShop.Profiles
{
    public class SizeProfile : Profile
    {
        public SizeProfile()
        {
            CreateMap<Size, CreateSizeVM>().ReverseMap();
            CreateMap<Size, UpdateSizeVM>().ReverseMap();
            CreateMap<Size, IncludeSizeVM>().ReverseMap();
            CreateMap<SizeVM, Size>().ReverseMap().ForMember(vm => vm.Products, opt => opt.MapFrom(src => src.ProductSizes.Select(pc => pc.Product).ToList()));
        }
    }
}
