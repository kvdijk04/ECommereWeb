using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineMarket.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineMarket.Areas.Admin.Controllers
{
    [Area("Admin")]
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
                return PartialView("ListProductsSearchPartial", null);
            }
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
        [HttpPost]
        public IActionResult FindProductId(string id)
        {
            List<Product> ls = new List<Product>();

            if (string.IsNullOrEmpty(id) || id.Length < 1)
            {
                return PartialView("ListProductsSearchPartial", null);
            }
            ls = _context.Products
                .AsNoTracking()
                .Include(a => a.Cat)
                .Where(x => x.ProductId.ToString().Contains(id))
                .OrderBy(x => x.ProductId)
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
        [HttpPost]
        public IActionResult FindReview(string keyword)
        {
            List<Comment> ls = new List<Comment>();

            if (string.IsNullOrEmpty(keyword) || keyword.Length < 1)
            {
                return PartialView("ListReviewSearchPartial", null);
            }
            ls = _context.Comments
                .AsNoTracking()
                .Include(x => x.Product)
                .Where(x => x.Product.ProductName.Contains(keyword))
                .OrderByDescending(x => x.Product.ProductName)
                .ToList();
            if (ls == null)
            {
                return PartialView("ListReviewSearchPartial", null);
            }
            else
            {
                return PartialView("ListReviewSearchPartial", ls);
            }
        }
        [HttpPost]
        public IActionResult FindReviewRate(string id)
        {
            List<Comment> ls = new List<Comment>();

            if (string.IsNullOrEmpty(id) || id.Length < 1)
            {
                return PartialView("ListReviewSearchPartial", null);
            }
            ls = _context.Comments
                .AsNoTracking()
                .Include(x => x.Product)
                .Where(x => x.Rating.ToString().Contains(id))
                .OrderByDescending(x => x.Rating)
                .ToList();
            if (ls == null)
            {
                return PartialView("ListReviewSearchPartial", null);
            }
            else
            {
                return PartialView("ListReviewSearchPartial", ls);
            }
        }

        [HttpPost]
        public IActionResult FindOrderID(string id)
        {
            List<Order> ls = new List<Order>();

            if (string.IsNullOrEmpty(id) || id.Length < 1)
            {
                return PartialView("ListOrderSearchPartial", null);
            }
            ls = _context.Orders
                .AsNoTracking()
                .Include(x => x.Customer)
                .Include(x => x.TransactStatus)
                .Where(x => x.OrderId.ToString().Contains(id))
                .OrderBy(x => x.OrderDate)
                .ToList();
            if (ls == null)
            {
                return PartialView("ListOrderSearchPartial", null);
            }
            else
            {
                return PartialView("ListOrderSearchPartial", ls);
            }
        }

        [HttpPost]
        public IActionResult FindOrderName(string keyword)
        {
            List<Order> ls = new List<Order>();

            if (string.IsNullOrEmpty(keyword) || keyword.Length < 1)
            {
                return PartialView("ListOrderSearchPartial", null);
            }

            ls = _context.Orders
                .Include(x => x.Customer)
                .Include(x => x.TransactStatus)
                .Where(x => x.Customer.FullName.Contains(keyword))
                .ToList();
            if (ls == null)
            {
                return PartialView("ListOrderSearchPartial", null);
            }
            else
            {
                return PartialView("ListOrderSearchPartial", ls);
            }

        }
    }
}
