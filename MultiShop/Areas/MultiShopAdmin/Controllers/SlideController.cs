﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MultiShop.Areas.MultiShopAdmin.Controllers
{
    [Area("MultiShopAdmin")]
    [Authorize(Roles = "Admin,Moderator")]
    [AutoValidateAntiforgeryToken]
    public class SlideController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}