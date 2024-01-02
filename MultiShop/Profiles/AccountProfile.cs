using AutoMapper;
using MultiShop.Models;
using MultiShop.Utilities.Extendions;
using MultiShop.ViewModels;

namespace MultiShop.Profiles
{
    public class AccountProfile : Profile
    {
        public AccountProfile()
        {
            CreateMap<RegisterVM, AppUser>().ReverseMap()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name.Capitalize()))
                .ForMember(dest => dest.Surname, opt => opt.MapFrom(src => src.Surname.Capitalize()));

            CreateMap<AppUser, LoginVM>().ReverseMap();
            CreateMap<AppUser, EditUserVM>().ReverseMap()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name.Capitalize()))
                .ForMember(dest => dest.Surname, opt => opt.MapFrom(src => src.Surname.Capitalize()));
        }
    }
}
