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
using NuGet.Packaging;

namespace MultiShop.Areas.MultiShopAdmin.Controllers
{
    [Area("MultiShopAdmin")]
    [Authorize(Roles = "Admin,Moderator")]
    [AutoValidateAntiforgeryToken]
    public class ProductController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly IMapper _mapper;

        private void CreatePopulateDropdowns(CreateProductVM create)
        {
            create.Categories = _mapper.Map<ICollection<IncludeCategoryVM>>(_context.Categories.ToList());
            create.Colors = _mapper.Map<ICollection<IncludeColorVM>>(_context.Colors.ToList());
            create.Sizes = _mapper.Map<ICollection<IncludeSizeVM>>(_context.Sizes.ToList());
        }
        private void UpdatePopulateDropdowns(UpdateProductVM update)
        {
            update.Categories = _mapper.Map<ICollection<IncludeCategoryVM>>(_context.Categories.ToList());
            update.Colors = _mapper.Map<ICollection<IncludeColorVM>>(_context.Colors.ToList());
            update.Sizes = _mapper.Map<ICollection<IncludeSizeVM>>(_context.Sizes.ToList());
        }

        public ProductController(AppDbContext context, IWebHostEnvironment env, IMapper mapper)
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
            double count = await _context.Products.CountAsync();
            ICollection<Product> product = await _context.Products.Skip(page * 4).Take(4)
                .Include(p => p.Category)
                .Include(p => p.ProductImages.Where(pi => pi.IsPrimary == true))
                .ToListAsync();
            ICollection<ProductVM> vMs = _mapper.Map<ICollection<ProductVM>>(product);

            PaginationVM<ProductVM> paginationVM = new PaginationVM<ProductVM>
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
        public async Task<IActionResult> Create()
        {
            CreateProductVM create = new CreateProductVM();
            CreatePopulateDropdowns(create);

            return View(create);
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateProductVM create)
        {
            if (!ModelState.IsValid)
            {
                CreatePopulateDropdowns(create);
                return View(create);
            }
            bool result = await _context.Products.AnyAsync(c => c.Name.ToLower().Trim() == create.Name.ToLower().Trim());
            if (result)
            {
                CreatePopulateDropdowns(create);
                ModelState.AddModelError("Name", "A Name is available");
                return View(create);
            };
            bool resultOrder = await _context.Products.AnyAsync(c => c.OrderId == create.OrderId);
            if (resultOrder)
            {
                CreatePopulateDropdowns(create);
                ModelState.AddModelError("OrderId", "A Order is available");
                return View(create);
            }

            bool resultCategory = await _context.Categories.AnyAsync(c => c.Id == create.CategoryId);
            if (!resultCategory)
            {
                CreatePopulateDropdowns(create);
                ModelState.AddModelError("CategoryId", "This id has no category");
                return View(create);
            }
            if (create.Price <= 0)
            {
                CreatePopulateDropdowns(create);
                ModelState.AddModelError("Price", "Price cannot be 0");
                return View(create);
            };

            foreach (int sizeId in create.SizeIds)
            {
                bool sizeResult = await _context.Sizes.AnyAsync(t => t.Id == sizeId);
                if (!sizeResult)
                {
                    CreatePopulateDropdowns(create);
                    return View(create);
                }
            }
            foreach (int colorId in create.ColorIds)
            {
                bool colorResult = await _context.Colors.AnyAsync(t => t.Id == colorId);
                if (!colorResult)
                {
                    CreatePopulateDropdowns(create);
                    return View(create);
                }
            }
            if (!create.MainPhoto.ValidateType())
            {
                CreatePopulateDropdowns(create);
                ModelState.AddModelError("MainPhoto", "File Not supported");
                return View(create);
            }

            if (!create.MainPhoto.ValidataSize(10))
            {
                CreatePopulateDropdowns(create);
                ModelState.AddModelError("MainPhoto", "Image should not be larger than 10 mb");
                return View(create);
            }

            ProductImages mainImage = new ProductImages
            {
                IsPrimary = true,
                Url = await create.MainPhoto.CreateFileAsync(_env.WebRootPath, "img")
            };

            Product product = _mapper.Map<Product>(create);

            product.ProductImages = new List<ProductImages> { mainImage };
            product.ProductSizes = create.SizeIds.Select(sizeId => new ProductSize { SizeId = sizeId }).ToList();
            product.ProductColors = create.ColorIds.Select(colorId => new ProductColor { ColorId = colorId }).ToList();

            TempData["Message"] = "";

            foreach (IFormFile photo in create.Photos)
            {
                if (!photo.ValidateType())
                {
                    TempData["Message"] += $"<p class=\"text-danger\">{photo.Name} type is not suitable</p>";
                    continue;
                }

                if (!photo.ValidataSize(10))
                {
                    TempData["Message"] += $"<p class=\"text-danger\">{photo.Name} the size is not suitable</p>";
                    continue;
                }

                product.ProductImages.Add(new ProductImages
                {
                    IsPrimary = null,
                    Url = await photo.CreateFileAsync(_env.WebRootPath, "img")
                });
            }

            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        [Authorize(Roles = "Admin")]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0) throw new WrongRequestException("The request sent does not exist");

            Product existed = await _context.Products
                .Include(p => p.ProductImages)
                .Include(p => p.ProductColors)
                .Include(p => p.ProductSizes)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (existed == null) throw new NotFoundException("Your request was not found");

            ICollection<ProductColor> productColors = await _context.ProductColors.Where(pc => pc.ProductId == id).ToListAsync();
            _context.ProductColors.RemoveRange(productColors);

            ICollection<ProductSize> productSizes = await _context.ProductSizes.Where(pc => pc.ProductId == id).ToListAsync();
            _context.ProductSizes.RemoveRange(productSizes);

            foreach (ProductImages image in existed.ProductImages)
            {
                image.Url.DeleteFileAsync(_env.WebRootPath, "img");
            }
            ICollection<ProductImages> productImages = await _context.ProductImages.Where(pc => pc.ProductId == id).ToListAsync();
            _context.ProductImages.RemoveRange(productImages);

            _context.Products.Remove(existed);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        [Authorize(Roles = "Admin,Moderator")]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Update(int id)
        {
            if (id <= 0) { throw new WrongRequestException("The request sent does not exist"); }
            Product product = await _context.Products
                .Include(p => p.ProductImages)
                .Include(p => p.ProductColors)
                .Include(p => p.ProductSizes)
                .FirstOrDefaultAsync(p => p.Id == id);
            if (product == null) throw new NotFoundException("Your request was not found");
            UpdateProductVM update = _mapper.Map<UpdateProductVM>(product);

            update.SizeIds = product.ProductSizes.Select(p => p.SizeId).ToList();
            update.ColorIds = product.ProductColors.Select(p => p.ColorId).ToList();

            update.Categories = _mapper.Map<ICollection<IncludeCategoryVM>>(await _context.Categories.ToListAsync());
            update.Colors = _mapper.Map<ICollection<IncludeColorVM>>(await _context.Colors.ToListAsync());
            update.Sizes = _mapper.Map<ICollection<IncludeSizeVM>>(await _context.Sizes.ToListAsync());

            return View(update);
        }

        [HttpPost]
        public async Task<IActionResult> Update(int id, UpdateProductVM update)
        {
            Product existed = await _context.Products
                .Include(p => p.ProductImages)
                .Include(p => p.ProductSizes)
                .Include(p => p.ProductColors)
                .FirstOrDefaultAsync(c => c.Id == id);

            update.ProductImages = existed.ProductImages;
            if (!ModelState.IsValid)
            {
                UpdatePopulateDropdowns(update);
                return View(update);
            }

            if (existed == null) throw new NotFoundException("Your request was not found");
            bool resultCategory = await _context.Categories.AnyAsync(c => c.Id == update.CategoryId);
            if (!resultCategory)
            {
                UpdatePopulateDropdowns(update);
                ModelState.AddModelError("CategoryId", "This id has no category");
                return View(update);
            }

            bool resultColor = await _context.Colors.AnyAsync(pc => update.ColorIds.Contains(pc.Id));
            if (!resultColor)
            {
                UpdatePopulateDropdowns(update);
                ModelState.AddModelError("ColorId", "This id has no color");
                return View(update);
            }

            ICollection<ProductColor> colorToRemove = existed.ProductColors
                .Where(pc => !update.ColorIds.Contains(pc.ColorId))
                .ToList();
            _context.ProductColors.RemoveRange(colorToRemove);

            ICollection<ProductColor> colorToAdd = update.ColorIds
                .Except(existed.ProductColors.Select(pc => pc.ColorId))
                .Select(colorId => new ProductColor { ColorId = colorId })
                .ToList();
            existed.ProductColors.AddRange(colorToAdd);

            bool resultSize = await _context.Sizes.AnyAsync(ps => update.SizeIds.Contains(ps.Id));
            if (!resultSize)
            {
                UpdatePopulateDropdowns(update);
                ModelState.AddModelError("SizeId", "This id has no size");
                return View(update);
            }

            ICollection<ProductSize> sizeToRemove = existed.ProductSizes
                .Where(ps => !update.SizeIds.Contains(ps.SizeId))
                .ToList();
            _context.ProductSizes.RemoveRange(sizeToRemove);

            ICollection<ProductSize> sizeToAdd = update.SizeIds
                .Except(existed.ProductSizes.Select(ps => ps.SizeId))
                .Select(sizeId => new ProductSize { SizeId = sizeId })
                .ToList();
            existed.ProductSizes.AddRange(sizeToAdd);


            if (update.MainPhoto is not null)
            {
                if (!update.MainPhoto.ValidateType())
                {
                    UpdatePopulateDropdowns(update);
                    ModelState.AddModelError("MainPhoto", "File Not supported");
                    return View(update);
                }

                if (!update.MainPhoto.ValidataSize(10))
                {
                    UpdatePopulateDropdowns(update);
                    ModelState.AddModelError("MainPhoto", "Image should not be larger than 10 mb");
                    return View(update);
                }
            }

            if (update.MainPhoto is not null)
            {
                string fileName = await update.MainPhoto.CreateFileAsync(_env.WebRootPath, "img");
                ProductImages prMain = existed.ProductImages.FirstOrDefault(pi => pi.IsPrimary == true);
                prMain.Url.DeleteFileAsync(_env.WebRootPath, "img");
                _context.ProductImages.Remove(prMain);

                existed.ProductImages.Add(new ProductImages
                {
                    IsPrimary = true,
                    Url = fileName
                });
            }

            if (existed.ProductImages is null) existed.ProductImages = new List<ProductImages>();

            if (update.ImageIds is null) update.ImageIds = new List<int>();

            ICollection<ProductImages> remove = existed.ProductImages
                .Where(pi => pi.IsPrimary == null && !update.ImageIds.Exists(imgId => imgId == pi.Id)).ToList();
            foreach (ProductImages image in remove)
            {
                image.Url.DeleteFileAsync(_env.WebRootPath, "img");
                existed.ProductImages.Remove(image);
            }

            TempData["Message"] = "";

            if (update.Photos is not null)
            {
                foreach (IFormFile photo in update.Photos)
                {
                    if (!photo.ValidateType())
                    {
                        TempData["Message"] += $"<p class=\"text-danger\">{photo.Name} type is not suitable</p>";
                        continue;
                    }

                    if (!photo.ValidataSize(10))
                    {
                        TempData["Message"] += $"<p class=\"text-danger\">{photo.Name} the size is not suitable</p>";
                        continue;
                    }

                    existed.ProductImages.Add(new ProductImages
                    {
                        IsPrimary = null,
                        Url = await photo.CreateFileAsync(_env.WebRootPath, "img")
                    });
                }
            }
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<UpdateProductVM, Product>()
                    .ForMember(dest => dest.ProductImages, opt => opt.Ignore());
            });
            var mapper = config.CreateMapper();

            mapper.Map(update, existed);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        [Authorize(Roles = "Admin,Moderator")]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> More(int id)
        {
            if (id <= 0) throw new WrongRequestException("The request sent does not exist");
            Product product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.ProductColors).ThenInclude(p => p.Color)
                .Include(p => p.ProductImages)
                .Include(p => p.ProductSizes).ThenInclude(p => p.Size)
                .FirstOrDefaultAsync(c => c.Id == id);
            if (product == null) throw new NotFoundException("Your request was not found");

            ProductVM vM = _mapper.Map<ProductVM>(product);

            return View(vM);
        }
    }
}
