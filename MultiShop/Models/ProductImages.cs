namespace MultiShop.Models
{
    public class ProductImages
    {
        public int Id { get; set; }
        public string Url { get; set; } = null!;
        public bool? IsPrimary { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;
    }
}
