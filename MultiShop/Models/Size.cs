using MultiShop.Models.Common;

namespace MultiShop.Models
{
    public class Size : BaseNameEntity
    {
        public ICollection<ProductSize>? ProductSizes { get; set; }
    }
}
