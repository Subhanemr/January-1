namespace MultiShop.Models
{
    public class Order
    {
        public int Id { get; set; }
        public bool? Status { get; set; }
        public string Address { get; set; } = null!;
        public decimal TotalPrice { get; set; }
        public DateTime PruchaseAt { get; set; }
        public string AppUserId { get; set; } = null!;
        public AppUser AppUser { get; set; } = null!;
        public ICollection<BasketItem> BasketItems { get; set; } = null!;
    }
}
