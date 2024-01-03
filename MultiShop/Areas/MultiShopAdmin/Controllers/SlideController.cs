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
    public class SlideController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly IMapper _mapper;

        public SlideController(AppDbContext context, IWebHostEnvironment env, IMapper mapper)
        {
            _context = context;
            _env = env;
            _mapper = mapper;
        }

        [Authorize(Roles = "Admin,Moderator")]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Index(int page)
        {
            if (page < 0) throw new WrongRequestException("The request sent does not exist");
            double count = await _context.Slides.CountAsync();
            ICollection<Slide> slide = await _context.Slides.Skip(page*4).Take(4).ToListAsync();
            ICollection <SlideVM> vMs = _mapper.Map<ICollection<SlideVM>>(slide);

            PaginationVM<SlideVM> paginationVM = new PaginationVM<SlideVM>
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
        public async Task<IActionResult> Create(CreateSlideVM create)
        {
            if (!ModelState.IsValid) return View(create);

            if (create.Photo is null)
            {
                ModelState.AddModelError("Photo", "The image must be uploaded");
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

            Slide slide = _mapper.Map<Slide>(create);
            slide.ImgUrl = await create.Photo.CreateFileAsync(_env.WebRootPath, "img");

            await _context.Slides.AddAsync(slide);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin,Moderator")]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Update(int id)
        {
            if (id <= 0) throw new WrongRequestException("The request sent does not exist");
            Slide slide = await _context.Slides.FirstOrDefaultAsync(c => c.Id == id);

            if (slide == null) throw new NotFoundException("Your request was not found");

            UpdateSlideVM update = _mapper.Map<UpdateSlideVM>(slide);

            return View(update);
        }

        [HttpPost]
        public async Task<IActionResult> Update(int id, UpdateSlideVM update)
        {
            if (!ModelState.IsValid) return View(update);

            Slide existed = await _context.Slides.FirstOrDefaultAsync(c => c.Id == id);
            if (existed == null) throw new NotFoundException("Your request was not found");
            if (update.Photo is not null)
            {

                if (!update.Photo.ValidateType())
                {
                    ModelState.AddModelError("Photo", "File Not supported");
                    return View(existed);
                }

                if (!update.Photo.ValidataSize(10))
                {
                    ModelState.AddModelError("Photo", "Image should not be larger than 10 mb");
                    return View(existed);
                }

                string fileName = await update.Photo.CreateFileAsync(_env.WebRootPath, "img");
                existed.ImgUrl.DeleteFileAsync(_env.WebRootPath, "img");
                existed.ImgUrl = fileName;
                update.ImgUrl = fileName;
            }

            _mapper.Map(update, existed);

            _context.Slides.Update(existed);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0) throw new WrongRequestException("The request sent does not exist");

            Slide existed = await _context.Slides.FirstOrDefaultAsync(c => c.Id == id);

            if (existed == null) throw new NotFoundException("Your request was not found");

            existed.ImgUrl.DeleteFileAsync(_env.WebRootPath, "img");

            _context.Slides.Remove(existed);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin,Moderator")]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> More(int id)
        {
            if (id <= 0) throw new WrongRequestException("The request sent does not exist");
            Slide slide = await _context.Slides.FirstOrDefaultAsync(c => c.Id == id);
            if (slide == null) throw new NotFoundException("Your request was not found");
            SlideVM vM = _mapper.Map<SlideVM>(slide);

            return View(vM);
        }
    }
}
