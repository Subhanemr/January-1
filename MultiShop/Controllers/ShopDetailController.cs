using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MultiShop.DAL;
using MultiShop.Models;
using MultiShop.Utilities.Exceptions;
using MultiShop.ViewModels;

namespace MultiShop.Controllers
{
    public class ShopDetailController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public ShopDetailController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IActionResult> Detail(int id)
        {
            if (id == 0) throw new WrongRequestException("The request sent does not exist");
            Product product = await _context.Products
                .Include(p => p.Category)
                .Include(pi => pi.ProductImages)
                .Include(ps => ps.ProductSizes).ThenInclude(ps => ps.Size)
                .Include(pc => pc.ProductColors).ThenInclude(pc => pc.Color)
                .FirstOrDefaultAsync(p => p.Id == id);
            if (product == null) throw new NotFoundException("Your request was not found");
            ProductVM vM = _mapper.Map<ProductVM>(product);

            ICollection<Product> products = await _context.Products
                .Include(pi => pi.ProductImages.Where(pi => pi.IsPrimary != null))
                .Where(p => p.CategoryId == product.CategoryId && p.Id != product.Id)
                .ToListAsync();
            ICollection<ProductVM> vMs = _mapper.Map<ICollection<ProductVM>>(products);


            ShopDetailVM vm = new ShopDetailVM { Product = vM, Products = vMs };

            return View(vm);
        }
    }
}
