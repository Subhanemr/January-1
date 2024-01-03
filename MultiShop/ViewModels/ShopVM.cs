using MultiShop.Models;

namespace MultiShop.ViewModels
{
    public class ShopVM
    {
        public int? Order { get; set; }
        public int? CategoryId { get; set; }
        public string? Search { get; set; }
        public ICollection<ProductVM> Products { get; set; }
        public ICollection<CategoryVM> Categories { get; set; }
    }
}
