using MultiShop.Models;

namespace MultiShop.ViewModels
{
    public class HeaderVM
    {
        public Dictionary<string, string> Settings { get; set; }
        public ICollection<CartItemVM>? Items { get; set; }
        public ICollection<WishListItemVM>? WishListItems { get; set; }
        public ICollection<CategoryVM> Categories { get; set; }
        public AppUser? User { get; set; }
    }
}
