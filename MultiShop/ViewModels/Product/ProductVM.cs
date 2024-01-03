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

        public string? FaceLink { get; set; }
        public string? TwitterLink { get; set; }
        public string? LinkedLink { get; set; }
        public string? PinterestLink { get; set; }

        public int? CategoryId { get; set; }
        public IncludeCategoryVM? Category { get; set; }
        public ICollection<ProductImages>? ProductImages { get; set; }
        public List<IncludeColorVM>? Colors { get; set; }
        public List<IncludeSizeVM>? Sizes { get; set; }
    }
}
