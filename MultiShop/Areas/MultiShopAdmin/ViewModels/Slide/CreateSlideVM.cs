using System.ComponentModel.DataAnnotations;

namespace MultiShop.Areas.MultiShopAdmin.ViewModels
{
    public class CreateSlideVM
    {
        [Required(ErrorMessage = "Name must be entered mutled")]
        [MaxLength(25, ErrorMessage = "It should not exceed 25 characters")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Name must be entered mutled")]
        [MaxLength(150, ErrorMessage = "It should not exceed 150 characters")]
        public string SubTitle { get; set; }

        [Required(ErrorMessage = "Name must be entered mutled")]
        [MaxLength(20, ErrorMessage = "It should not exceed 20 characters")]
        public string ButtonText { get; set; }

        [Required(ErrorMessage = "OrderId must be entered mutled")]
        public int OrderId { get; set; }


        [Required(ErrorMessage = "Image must be uploaded")]
        public IFormFile Photo { get; set; }
    }
}
