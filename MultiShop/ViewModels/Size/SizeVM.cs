using NuGet.Protocol.Core.Types;

namespace MultiShop.ViewModels
{
    public class SizeVM
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<IncludeProductVM> Products { get; set; }
    }
}
