using MultiShop.Models.Common;

namespace MultiShop.Models
{
    public class Slide : BaseEntity
    {
        public string Title { get; set; } = null!;
        public string SubTitle { get; set; } = null!;
        public string ButtonText { get; set; } = "Shop Now";
        public int OrderId { get; set; }
        public string ImgUrl { get; set; } = null!;

    }
}
