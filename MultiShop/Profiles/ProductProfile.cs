using AutoMapper;
using MultiShop.Areas.MultiShopAdmin.ViewModels;
using MultiShop.Models;
using MultiShop.ViewModels;

namespace MultiShop.Profiles
{
    public class ProductProfile : Profile
    {
        public ProductProfile()
        {
            CreateMap<Product, CreateProductVM>().ReverseMap();
            CreateMap<Product, UpdateProductVM>().ReverseMap();
            CreateMap<Product, IncludeProductVM>().ReverseMap();
            CreateMap<ProductVM, Product>().ReverseMap()
                .ForMember(vm => vm.Colors, opt => opt.MapFrom(src => src.ProductColors.Select(pc => pc.Color).ToList()))
                .ForMember(vm => vm.Sizes, opt => opt.MapFrom(src => src.ProductSizes.Select(pc => pc.Size).ToList()));


        }
    }
}
