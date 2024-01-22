using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineMarket.Extension;
using OnlineMarket.Helper;
using OnlineMarket.Models;
using OnlineMarket.ModelViews;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace OnlineMarket.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AdminLoginController : Controller
    {
        private readonly OnlineShopContext _context;
        public INotyfService _notyfService { get; }
        public AdminLoginController(OnlineShopContext context, INotyfService notifyfService)
        {
            _context = context;
            _notyfService = notifyfService;

        }
        public IActionResult Index()
        {

            return View();

        }



    }
}
