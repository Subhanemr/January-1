using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MultiShop.Areas.MultiShopAdmin.ViewModels;
using MultiShop.DAL;
using MultiShop.Models;
using MultiShop.Utilities.Exceptions;
using MultiShop.ViewModels;

namespace MultiShop.Areas.MultiShopAdmin.Controllers
{
    [Area("MultiShopAdmin")]
    [Authorize(Roles = "Admin,Moderator")]
    [AutoValidateAntiforgeryToken]
    public class SizeController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public SizeController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [Authorize(Roles = "Admin,Moderator")]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Index(int page)
        {
            if (page < 0) throw new WrongRequestException("The request sent does not exist");
            double count = await _context.Sizes.CountAsync();
            ICollection<Size> sizes = await _context.Sizes.Skip(page * 4).Take(4).Include(c => c.ProductSizes).ToListAsync();
            ICollection<SizeVM> vMs = _mapper.Map<ICollection<SizeVM>>(sizes);

            PaginationVM<SizeVM> paginationVM = new PaginationVM<SizeVM>
            {
                CurrentPage = page + 1,
                TotalPage = Math.Ceiling(count / 4),
                Items = vMs
            };
            if (paginationVM.TotalPage < page) throw new NotFoundException("Your request was not found");

            return View(paginationVM);
        }

        [Authorize(Roles = "Admin,Moderator")]
        [AutoValidateAntiforgeryToken]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateSizeVM create)
        {
            if (!ModelState.IsValid)
            {
                return View(create);
            }

            bool result = await _context.Sizes.AnyAsync(c => c.Name.ToLower().Trim() == create.Name.ToLower().Trim());

            if (result)
            {
                ModelState.AddModelError("Color.Name", "A Color with this name already exists");
                return View(create);
            }
            Size size = _mapper.Map<Size>(create);

            await _context.Sizes.AddAsync(size);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin,Moderator")]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Update(int id)
        {
            if (id <= 0) throw new WrongRequestException("The request sent does not exist");
            Size size = await _context.Sizes.FirstOrDefaultAsync(c => c.Id == id);

            if (size == null) throw new NotFoundException("Your request was not found");
            UpdateSizeVM update = _mapper.Map<UpdateSizeVM>(size);

            return View(update);
        }

        [HttpPost]
        public async Task<IActionResult> Update(int id, UpdateSizeVM update)
        {
            if (!ModelState.IsValid) return View();

            Size existed = await _context.Sizes.FirstOrDefaultAsync(c => c.Id == id);
            if (existed == null) throw new NotFoundException("Your request was not found");

            bool result = await _context.Sizes.AnyAsync(c => c.Name.ToLower().Trim() == update.Name.ToLower().Trim() && c.Id != id);

            if (result)
            {
                ModelState.AddModelError("Name", "A Size is available");
                return View();
            }

            _mapper.Map(update, existed);
            _context.Sizes.Update(existed);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0) throw new WrongRequestException("The request sent does not exist");
            Size existed = await _context.Sizes.FirstOrDefaultAsync(c => c.Id == id);
            if (existed == null) throw new NotFoundException("Your request was not found");
            _context.Sizes.Remove(existed);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin,Moderator")]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> More(int id)
        {
            if (id <= 0) throw new WrongRequestException("The request sent does not exist");
            Size size = await _context.Sizes
                .Include(p => p.ProductSizes)
                .ThenInclude(p => p.Product).ThenInclude(p => p.ProductImages)
                .FirstOrDefaultAsync(c => c.Id == id);
            if (size == null) throw new NotFoundException("Your request was not found");
            SizeVM vM = _mapper.Map<SizeVM>(size);  

            return View(vM);
        }
    }
}
