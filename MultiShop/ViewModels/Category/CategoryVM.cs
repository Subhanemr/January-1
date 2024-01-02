namespace MultiShop.ViewModels
{
    public class CategoryVM
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Img { get; set; }
        public ICollection<IncludeProductVM> Products { get; set; }
    }
}
