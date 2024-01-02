using System.ComponentModel.DataAnnotations;

namespace MultiShop.Areas.MultiShopAdmin.ViewModels
{
    public class UpdateCategoryVM
    {
        [Required(ErrorMessage = "Name must be entered mutled")]
        [MaxLength(25, ErrorMessage = "It should not exceed 25 characters")]
        public string Name { get; set; }
        public string Img { get; set; }
        public IFormFile? Photo { get; set; }
    }
}
