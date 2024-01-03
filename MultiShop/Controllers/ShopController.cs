using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MultiShop.DAL;
using MultiShop.Models;
using MultiShop.Utilities.Exceptions;
using MultiShop.ViewModels;

namespace MultiShop.Controllers
{
    public class ShopController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public ShopController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index(string? search, int? order, int? categoryId, int page)
        {
            if (page < 0) throw new WrongRequestException("The request sent does not exist");
            double count = await _context.Products.CountAsync();
            IQueryable<Product> queryable = _context.Products.Skip(page * 4).Take(4)
                .Include(pi => pi.ProductImages.Where(a => a.IsPrimary != null)).AsQueryable();
            switch (order)
            {
                case 1:
                    queryable = queryable.OrderBy(p => p.Name);
                    break;
                case 2:
                    queryable = queryable.OrderBy(p => p.Price);
                    break;
                case 3:
                    queryable = queryable.OrderByDescending(p => p.CreateAt);
                    break;
                case 4:
                    queryable = queryable.OrderByDescending(p => p.Price);
                    break;
            }
            if (!string.IsNullOrEmpty(search))
            {
                queryable = queryable.Where(p => p.Name.ToLower().Contains(search.ToLower()));
            }
            if (categoryId != null)
            {
                queryable = queryable.Where(p => p.CategoryId == categoryId);
            }
            ShopVM shopVM = new ShopVM
            {
                Categories = _mapper.Map<ICollection<CategoryVM>>(await _context.Categories.Include(c => c.Products).ToListAsync()),
                Products = _mapper.Map<ICollection<ProductVM>>(await queryable.ToListAsync()),
                Order = order,
                Search = search,
                CategoryId = categoryId,
            };

            PaginationVM<ShopVM> paginationVM = new PaginationVM<ShopVM>
            {
                CurrentPage = page + 1,
                TotalPage = Math.Ceiling(count / 4),
                Item = shopVM
            };
            if (paginationVM.TotalPage < page) throw new NotFoundException("Your request was not found");

            return View(paginationVM);
        }
    }
}
