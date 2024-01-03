using Microsoft.AspNetCore.Identity;

namespace MultiShop.Models
{
    public class AppUser : IdentityUser
    {
        public string Name { get; set; } = null!;
        public string Surname { get; set; } = null!;
        public string Img { get; set; } = "default-profile.png";
        public ICollection<BasketItem>? BasketItems { get; set; }
        public ICollection<Order>? Orders { get; set; }

    }
}
