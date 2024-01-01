namespace MultiShop.Models
{
    public class ProductColor
    {
        public int Id { get; set; }
        public int? ProductId { get; set; }
        public int? ColorId { get; set; }
        public Product Product { get; set; } = null!;
        public Color Color { get; set; } = null!;
    }
}
