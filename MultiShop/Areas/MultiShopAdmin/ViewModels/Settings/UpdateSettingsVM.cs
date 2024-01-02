using System.ComponentModel.DataAnnotations;

namespace MultiShop.Areas.MultiShopAdmin.ViewModels
{
    public class UpdateSettingsVM
    {
        [Required(ErrorMessage = "Key is reuqired")]
        public string Key { get; set; }
        [Required(ErrorMessage = "Value is reuqired")]
        public string Value { get; set; }
    }
}
