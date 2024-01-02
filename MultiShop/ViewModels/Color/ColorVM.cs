namespace MultiShop.ViewModels
{
    public class ColorVM
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<IncludeProductVM> Products { get; set; }
    }
}
