namespace MultiShop.ViewModels
{
    public record SizeVM(int Id, string Name, ICollection<IncludeProductVM> Products);
}
