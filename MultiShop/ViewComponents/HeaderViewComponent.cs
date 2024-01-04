using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MultiShop.DAL;
using MultiShop.Models;
using MultiShop.ViewModels;
using Newtonsoft.Json;
using System.Security.Claims;

namespace MultiShop.ViewComponents
{
    public class HeaderViewComponent : ViewComponent
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _http;
        private readonly UserManager<AppUser> _userManager;
        private readonly IMapper _mapper;

        public HeaderViewComponent(AppDbContext context, IHttpContextAccessor http, UserManager<AppUser> userManager, IMapper mapper)
        {
            _context = context;
            _http = http;
            _userManager = userManager;
            _mapper = mapper;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            Dictionary<string, string> keyValuePairs = await _context.Settings.ToDictionaryAsync(p => p.Key, p => p.Value);
            ICollection<Category> categories = await _context.Categories.Where(c => c.Products.Count > 0).ToListAsync();
            ICollection<CategoryVM> vMs = _mapper.Map<ICollection<CategoryVM>>(categories);

            ICollection<CartItemVM> cartVM = new List<CartItemVM>();
            ICollection<WishListItemVM> wishLists = new List<WishListItemVM>();
            if (Request.Path.StartsWithSegments("/Account/MyOrders"))
            {
                AppUser user = await _userManager.Users
                    .Include(b => b.BasketItems.Where(bi => bi.OrderId != null))
                    .ThenInclude(p => p.Product)
                    .ThenInclude(p => p.ProductImages.Where(pi => pi.IsPrimary == true))
                    .FirstOrDefaultAsync(u => u.Id == _http.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
                foreach (BasketItem item in user.BasketItems ?? new List<BasketItem>())
                {
                    cartVM.Add(new CartItemVM
                    {
                        Id = item.ProductId,
                        Name = item.Product.Name,
                        Price = item.Product.Price,
                        Count = item.Count,
                        SubTotal = item.Count * item.Product.Price,
                        Image = item.Product.ProductImages.FirstOrDefault()?.Url
                    });
                }
            }
            else
            {
                if (_http.HttpContext.User.Identity.IsAuthenticated)
                {
                    AppUser user = await _userManager.Users
                        .Include(b => b.BasketItems.Where(bi => bi.OrderId == null))
                        .ThenInclude(p => p.Product)
                        .ThenInclude(p => p.ProductImages.Where(pi => pi.IsPrimary == true))
                        .FirstOrDefaultAsync(u => u.Id == _http.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
                    foreach (BasketItem item in user.BasketItems ?? new List<BasketItem>())
                    {
                        cartVM.Add(new CartItemVM
                        {
                            Id = item.ProductId,
                            Name = item.Product.Name,
                            Price = item.Product.Price,
                            Count = item.Count,
                            SubTotal = item.Count * item.Product.Price,
                            Image = item.Product.ProductImages.FirstOrDefault()?.Url
                        });
                    }
                }
                else
                {
                    if (_http.HttpContext.Request.Cookies["BasketMultiShop"] is not null)
                    {
                        ICollection<CartCookieItemVM> cart = JsonConvert.DeserializeObject<ICollection<CartCookieItemVM>>(_http.HttpContext.Request.Cookies["BasketMultiShop"]);
                        foreach (CartCookieItemVM cartCookieItemVM in cart)
                        {
                            Product product = await _context.Products.Include(p => p.ProductImages.Where(pi => pi.IsPrimary == true)).FirstOrDefaultAsync(p => p.Id == cartCookieItemVM.Id);
                            if (product is not null)
                            {
                                CartItemVM cartItemVM = new CartItemVM
                                {
                                    Id = cartCookieItemVM.Id,
                                    Name = product.Name,
                                    Price = product.Price,
                                    Image = product.ProductImages.FirstOrDefault().Url,
                                    Count = cartCookieItemVM.Count,
                                    SubTotal = Convert.ToDecimal(cartCookieItemVM.Count) * product.Price

                                };
                                cartVM.Add(cartItemVM);
                            }
                        }
                    }

                }
            }
            if (User.Identity.IsAuthenticated)
            {
                AppUser appUser = await _userManager.Users
                    .Include(b => b.WishListItems)
                    .ThenInclude(p => p.Product)
                    .ThenInclude(pi => pi.ProductImages.Where(pi => pi.IsPrimary == true))
                    .FirstOrDefaultAsync(u => u.Id == _http.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
                foreach (WishListItem item in appUser.WishListItems)
                {
                    wishLists.Add(new WishListItemVM
                    {
                        Id = item.ProductId,
                        Name = item.Product.Name,
                        Price = item.Product.Price,
                        Image = item.Product.ProductImages.FirstOrDefault()?.Url
                    });
                }
            }
            else
            {
                if (Request.Cookies["WishListMultiShop"] is not null)
                {
                    ICollection<WishListCookieItemVM> wishes = JsonConvert.DeserializeObject<ICollection<WishListCookieItemVM>>(Request.Cookies["WishListMultiShop"]);
                    foreach (WishListCookieItemVM wishListCookieItem in wishes)
                    {
                        Product product = await _context.Products.Include(p => p.ProductImages.Where(pi => pi.IsPrimary == true)).FirstOrDefaultAsync(p => p.Id == wishListCookieItem.Id);
                        if (product is not null)
                        {
                            WishListItemVM wish = new WishListItemVM
                            {
                                Id = wishListCookieItem.Id,
                                Name = product.Name,
                                Price = product.Price,
                                Image = product.ProductImages.FirstOrDefault().Url
                            };
                            wishLists.Add(wish);
                        }
                    }
                }

            }

            AppUser app = new AppUser();

            if (User.Identity.IsAuthenticated)
            {
                app = await _userManager.FindByNameAsync(User.Identity.Name);
            }

            HeaderVM headerVM = new HeaderVM { Settings = keyValuePairs, Items = cartVM, User = app, Categories = vMs, WishListItems = wishLists };

            return View(headerVM);
        }
    }
}
