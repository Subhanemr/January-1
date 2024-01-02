using AutoMapper;
using MultiShop.Areas.MultiShopAdmin.ViewModels;
using MultiShop.Models;
using MultiShop.ViewModels;

namespace MultiShop.Profiles
{
    public class SlideProfile : Profile
    {
        public SlideProfile()
        {
            CreateMap<Slide, CreateSlideVM>().ReverseMap();
            CreateMap<Slide, UpdateSlideVM>().ReverseMap();
            CreateMap<Slide, SlideVM>().ReverseMap();

        }
    }
}
