using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OnlineMarket.Helper;
using OnlineMarket.Models;
using PagedList.Core;

namespace OnlineMarket.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AdminProductsController : Controller
    {
        private readonly OnlineShopContext _context;
        public INotyfService _notyfService { get; }

        public AdminProductsController(OnlineShopContext context, INotyfService notifyfService)
        {
            _context = context;
            _notyfService = notifyfService;
        }

        // GET: Admin/AdminProducts
        public IActionResult Index(int page = 1,int CatID = 0)
        {
            var pageNumber = page;
            var pageSize = 10;

            List<Product> lsProducts = new List<Product>();
            if(CatID != 0)
            {
               lsProducts = _context.Products.AsNoTracking().Where(x=>x.CatId==CatID).Include(x => x.Cat).OrderByDescending(x => x.ProductId).ToList();
            }
            else
            {
               
               lsProducts = _context.Products.AsNoTracking().Include(x => x.Cat).OrderByDescending(x => x.CatId).ToList();
            }

            
            PagedList<Product> models = new PagedList<Product>(lsProducts.AsQueryable(), pageNumber, pageSize);
            ViewBag.CurrentCateID = CatID;
            ViewBag.CurrentPage = pageNumber;
            
            ViewData["DanhMuc"] = new SelectList(_context.Categories, "CatId", "CatName",CatID);

            return View(models);
        }

        public IActionResult Filtter(int CatID = 0)
        {
            var url = $"/Admin/AdminProducts?CatID={CatID}";
            if (CatID == 0)
            {
                url = $"/Admin/AdminProducts";
            }

            return Json(new { status = "success", redirectUrl = url });
        }

        // GET: Admin/AdminProducts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            /*note*/
            var product = await _context.Products
                .Include(p => p.Cat)
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Admin/AdminProducts/Create
        public IActionResult Create()
        {
            ViewData["DanhMuc"] = new SelectList(_context.Categories, "CatId", "CatName");
            return View();
        }

        // POST: Admin/AdminProducts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProductId,ProductName,ShortDesc,Description,CatId,Price,Discount,Thumb,Video,DateCreated,DateModified,BestSeller,HomeTag,Active,Tags,Title,Alias,MetaDesc,MetaKey,UnitslnStock")] Product product,Microsoft.AspNetCore.Http.IFormFile fThumb)
        {
            if (ModelState.IsValid)
            {
                product.ProductName = Utilities.ToTitleCase(product.ProductName);

                if (string.IsNullOrEmpty(product.ProductName) && string.IsNullOrEmpty(product.ShortDesc) && string.IsNullOrEmpty(product.Description) &&  string.IsNullOrEmpty(product.Thumb) && string.IsNullOrEmpty(product.Video) && string.IsNullOrEmpty(product.Tags) && string.IsNullOrEmpty(product.Title) && string.IsNullOrEmpty(product.Alias) && string.IsNullOrEmpty(product.MetaDesc) && string.IsNullOrEmpty(product.MetaKey))
                {
                    _notyfService.Error("Vui lòng nhập đầy đủ thông tin");
                    return RedirectToAction(nameof(Create));
                }

                if (product.Price == null && product.Discount == null && product.UnitslnStock == null)
                {
                    _notyfService.Error("Vui lòng nhập đầy đủ thông tin");
                    return RedirectToAction(nameof(Create));
                }

                if (fThumb != null)
                {
                    string extension = Path.GetExtension(fThumb.FileName);
                    string image = Utilities.SEOUrl(product.ProductName) + extension;
                    product.Thumb = await Utilities.UploadFile(fThumb, @"products", image.ToLower());
                    
                }
                if (product.Discount >= product.Price)
                {
                    _notyfService.Error("Giá khuyến mãi phải nhỏ hơn giá gốc");
                    return RedirectToAction(nameof(Edit));

                }

                if (product.UnitslnStock < 0)
                {
                    _notyfService.Error("Số lượng sản phẩm không hợp lệ");
                    return RedirectToAction(nameof(Edit));
                }
                
                if (string.IsNullOrEmpty(product.Thumb))
                {
                    product.Thumb = "default.jpg";
                }
                product.Alias = Utilities.SEOUrl(product.ProductName);
                product.DateModified = DateTime.Now;
                product.DateCreated = DateTime.Now;

                _context.Add(product);
                await _context.SaveChangesAsync();
                _notyfService.Success("Thêm sản phẩm thành công");
                return RedirectToAction(nameof(Index));
            }
            ViewData["DanhMuc"] = new SelectList(_context.Categories, "CatId", "CatName", product.CatId);
            return View(product);
        }

        // GET: Admin/AdminProducts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            ViewData["DanhMuc"] = new SelectList(_context.Categories, "CatId", "CatName", product.CatId);
            return View(product);
        }

        // POST: Admin/AdminProducts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProductId,ProductName,ShortDesc,Description,CatId,Price,Discount,Thumb,Video,DateCreated,DateModified,BestSeller,HomeTag,Active,Tags,Title,Alias,MetaDesc,MetaKey,UnitslnStock")] Product product, Microsoft.AspNetCore.Http.IFormFile fThumb)
        {
            if (id != product.ProductId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    product.ProductName = Utilities.ToTitleCase(product.ProductName);
                    if (fThumb != null)
                    {
                        string extension = Path.GetExtension(fThumb.FileName);
                        string image = Utilities.SEOUrl(product.ProductName) + extension;
                        product.Thumb = await Utilities.UploadFile(fThumb, @"products", image.ToLower());

                    }
                    
                     if (product.Discount >= product.Price)
                    {
                        _notyfService.Error("Giá khuyến mãi phải nhỏ hơn giá gốc");
                        return RedirectToAction(nameof(Edit));

                    }

                    if (product.UnitslnStock < 0)
                    {
                        _notyfService.Error("Số lượng sản phẩm không hợp lệ");
                        return RedirectToAction(nameof(Edit));
                    }

                    if (string.IsNullOrEmpty(product.Thumb))
                    {
                        product.Thumb = "default.jpg";
                    }
                    product.Alias = Utilities.SEOUrl(product.ProductName);
                    product.DateModified = DateTime.Now;

                    _context.Update(product);
                    _notyfService.Success("Cập nhật thành công");
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.ProductId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["DanhMuc"] = new SelectList(_context.Categories, "CatId", "CatName", product.CatId);
            return View(product);
        }


        // GET: Admin/AdminProducts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            /*note*/
            var product = await _context.Products
                .Include(p => p.Cat)
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Admin/AdminProducts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            _notyfService.Success("Gỡ sản phẩm thành công");
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.ProductId == id);
        }
    }
}
