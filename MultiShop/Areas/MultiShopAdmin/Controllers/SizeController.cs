using Microsoft.AspNetCore.Mvc;

namespace MultiShop.Areas.MultiShopAdmin.Controllers
{
    [Area("MultiShopAdmin")]
    public class SizeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
