namespace MultiShop.ViewModels
{
    public class ShopDetailVM
    {
        public ProductVM Product { get; set; }
        public ICollection<ProductVM> Products { get; set; }
    }
}
