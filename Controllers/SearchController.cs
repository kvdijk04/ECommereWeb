using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineMarket.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PagedList.Core;
using OnlineMarket.Extension;

namespace OnlineMarket.Controllers
{
    public class SearchController : Controller
    {
        private readonly OnlineShopContext _context;
        public SearchController(OnlineShopContext context)
        {
            _context = context;
        }


        [HttpPost]
        public IActionResult FindProduct(string keyword)
        {
            List<Product> ls = new List<Product>();

            if (string.IsNullOrEmpty(keyword) || keyword.Length < 1)
            {
                return PartialView("_ListProductsSearchPartial", null);
            }
            ls = _context.Products
                .AsNoTracking()
                .Include(a => a.Cat)
                .Where(x => x.ProductName.Contains(keyword) || x.Cat.CatName.Contains(keyword))
                .OrderBy(x => x.ProductName)
                .ToList();
            
            if (ls == null)
            {
                return PartialView("_ListProductsSearchPartial", null);
            }
            else
            {
                return PartialView("_ListProductsSearchPartial", ls);
            }
        }
        public IActionResult FilterCategory(string keyword)
        {
            List<Product> ls = new List<Product>();

            if (string.IsNullOrEmpty(keyword) || keyword.Length < 1)
            {
                return PartialView("_ListProductPartialView", null);
            }
            ls = _context.Products
                .AsNoTracking()
                .Include(a => a.Cat)
                .Where(x => x.ProductName.Contains(keyword))
                .OrderBy(x => x.ProductName)
                .ToList();
            if (ls == null)
            {
                return PartialView("_ListProductPartialView", null);
            }
            else
            {
                return PartialView("_ListProductPartialView", ls);
            }
        }
        public IActionResult Filtter(int BestSeller = 0)
        {
            var url = $"/Product?BestSeller={BestSeller}";
            if (BestSeller == 0)
            {
                url = $"/Product";
            }

            return Json(new { status = "success", redirectUrl = url });
        }
        public IActionResult FindProductByPriceTo1M()
        {
            List<Product> ls = new List<Product>();
            ls = _context.Products
                .AsNoTracking()
                .Include(a => a.Cat)
                .Where(x => x.Discount > 0 && x.Discount <= 1000000 ? x.Discount <= x.Price : x.Price > 0 && x.Price <= 1000000)
                .ToList();
            
            if (ls == null)
            {
                return PartialView("_ListProductsSearchPartial", null);
            }
            else
            {
                return PartialView("_ListProductsSearchPartial", ls);
            }
        }
        public IActionResult FindProductByPriceTo3M()
        {
            List<Product> ls = new List<Product>();
            ls = _context.Products
                .AsNoTracking()
                .Include(a => a.Cat)
                .Where(x => x.Discount > 1000000 && x.Discount <= 3000000 ? x.Discount <= x.Price : (x.Price > 1000000 && x.Price <= 3000000 && x.Discount == 0))
                .ToList();
            if (ls == null)
            {
                return PartialView("_ListProductsSearchPartial", null);
            }
            else
            {
                return PartialView("_ListProductsSearchPartial", ls);
            }
        }
        public IActionResult FindProductByPriceTo5M()
        {
            List<Product> ls = new List<Product>();
            ls = _context.Products
                .AsNoTracking()
                .Include(a => a.Cat)
                .Where(x =>  x.Discount > 3000000 && x.Discount <= 5000000 ? x.Discount <= x.Price : (x.Price > 3000000 && x.Price <= 5000000 && x.Discount ==0))
                .ToList();

            if (ls == null)
            {
                return PartialView("_ListProductsSearchPartial", null);
            }
            else
            {
                return PartialView("_ListProductsSearchPartial", ls);
            }

        }
        public IActionResult FindProductByPriceToAll()
        {
            List<Product> ls = new List<Product>();
            ls = _context.Products
                .AsNoTracking()
                .Include(a => a.Cat)
                .Where(x => x.Discount > 5000000 ? x.Discount <= x.Price : (x.Price > 5000000 && x.Discount == 0))
                .ToList();
            if (ls == null)
            {
                return PartialView("_ListProductsSearchPartial", null);
            }
            else
            {
                return PartialView("_ListProductsSearchPartial", ls);
            }

        }
        public IActionResult FindProductBS()
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
                return PartialView("_ListProductsSearchPartial", null);
            }
            else
            {
                return PartialView("_ListProductsSearchPartial", ls);
            }

        }
        public IActionResult FindProductByPriceDown()
        {
            List<Product> ls = new List<Product>();
            ls = _context.Products
                .AsNoTracking()
                .Include(a => a.Cat)
                .Where(x => x.Discount > 0 ? x.Discount <= x.Price : x.Price > 0)
                .OrderByDescending(x => x.Discount > 0 ? x.Discount : x.Price)
                .ToList();
            if (ls == null)
            {
                return PartialView("_ListProductsSearchPartial", null);
            }
            else
            {
                return PartialView("_ListProductsSearchPartial", ls);
            }

        }
        public IActionResult FindProductByPriceUp()
        {
            List<Product> ls = new List<Product>();
            ls = _context.Products
                .AsNoTracking()
                .Include(a => a.Cat)
                .Where(x => x.Discount > 0 ? x.Discount <= x.Price : x.Price > 0)
                .OrderBy(x => x.Discount > 0 ? x.Discount : x.Price)
                .ToList();
            if (ls == null)
            {
                return PartialView("_ListProductsSearchPartial", null);
            }
            else
            {
                return PartialView("_ListProductsSearchPartial", ls);
            }

        }
        //best seller theo mệnh giá
        public IActionResult FindProductBS5M()
        {
            List<Product> ls = new List<Product>();
            ls = _context.Products
                .AsNoTracking()
                .Include(a => a.Cat)
                .Where(x => x.Discount > 0 && x.Discount > 3000000 && x.Discount <= 5000000 && x.BestSeller ? x.Discount <= x.Price : x.Price > 0 && x.Price > 3000000 && x.Price <= 5000000 && x.BestSeller)
                .OrderByDescending(x => x.ProductName)
                .ToList();
            if (ls == null)
            {
                return PartialView("_ListProductsSearchPartial", null);
            }
            else
            {
                return PartialView("_ListProductsSearchPartial", ls);
            }

        }
        public IActionResult FindProductBSAll()
        {
            List<Product> ls = new List<Product>();
            ls = _context.Products
                .AsNoTracking()
                .Include(a => a.Cat)
                .Where(x => x.Discount > 5000000 && x.BestSeller ? x.Discount <= x.Price : (x.Price > 5000000 && x.Discount == 0 && x.BestSeller))
                .OrderByDescending(x => x.ProductName)
                .ToList();
            if (ls == null)
            {
                return PartialView("_ListProductsSearchPartial", null);
            }
            else
            {
                return PartialView("_ListProductsSearchPartial", ls);
            }

        }
        public IActionResult FindProductBS3M()
        {
            List<Product> ls = new List<Product>();
            ls = _context.Products
                .AsNoTracking()
                .Include(a => a.Cat)
                .Where(x => x.Discount > 0 && x.Discount > 1000000 && x.Discount <= 3000000  && x.BestSeller ? x.Discount <= x.Price : x.Price > 0 && x.Price > 1000000 && x.Price <= 3000000 && x.BestSeller)
                .OrderByDescending(x => x.ProductName)
                .ToList();
            if (ls == null)
            {
                return PartialView("_ListProductsSearchPartial", null);
            }
            else
            {
                return PartialView("_ListProductsSearchPartial", ls);
            }

        }
        public IActionResult FindProductBS1M()
        {
            List<Product> ls = new List<Product>();
            ls = _context.Products
                .AsNoTracking()
                .Include(a => a.Cat)
                .Where(x => x.Discount > 0 && x.Discount <= 1000000 && x.BestSeller ? x.Discount <= x.Price : x.Price > 0 && x.Price <= 1000000 && x.BestSeller)
                .OrderByDescending(x => x.ProductName)
                .ToList();
            if (ls == null)
            {
                return PartialView("_ListProductsSearchPartial", null);
            }
            else
            {
                return PartialView("_ListProductsSearchPartial", ls);
            }

        }
        //-----------------------------------------------------------
        //Gía giảm dần theo mệnh giá
        public IActionResult GiamDan1M()
        {
            List<Product> ls = new List<Product>();
            ls = _context.Products
                .AsNoTracking()
                .Include(a => a.Cat)
                .Where(x => x.Discount > 0 && x.Discount <= 1000000 ? x.Discount <= x.Price : x.Price > 0 && x.Price <= 1000000)
                .OrderByDescending(x => x.Discount > 0 ? x.Discount : x.Price)
                .ToList();
            if (ls == null)
            {
                return PartialView("_ListProductsSearchPartial", null);
            }
            else
            {
                return PartialView("_ListProductsSearchPartial", ls);
            }

        }
        public IActionResult GiamDan3M()
        {
            List<Product> ls = new List<Product>();
            ls = _context.Products
                .AsNoTracking()
                .Include(a => a.Cat)
                .Where(x => x.Discount > 0 && x.Discount > 1000000 && x.Discount <= 3000000 ? x.Discount <= x.Price : x.Price > 0 && x.Price > 1000000 && x.Price <= 3000000)
                .OrderByDescending(x => x.Discount > 0 ? x.Discount : x.Price)
                .ToList();
            if (ls == null)
            {
                return PartialView("_ListProductsSearchPartial", null);
            }
            else
            {
                return PartialView("_ListProductsSearchPartial", ls);
            }

        }
        public IActionResult GiamDan5M()
        {
            List<Product> ls = new List<Product>();
            ls = _context.Products
                .AsNoTracking()
                .Include(a => a.Cat)
                .Where(x => x.Discount > 0 && x.Discount > 3000000 && x.Discount <= 5000000 ? x.Discount <= x.Price : x.Price > 0 && x.Price > 3000000 && x.Price <= 5000000)
                .OrderByDescending(x => x.Discount > 0 ? x.Discount : x.Price)
                .ToList();
            if (ls == null)
            {
                return PartialView("_ListProductsSearchPartial", null);
            }
            else
            {
                return PartialView("_ListProductsSearchPartial", ls);
            }

        }
        public IActionResult GiamDan7M()
        {
            List<Product> ls = new List<Product>();
            ls = _context.Products
                .AsNoTracking()
                .Include(a => a.Cat)
                .Where(x => x.Discount > 5000000 ? x.Discount <= x.Price : (x.Price > 5000000 && x.Discount == 0))
                .OrderByDescending(x => x.Discount > 0 ? x.Discount : x.Price)
                .ToList();
            if (ls == null)
            {
                return PartialView("_ListProductsSearchPartial", null);
            }
            else
            {
                return PartialView("_ListProductsSearchPartial", ls);
            }

        }
        //---------------------------------------------------------
        //Gía tăng dần theo mệnh giá
        public IActionResult TangDan1M()
        {
            List<Product> ls = new List<Product>();
            ls = _context.Products
                .AsNoTracking()
                .Include(a => a.Cat)
                .Where(x => x.Discount > 0 && x.Discount <= 1000000 ? x.Discount <= x.Price : x.Price > 0 && x.Price <= 1000000)
                .OrderBy(x => x.Discount > 0 ? x.Discount : x.Price)
                .ToList();
            if (ls == null)
            {
                return PartialView("_ListProductsSearchPartial", null);
            }
            else
            {
                return PartialView("_ListProductsSearchPartial", ls);
            }

        }
        public IActionResult TangDan3M()
        {
            List<Product> ls = new List<Product>();
            ls = _context.Products
                .AsNoTracking()
                .Include(a => a.Cat)
                .Where(x => x.Discount > 0 && x.Discount > 1000000 && x.Discount <= 3000000 ? x.Discount <= x.Price : x.Price > 0 && x.Price > 1000000 && x.Price <= 3000000)
                .OrderBy(x => x.Discount > 0 ? x.Discount : x.Price)
                .ToList();
            if (ls == null)
            {
                return PartialView("_ListProductsSearchPartial", null);
            }
            else
            {
                return PartialView("_ListProductsSearchPartial", ls);
            }

        }
        public IActionResult TangDan5M()
        {
            List<Product> ls = new List<Product>();
            ls = _context.Products
                .AsNoTracking()
                .Include(a => a.Cat)
                .Where(x => x.Discount > 0 && x.Discount > 3000000 && x.Discount <= 5000000 ? x.Discount <= x.Price : x.Price > 0 && x.Price > 3000000 && x.Price <= 5000000)
                .OrderBy(x => x.Discount > 0 ? x.Discount : x.Price)
                .ToList();
            if (ls == null)
            {
                return PartialView("_ListProductsSearchPartial", null);
            }
            else
            {
                return PartialView("_ListProductsSearchPartial", ls);
            }

        }
        public IActionResult TangDan7M()
        {
            List<Product> ls = new List<Product>();
            ls = _context.Products
                .AsNoTracking()
                .Include(a => a.Cat)
                .Where(x => x.Discount > 5000000 ? x.Discount <= x.Price : (x.Price > 5000000 && x.Discount == 0))
                .OrderBy(x => x.Discount > 0 ? x.Discount : x.Price)
                .ToList();
            if (ls == null)
            {
                return PartialView("_ListProductsSearchPartial", null);
            }
            else
            {
                return PartialView("_ListProductsSearchPartial", ls);
            }

        }
        //----------------------------------------------------
        //Best seller với giá tăng dần
        public IActionResult BSAndTangDan()
        {
            List<Product> ls = new List<Product>();
            ls = _context.Products
                .AsNoTracking()
                .Include(a => a.Cat)
                .Where(x => x.Discount > 0 && x.BestSeller ? x.Discount  <= x.Price : x.Price > 0 && x.BestSeller)
                .OrderBy(x => x.Discount > 0 ? x.Discount : x.Price)
                /*.Where(x => x.BestSeller)
                .OrderBy(x => x.Price)*/
                .ToList();
            if (ls == null)
            {
                return PartialView("_ListProductsSearchPartial", null);
            }
            else
            {
                return PartialView("_ListProductsSearchPartial", ls);
            }

        }
        //----------------------------------------------
        //best seller với giá giảm dần
        public IActionResult BSAndGiamDan()
        {
            List<Product> ls = new List<Product>();
            ls = _context.Products
                .AsNoTracking()
                .Include(a => a.Cat)
                .Where(x => x.Discount > 0 && x.BestSeller ? x.Discount <= x.Price : x.Price > 0 && x.BestSeller)
                .OrderByDescending(x => x.Discount > 0 ? x.Discount : x.Price)
                /*.Where(x=> x.BestSeller)
                .OrderByDescending(x => x.Price)*/
                .ToList();
            if (ls == null)
            {
                return PartialView("_ListProductsSearchPartial", null);
            }
            else
            {
                return PartialView("_ListProductsSearchPartial", ls);
            }

        }
        //------------------------------
        public IActionResult FindProductByPriceDownBS5MNo5M()
        {
            List<Product> ls = new List<Product>();
            ls = _context.Products
                .AsNoTracking()
                .Include(a => a.Cat)
                .Where(x => x.BestSeller)
                .OrderByDescending(x => x.Price)
                .ToList();
            if (ls == null)
            {
                return PartialView("_ListProductsSearchPartial", null);
            }
            else
            {
                return PartialView("_ListProductsSearchPartial", ls);
            }

        }

        public IActionResult FindProductByPriceDownBS5M()
        {
            List<Product> ls = new List<Product>();
            ls = _context.Products
                .AsNoTracking()
                .Include(a => a.Cat)
                .Where(x => x.BestSeller && x.Price > 3000000 && x.Price <= 5000000)
                .OrderByDescending(x => x.Price )
                .ToList();
            if (ls == null)
            {
                return PartialView("_ListProductsSearchPartial", null);
            }
            else
            {
                return PartialView("_ListProductsSearchPartial", ls);
            }

        }
        public IActionResult FindProductByPriceUpBS5M()
        {
            List<Product> ls = new List<Product>();
            ls = _context.Products
                .AsNoTracking()
                .Include(a => a.Cat)
                .Where(x => x.Price > 3000000 && x.Price <= 5000000 && x.BestSeller)
                .OrderBy(x => x.Price)
                .ToList();
            if (ls == null)
            {
                return PartialView("_ListProductsSearchPartial", null);
            }
            else
            {
                return PartialView("_ListProductsSearchPartial", ls);
            }

        }

        //Sort by discount
        public IActionResult FindProductByDiscount()
        {
            List<Product> ls = new List<Product>();
            ls = _context.Products
                .AsNoTracking()
                .Include(a => a.Cat)
                .Where(x => x.Discount > 0)
                .OrderBy(x => x.Discount)
                .ToList();
            if (ls == null)
            {
                return PartialView("_ListProductsSearchPartial", null);
            }
            else
            {
                return PartialView("_ListProductsSearchPartial", ls);
            }

        }

        public IActionResult FindProductByDiscount1M()
        {
            List<Product> ls = new List<Product>();
            ls = _context.Products
                .AsNoTracking()
                .Include(a => a.Cat)
                .Where(x => x.Discount > 0 && x.Discount <= 1000000)
                .OrderBy(x => x.Discount)
                .ToList();
            if (ls == null)
            {
                return PartialView("_ListProductsSearchPartial", null);
            }
            else
            {
                return PartialView("_ListProductsSearchPartial", ls);
            }

        }

        public IActionResult FindProductByDiscount3M()
        {
            List<Product> ls = new List<Product>();
            ls = _context.Products
                .AsNoTracking()
                .Include(a => a.Cat)
                .Where(x => x.Discount > 1000000 && x.Discount <= 3000000)
                .OrderBy(x => x.Discount)
                .ToList();
            if (ls == null)
            {
                return PartialView("_ListProductsSearchPartial", null);
            }
            else
            {
                return PartialView("_ListProductsSearchPartial", ls);
            }

        }
    }
}
