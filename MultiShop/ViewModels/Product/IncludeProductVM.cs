namespace MultiShop.ViewModels
{
    public record IncludeProductVM(int Id, string Name, decimal Price, int OrderId, string SKU, string? Description, int CategoryId);
}
