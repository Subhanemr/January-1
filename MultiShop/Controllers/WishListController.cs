using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MultiShop.DAL;
using MultiShop.Models;
using MultiShop.Utilities.Exceptions;
using MultiShop.ViewModels;
using Newtonsoft.Json;
using System.Security.Claims;

namespace MultiShop.Controllers
{
    public class WishListController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public WishListController(AppDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            ICollection<WishListItemVM> wishLists = new List<WishListItemVM>();

            if (User.Identity.IsAuthenticated)
            {
                AppUser appUser = await _userManager.Users
                    .Include(b => b.WishListItems)
                    .ThenInclude(p => p.Product)
                    .ThenInclude(pi => pi.ProductImages.Where(pi => pi.IsPrimary == true))
                    .FirstOrDefaultAsync(u => u.Id == User.FindFirstValue(ClaimTypes.NameIdentifier));
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


            return View(wishLists);
        }

        public async Task<IActionResult> AddWishList(int id)
        {
            if (id <= 0) throw new WrongRequestException("The request sent does not exist");
            Product product = await _context.Products.FirstOrDefaultAsync(x => x.Id == id);
            if (product == null) throw new NotFoundException("Your request was not found");
            ICollection<WishListCookieItemVM> cart;
            ICollection<WishListItemVM> cartItems;

            if (User.Identity.IsAuthenticated)
            {
                AppUser appUser = await _userManager.Users.Include(p => p.WishListItems).FirstOrDefaultAsync(u => u.Id == User.FindFirstValue(ClaimTypes.NameIdentifier));
                if (appUser == null) throw new NotFoundException("Your request was not found");
                WishListItem item = appUser.WishListItems.FirstOrDefault(b => b.ProductId == id);
                if (item == null)
                {
                    item = new WishListItem
                    {
                        AppUserId = appUser.Id,
                        ProductId = product.Id,
                        Price = product.Price
                    };

                    appUser.WishListItems.Add(item);
                }
                await _context.SaveChangesAsync();
            }
            else
            {
                if (Request.Cookies["WishListMultiShop"] is not null)
                {
                    cart = JsonConvert.DeserializeObject<ICollection<WishListCookieItemVM>>(Request.Cookies["WishListMultiShop"]);

                    WishListCookieItemVM item = cart.FirstOrDefault(c => c.Id == id);
                    if (item == null)
                    {
                        WishListCookieItemVM cartCookieItem = new WishListCookieItemVM
                        {
                            Id = id
                        };
                        cart.Add(cartCookieItem);
                    }
                }
                else
                {
                    cart = new List<WishListCookieItemVM>();
                    WishListCookieItemVM cartCookieItem = new WishListCookieItemVM
                    {
                        Id = id
                    };
                    cart.Add(cartCookieItem);
                }

                string json = JsonConvert.SerializeObject(cart);
                Response.Cookies.Append("WishListMultiShop", json, new CookieOptions
                {
                    Expires = DateTimeOffset.Now.AddDays(1),
                });

            }

            return RedirectToAction(nameof(Index), "Home");
        }

        public async Task<IActionResult> DeleteItem(int id)
        {
            if (id <= 0) throw new WrongRequestException("The request sent does not exist");

            if (User.Identity.IsAuthenticated)
            {
                AppUser appUser = await _userManager.Users
                    .Include(p => p.WishListItems)
                    .FirstOrDefaultAsync(u => u.Id == User.FindFirstValue(ClaimTypes.NameIdentifier));

                if (appUser == null) throw new NotFoundException("Your request was not found");

                WishListItem item = appUser.WishListItems.FirstOrDefault(b => b.ProductId == id);

                if (item == null) throw new WrongRequestException("The request sent does not exist");

                _context.WishListItems.Remove(item);

                await _context.SaveChangesAsync();
            }
            else
            {
                ICollection<WishListCookieItemVM> cart = JsonConvert.DeserializeObject<ICollection<WishListCookieItemVM>>(Request.Cookies["WishListMultiShop"]);

                WishListCookieItemVM item = cart.FirstOrDefault(c => c.Id == id);

                if (item == null) throw new WrongRequestException("The request sent does not exist");

                cart.Remove(item);


                string json = JsonConvert.SerializeObject(cart);
                Response.Cookies.Append("WishListMultiShop", json, new CookieOptions
                {
                    Expires = DateTimeOffset.Now.AddDays(1)
                });

            }

            return RedirectToAction(nameof(Index));
        }
    }
}
