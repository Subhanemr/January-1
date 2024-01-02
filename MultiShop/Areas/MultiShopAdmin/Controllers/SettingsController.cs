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
    public class SettingsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public SettingsController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [Authorize(Roles = "Admin,Moderator")]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Index(int page)
        {
            if (page < 0) throw new WrongRequestException("The request sent does not exist");
            double count = await _context.Settings.CountAsync();
            ICollection<Settings> settings = await _context.Settings.Skip(page * 4).Take(4)
                .ToListAsync(); 
            ICollection<SettingsVM> vMs = _mapper.Map<ICollection<SettingsVM>>(settings);

            PaginationVM<SettingsVM> paginationVM = new PaginationVM<SettingsVM>
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
        public async Task<IActionResult> Create(CreateSettingsVM create)
        {
            if (!ModelState.IsValid) return View(create);

            bool result = await _context.Settings.AnyAsync(c => c.Key.ToLower().Trim() == create.Key.ToLower().Trim());

            if (result)
            {
                ModelState.AddModelError("Key", "A Key with this name already exists");
                return View(create);
            }

            Settings settings = _mapper.Map<Settings>(create);

            await _context.Settings.AddAsync(settings);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin,Moderator")]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Update(int id)
        {
            if (id <= 0) { throw new WrongRequestException("The request sent does not exist"); }

            Settings settings = await _context.Settings.FirstOrDefaultAsync(c => c.Id == id);
            if (settings == null) { throw new NotFoundException("Your request was not found"); }
            UpdateSettingsVM update = _mapper.Map<UpdateSettingsVM>(settings);

            return View(update);
        }

        [HttpPost]
        public async Task<IActionResult> Update(int id, UpdateSettingsVM update)
        {
            if (!ModelState.IsValid) return View(update);

            Settings settings = await _context.Settings.FirstOrDefaultAsync(c => c.Id == id);
            if (settings == null) { throw new NotFoundException("Your request was not found"); }

            bool result = await _context.Settings.AnyAsync(c => c.Key.ToLower().Trim() == update.Key.ToLower().Trim() && c.Id != id);

            if (result)
            {
                ModelState.AddModelError("Key", "A Key with this name already exists");
                return View(update);
            }

            settings.Key = update.Key;
            settings.Value = update.Value;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0) throw new WrongRequestException("The request sent does not exist");
            Settings settings = await _context.Settings.FirstOrDefaultAsync(s => s.Id == id);
            if (settings == null) throw new NotFoundException("Your request was not found");
            _context.Settings.Remove(settings);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
