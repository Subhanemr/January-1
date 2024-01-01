using MultiShop.Models.Common;

namespace MultiShop.Models
{
    public class Category : BaseNameEntity
    {
        public string Img { get; set; } = null!;
        public ICollection<Product>? Products { get; set; }
    }
}
