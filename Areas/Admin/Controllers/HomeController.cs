using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OnlineMarket.Models;
using OnlineMarket.ModelViews;
using PagedList.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Areas.Admin.Controllers
{
    [Area("Admin")]

    public class HomeController : Controller
    {
        private readonly OnlineShopContext _context;
        public INotyfService _notyfService { get; }

        public HomeController(OnlineShopContext context, INotyfService notifyfService)
        {
            _context = context;
            _notyfService = notifyfService;
        }
        public IActionResult Index(int page = 1, int CatID = 0)
        {
            var pageNumber = page;
            var pageSize = 10;

            List<Product> lsProducts = new List<Product>();
            if (CatID != 0)
            {
                lsProducts = _context.Products.AsNoTracking().Where(x => x.CatId == CatID).Include(x => x.Cat).OrderByDescending(x => x.ProductId).ToList();
            }
            else
            {

                lsProducts = _context.Products.AsNoTracking().Include(x => x.Cat).OrderByDescending(x => x.CatId).ToList();
            }


            PagedList<Product> models = new PagedList<Product>(lsProducts.AsQueryable(), pageNumber, pageSize);
            ViewBag.CurrentCateID = CatID;
            ViewBag.CurrentPage = pageNumber;

            ViewData["DanhMuc"] = new SelectList(_context.Categories, "CatId", "CatName", CatID);

            return View(models);
        }

    }
}
