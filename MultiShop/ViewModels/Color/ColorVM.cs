namespace MultiShop.ViewModels
{
    public record ColorVM(int Id, string Name, ICollection<IncludeProductVM> Products);
}
