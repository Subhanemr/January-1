using MultiShop.Models;
using System.ComponentModel.DataAnnotations;

namespace MultiShop.ViewModels
{
    public class OrderVM
    {
        [Required(ErrorMessage = "Please enter your address")]
        public string Address { get; set; }
        public ICollection<BasketItem>? BasketItems { get; set; }
    }
}
