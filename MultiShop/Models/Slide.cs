namespace MultiShop.Models
{
    public class Slide
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string SubTitle { get; set; } = null!;
        public string ButtonText { get; set; } = "Shop Now";
        public string ImgUrl { get; set; } = null!;

    }
}
