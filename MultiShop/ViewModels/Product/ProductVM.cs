using MultiShop.Models;
using MultiShop.ViewModels;

namespace MultiShop.ViewModels
{
    public class ProductVM
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int OrderId { get; set; }
        public string SKU { get; set; }
        public string? Description { get; set; }
        public int CategoryId { get; set; }
        public IncludeCategoryVM Category { get; set; }
        public ICollection<ProductImages> ProductImages { get; set; }
        public ICollection<IncludeColorVM> Colors { get; set; }
        public ICollection<IncludeSizeVM> Sizes { get; set; }
    }
}
