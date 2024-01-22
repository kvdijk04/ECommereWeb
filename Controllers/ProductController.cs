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

namespace OnlineMarket.Controllers
{
    public class ProductController : Controller
    {
        private readonly OnlineShopContext _context;
        public ProductController(OnlineShopContext context)
        {
            _context = context;
        }

        [Route("shop.html", Name = "ShopProduct")]
        public IActionResult Index(int? page,int CatID=0)
        {
            try
            {
                var lsnewproducts = _context.Products.AsNoTracking().OrderByDescending(x => x.DateCreated).ToList();

                var pageNumber = page == null || page <= 0 ? 1 : page.Value;
                var pageSize = 6;
                var lsProducts = _context.Products.AsNoTracking().OrderByDescending(x => x.DateCreated);


                PagedList<Product> models = new PagedList<Product>(lsProducts, pageNumber, pageSize);
                ViewBag.CurrentPage = pageNumber;
                ViewBag.CurrentCateID = CatID;
                ViewBag.AllNewProducts = lsnewproducts;

                ViewData["DanhMuc"] = new SelectList(_context.Categories, "CatId", "CatName", CatID);
                return View(models);

            }
            catch
            {
                return RedirectToAction("Index", "Home");
            }
            ViewBag.CurrentCateID = CatID;
            ViewData["DanhMuc"] = new SelectList(_context.Categories, "CatId", "CatName", CatID);

        }

        [Route("/{Alias}", Name = "ListProduct")]
        public IActionResult List(string Alias,int page = 1, int CatID=0)
        {
            try
            {
                var pageSize = 6;
                var categories = _context.Categories.AsNoTracking().SingleOrDefault(x => x.Alias == Alias);
                var lsProducts = _context.Products
                    .AsNoTracking()
                    .Where(x => x.CatId == categories.CatId)
                    .OrderByDescending(x => x.DateCreated);
                PagedList<Product> models = new PagedList<Product>(lsProducts, page, pageSize);
                var lsnewproducts = _context.Products.AsNoTracking().Where(x => x.CatId == categories.CatId).OrderByDescending(x => x.DateCreated).ToList();

                ViewBag.CurrentPage = page;
                ViewBag.CurrentCategory = categories;
                ViewBag.CurrentCateID = CatID;
                ViewBag.AllNewProducts = lsnewproducts;
                ViewData["DanhMuc"] = new SelectList(_context.Categories, "CatId", "CatName", CatID);
                return View(models);
            }
            catch
            {
                return RedirectToAction("Index", "Home");
            }
            ViewBag.CurrentCateID = CatID;
            ViewData["DanhMuc"] = new SelectList(_context.Categories, "CatId", "CatName", CatID);

        }
        [Route("san-pham/xem-nhanh/{Alias}-{id}")]
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
                    .ToList();

                ViewBag.SanPham = lsProduct;
                return View(product);
            }
            catch
            {
                return RedirectToAction("Index", "Home");
            }

        }
        [Route("/{Alias}-{id}", Name = "ProductDetails")]
        public IActionResult Details(int id)
        {
            try
            {
                var lsComments = _context.Comments.Include(x => x.Product).OrderByDescending(x => x.CommentedOn).ToList();
                var product = _context.Products.Include(x => x.Cat).FirstOrDefault(x => x.ProductId == id);
                var lsCommentsPId = _context.Comments.Where(x => x.ProductId == product.ProductId).ToList();
                if (product == null)
                {
                    return RedirectToAction("Index");
                }
                var lsProduct = _context.Products
                    .AsNoTracking()
                    .Where(x => x.CatId == product.CatId && x.ProductId != id && x.Active == true)
                    .OrderByDescending(x => x.DateCreated)
                    .ToList();

                ViewBag.SanPham = lsProduct;
                ViewBag.AllComments = lsComments;
                ViewBag.AllComments = lsCommentsPId;

                return View(product);
            }
            catch
            {
                return RedirectToAction("Index", "Home");
            }

        }
        public IActionResult FindProduct(string keyword)
        {
            List<Product> ls = new List<Product>();
            ls = _context.Products
                .AsNoTracking()
                .Include(a => a.Cat)
                .Where(x => x.ProductName.Contains(keyword))
                .OrderByDescending(x => x.ProductName)
                .ToList();
            
            if (ls == null)
            {
                return PartialView("ListProductsSearchPartial", null);
            }
            else
            {
                return PartialView("ListProductsSearchPartial", ls);
            }
        }
        public IActionResult FindProductByBS(string keyword)
        {
            List<Product> ls = new List<Product>();
            ls = _context.Products
                .AsNoTracking()
                .Include(a => a.Cat)
                .Where(x => x.BestSeller)
                .OrderByDescending(x => x.ProductName)
                .ToList();
            if (ls == null)
            {
                return PartialView("ListProductsSearchPartial", null);
            }
            else
            {
                return PartialView("ListProductsSearchPartial", ls);
            }
        }

        public IActionResult Filtter(int CatID = 0)
        {
            var url = $"/Product/Index?CatID={CatID}";
            if (CatID == 0)
            {
                url = $"/Product/Index";
            }

            return Json(new { status = "success", redirectUrl = url });
        }
    }
}
