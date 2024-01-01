using MultiShop.Models.Common;

namespace MultiShop.Models
{
    public class Color : BaseNameEntity
    {
        public ICollection<ProductColor>? ProductColors { get; set; }
    }
}
