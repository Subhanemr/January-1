namespace MultiShop.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Img { get; set; }
        public ICollection<Product>? Products { get; set; }
    }
}
