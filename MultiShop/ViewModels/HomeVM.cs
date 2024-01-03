using MultiShop.ViewModels;

namespace MultiShop.ViewModels
{
    public class HomeVM
    {
        public ICollection<ProductVM> Products { get; set; }
        public ICollection<CategoryVM> Categories { get; set; }
        public ICollection<SlideVM> Slides { get; set; }
        public ICollection<SettingsVM> Settings { get; set; }
    }
}
