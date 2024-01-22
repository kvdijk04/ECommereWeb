//using AspNetCore.SEOHelper.Sitemap;
using AspNetCoreHero.ToastNotification.Abstractions;
using MailChimp;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OnlineMarket.Models;
using OnlineMarket.ModelViews;
using PagedList.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using WebApplication1.Models;


namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly OnlineShopContext _context;
        private readonly IWebHostEnvironment _env;
        public INotyfService _notyfService { get; }

        public HomeController(ILogger<HomeController> logger, OnlineShopContext context, IWebHostEnvironment env, INotyfService notifyfService)
        {
            _logger = logger;
            _context = context;
            _env = env;
            _notyfService = notifyfService;

        }

        public IActionResult Index()
        {
            HomeViewVM model = new HomeViewVM();

            var lsProducts = _context.Products
                .AsNoTracking()
                .Where(x => x.Active == true && x.HomeTag == true &&x.BestSeller==true)
                .OrderByDescending(x => x.ProductName)
                
                .ToList();

            List<ProductHomeVM> lsProductViews = new List<ProductHomeVM>();

            var lsCats = _context.Categories
                .AsNoTracking()
                .Where(x => x.Published == true)
                .OrderByDescending(x => x.Ordering)
                .ToList();

            foreach(var item in lsCats)
            {
                ProductHomeVM productHome = new ProductHomeVM();
                productHome.category = item;
                productHome.lsProducts = lsProducts.Where(x => x.CatId == item.CatId).ToList();
                lsProductViews.Add(productHome);
            }

            var TinTuc = _context.Posts
                .AsNoTracking()
                .Where(x => x.Published == true && x.IsNewFeed == true)
                .OrderByDescending(x => x.CreateDate)
                .ToList();
            var NewProducts = _context.Products
                        .AsNoTracking()
                        .Where(x => x.Active == true && x.HomeTag == true)
                        .OrderByDescending(x => x.DateCreated)
                        .ToList();
            model.Products = lsProductViews;
            model.TinTucs = TinTuc;
            model.lsNewProduct = NewProducts;
            ViewBag.AllProducts = lsProducts;

            return View(model);
        }
        [Route("contact.html", Name = "Contact")]
        public IActionResult Contact()
        {
            return View();
        }
        [Route("trang-chu/xem-nhanh/{Alias}-{id}")]
        public IActionResult QuickView(int id)
        {
            try
            {
                var product = _context.Products.Include(x => x.Cat).FirstOrDefault(x => x.ProductId == id);
                if (product == null)
                {
                    return RedirectToAction("Index");
                }
                var lsProduct = _context.Products
                    .AsNoTracking()
                    .Where(x => x.CatId == product.CatId && x.ProductId != id && x.Active == true)
                    .OrderByDescending(x => x.DateCreated)
                    .Take(4)
                    .ToList();

                ViewBag.SanPham = lsProduct;
                return View(product);
            }
            catch
            {
                return RedirectToAction("Index", "Home");
            }

        }
        [Route("about.html", Name = "About")]
        public IActionResult About()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });

        }
        // GET: Subcribers/Create
        public IActionResult _Subscribe()
        {
            return View();
        }
        // POST: Subcribers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Email,FullName,Phone")] Subcriber subcriber)
        {

            if (ModelState.IsValid)
            {
                if (subcriber.Email == null || subcriber.FullName == null || subcriber.Phone == null)
                {
                    _notyfService.Warning("Vui lòng điền đầy đủ thông tin!");

                }
                else
                {
                    Subcriber esubcriber = _context.Subcriber.Where(x => x.Email == subcriber.Email).FirstOrDefault();
                    if (esubcriber != null)
                    {
                        _notyfService.Error("Email này đã được đăng ký!");
                    }
                    else
                    {
                        _context.Add(subcriber);
                        await _context.SaveChangesAsync();
                        _notyfService.Success("Bạn đã đăng ký thành công");

                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(subcriber);
        }

    }
}
