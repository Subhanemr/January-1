using MultiShop.Models.Common;

namespace MultiShop.Models
{
    public class Order : BaseEntity
    {
        public bool? Status { get; set; }
        public string Address { get; set; } = null!;
        public decimal TotalPrice { get; set; }
        public DateTime PruchaseAt { get; set; }
        public string AppUserId { get; set; } = null!;
        public AppUser AppUser { get; set; } = null!;
        public ICollection<BasketItem> BasketItems { get; set; } = null!;
    }
}
