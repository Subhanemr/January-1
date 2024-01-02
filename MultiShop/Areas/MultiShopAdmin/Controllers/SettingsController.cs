using Microsoft.AspNetCore.Mvc;

namespace MultiShop.Areas.MultiShopAdmin.Controllers
{
    [Area("MultiShopAdmin")]
    public class SettingsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
