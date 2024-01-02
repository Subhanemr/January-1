using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MultiShop.DAL;

namespace MultiShop.ViewComponents
{
    public class FooterViewComponent : ViewComponent
    {
        private readonly AppDbContext _context;

        public FooterViewComponent(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            Dictionary<string, string> keyValuePairs = await _context.Settings.ToDictionaryAsync(p => p.Key, p => p.Value);
            return View(keyValuePairs);
        }
    }
}
