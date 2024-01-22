using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using OnlineMarket.Enums;
using OnlineMarket.Models;
using System.Collections.Generic;
using System.Linq;

namespace OnlineMarket.Controllers.Components
{
    public class CategoriesViewComponent : ViewComponent
    {
        private readonly OnlineShopContext _context;
        private IMemoryCache _memoryCache;
        public CategoriesViewComponent(OnlineShopContext context, IMemoryCache memoryCache)
        {
            _context = context;
            _memoryCache = memoryCache;

        }
        public IViewComponentResult Invoke()
        {
            var _lsProduct = _memoryCache.GetOrCreate(CacheKeys.Categories, entry =>
            {
                entry.SlidingExpiration = System.TimeSpan.FromSeconds(1);
                return GetlsCategories();

            });
            return View(_lsProduct);
        }
        public List<Category> GetlsCategories()
        {
            List<Category> lsDanhMuc = new List<Category>();
            lsDanhMuc = _context.Categories.Where(x => x.Published == true).OrderByDescending(x => x.Ordering).ToList();
            return lsDanhMuc;
        }
    }
}
