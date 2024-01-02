using MultiShop.Models;

namespace MultiShop.ViewModels
{
    public class IncludeProductVM
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int OrderId { get; set; }
        public string SKU { get; set; }
        public string? Description { get; set; }
        public int CategoryId { get; set; }
        public ICollection<ProductImages> ProductImages { get; set; }
    }
}
