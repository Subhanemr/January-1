using MultiShop.Models;
using MultiShop.ViewModels;

namespace MultiShop.ViewModels
{
    public record ProductVM(int Id, string Name, decimal Price, int OrderId, string SKU, string? Description, 
        int CategoryId, IncludeCategoryVM Category, ICollection<ProductImages> ProductImages, ICollection<IncludeColorVM> Colors, ICollection<IncludeSizeVM> Sizes);
}
