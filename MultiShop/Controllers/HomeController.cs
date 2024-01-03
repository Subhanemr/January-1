using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MultiShop.DAL;
using MultiShop.Models;
using MultiShop.ViewModels;


namespace MultiShop.Controllers
{
    public class HomeController : Controller
    {
        private readonly IMapper _mapper;
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index()
        {
            ICollection<Product> products = await _context.Products
                .Include(p => p.ProductImages.Where(pi => pi.IsPrimary != null)).OrderByDescending(s => s.OrderId).Take(8)
                .ToListAsync();
            ICollection<Settings> settings = await _context.Settings.ToListAsync();
            ICollection<Slide> slides = await _context.Slides.ToListAsync();
            ICollection<Category> categories = await _context.Categories.Where(c => c.Products.Count>0).ToListAsync();

            ICollection<CategoryVM> categoriesvMs = _mapper.Map<ICollection<CategoryVM>>(categories);
            ICollection<ProductVM> productVMs = _mapper.Map<ICollection<ProductVM>>(products);
            ICollection<SlideVM> slideVMs = _mapper.Map<ICollection<SlideVM>>(slides);
            ICollection<SettingsVM> settingsVMs = _mapper.Map<ICollection<SettingsVM>>(settings);

            HomeVM vm = new HomeVM { Slides = slideVMs, Products = productVMs, Settings = settingsVMs, Categories = categoriesvMs };

            return View(vm);
        }

        public IActionResult ErrorPage(string error)
        {
            if (error == null)
            {
                return RedirectToAction(nameof(Index));
            }
            return View(model: error);
        }
    }
}
