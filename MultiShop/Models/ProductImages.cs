using MultiShop.Models.Common;

namespace MultiShop.Models
{
    public class ProductImages : BaseEntity
    {
        public string Url { get; set; } = null!;
        public bool? IsPrimary { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;
    }
}
