using MultiShop.Models;

namespace MultiShop.ViewModels
{
    public class HeaderVM
    {
        public Dictionary<string, string> Settings { get; set; }
        public ICollection<CartItemVM> Items { get; set; }
        public AppUser? User { get; set; }
    }
}
