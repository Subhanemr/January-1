using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MultiShop.Areas.MultiShopAdmin.ViewModels;
using MultiShop.DAL;
using MultiShop.Models;
using MultiShop.Utilities.Exceptions;
using MultiShop.Utilities.Extendions;
using MultiShop.ViewModels;

namespace MultiShop.Areas.MultiShopAdmin.Controllers
{
    [Area("MultiShopAdmin")]
    [Authorize(Roles = "Admin,Moderator")]
    [AutoValidateAntiforgeryToken]
    public class CategoryController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly IMapper _mapper;

        public CategoryController(AppDbContext context, IMapper mapper, IWebHostEnvironment env)
        {
            _context = context;
            _mapper = mapper;
            _env = env;
        }

        public async Task<IActionResult> Index(int page)
        {
            if (page < 0) throw new WrongRequestException("The request sent does not exist");
            double count = await _context.Categories.CountAsync();
            ICollection<Category> categories = await _context.Categories.Skip(page * 4).Take(4).Include(c => c.Products).ToListAsync();
            ICollection<CategoryVM> vMs = _mapper.Map<ICollection<CategoryVM>>(categories);

            PaginationVM<CategoryVM> paginationVM = new PaginationVM<CategoryVM>
            {
                TotalPage = Math.Ceiling(count / 4),
                CurrentPage = page + 1,
                Items = vMs
            };
            if (paginationVM.TotalPage < page) throw new NotFoundException("Your request was not found");
            return View(paginationVM);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateCategoryVM create)
        {
            if (!ModelState.IsValid) return View(create);

            bool result = await _context.Categories.AnyAsync(c => c.Name.ToLower().Trim() == create.Name.ToLower().Trim());

            if (result)
            {
                ModelState.AddModelError("Name", "A Category is available");
                return View(create);
            }
            if (!create.Photo.ValidateType())
            {
                ModelState.AddModelError("Photo", "File Not supported");
                return View(create);
            }

            if (!create.Photo.ValidataSize(10))
            {
                ModelState.AddModelError("Photo", "Image should not be larger than 10 mb");
                return View(create);
            }
            Category category = _mapper.Map<Category>(create);
            category.Img = await create.Photo.CreateFileAsync(_env.WebRootPath, "img");
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
