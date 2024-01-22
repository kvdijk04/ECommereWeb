using Microsoft.AspNetCore.Mvc;
using OnlineMarket.Extension;
using OnlineMarket.ModelViews;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineMarket.Controllers.Components
{
    public class NumberCartViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            var cart = HttpContext.Session.Get<List<CartItem>>("GioHang");
           
            return View(cart);
        }
    }
}
