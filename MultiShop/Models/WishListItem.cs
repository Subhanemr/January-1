using MultiShop.Models.Common;

namespace MultiShop.Models
{
    public class WishListItem : BaseEntity
    {
        public decimal Price { get; set; }
        public string AppUserId { get; set; } = null!;
        public AppUser AppUser { get; set; } = null!;
        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;
    }
}
