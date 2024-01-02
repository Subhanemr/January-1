using AutoMapper;
using MultiShop.Areas.MultiShopAdmin.ViewModels;
using MultiShop.Models;
using MultiShop.ViewModels;

namespace MultiShop.Profiles
{
    public class SettingsProfile : Profile
    {
        public SettingsProfile()
        {
            CreateMap<Settings, CreateSettingsVM>().ReverseMap();
            CreateMap<Settings, UpdateSettingsVM>().ReverseMap();
            CreateMap<Settings, SettingsVM>().ReverseMap();
        }
    }
}
