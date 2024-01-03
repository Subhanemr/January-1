using MultiShop.Models.Common;

namespace MultiShop.Models
{
    public class Product : BaseNameEntity
    {
        public decimal Price { get; set; }
        public int OrderId { get; set; }
        public string SKU { get; set; } = null!;
        public string? Description { get; set; }

        public string? FaceLink { get; set; }
        public string? TwitterLink { get; set; }
        public string? LinkedLink { get; set; }
        public string? PinterestLink { get; set; }

        public int CategoryId { get; set; }
        public Category Category { get; set; } = null!;
        public ICollection<ProductImages> ProductImages { get; set; } = null!;
        public ICollection<ProductColor>? ProductColors { get; set; }
        public ICollection<ProductSize>? ProductSizes { get; set; }
    }
}
