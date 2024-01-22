using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OnlineMarket.Extension;
using OnlineMarket.Helper;
using OnlineMarket.ModelViews;
using Stripe;
using Stripe.Checkout;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using WebApplication1;
using AspNetCoreHero.ToastNotification.Abstractions;

public class StripeOptions
{
    public string option { get; set; }
}
namespace OnlineMarket.Controllers
{
    public class CheckoutApiController : Controller
    {
        public double TyGiaUSD = 2480;
        public INotyfService _notyfService { get; }

        public CheckoutApiController(INotyfService notifyfService)
        {
            _notyfService = notifyfService;
        }

        public List<CartItem> GioHang
        {
            get
            {
                var gh = HttpContext.Session.Get<List<CartItem>>("GioHang");
                if (gh == default(List<CartItem>))
                {
                    gh = new List<CartItem>();
                }
                return gh;
            }
        }

        [Route("/create-checkout-session.html")]
        [HttpPost]
        public ActionResult Create()
        {
            var gh = HttpContext.Session.Get<List<CartItem>>("GioHang");
            var hostname = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}";
            var domain = $"{hostname}";
            var productCreateOptions = new ProductCreateOptions();
            //var productCreateOptions = new ProductCreateOptions
            //{
            //    Name = "T-shirt",
            //    Description = "Comfortable cotton t-shirt",
            //};
            foreach(var item in gh)
            {
                
                productCreateOptions.Name = item.product.ProductName;
                productCreateOptions.Description = item.product.ShortDesc.ToString();
                productCreateOptions.Url = item.product.Alias.ToString();
            }
            var productService = new ProductService();
            var product = productService.Create(productCreateOptions);
            
            var priceCreateOptions = new PriceCreateOptions();
            foreach(var item in gh)
            {
                priceCreateOptions.Product = product.Id;
                priceCreateOptions.UnitAmount = ((item.product.Discount > 0 && item.product.Discount < item.product.Price ? item.product.Discount.Value : item.product.Price.Value) / 248);
                //priceCreateOptions.UnitAmount = item.product.Price.Value / 233;
                priceCreateOptions.Currency = "usd";
 
            }
            var priceService = new PriceService();
            var price = priceService.Create(priceCreateOptions);



            var options = new SessionCreateOptions()
            {
                PaymentMethodTypes = new List<string>
                {
                    "card",
                },
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
                SuccessUrl = domain + "/success.html",
                CancelUrl = domain + "/cancel.html",
            };
            foreach(var item in gh)
            {
                options.LineItems.Add(new SessionLineItemOptions()
                {
                   
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        
                        Currency = "usd",
                        UnitAmount = ((item.product.Discount > 0 && item.product.Discount < item.product.Price ? item.product.Discount.Value : item.product.Price.Value ) / 248),
                        //UnitAmount = item.product.Price.Value / 233,
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.product.ProductName,
                        }
                    },
                    //Price =price.Id,
                    AdjustableQuantity = new SessionLineItemAdjustableQuantityOptions
                    {
                        Enabled = true,
                        Minimum = 1,
                        Maximum = 10,
                    },
                    Quantity=item.quantity,
                });

            }
            var service = new SessionService();
            Session session = service.Create(options);

            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);
        }
        [Route("stripe.html")]
        public IActionResult Index()
        {
            return View(GioHang);
        }
        [Route("success.html")]
        public IActionResult Success()
        {
            HttpContext.Session.Remove("GioHang");
            _notyfService.Success("Thanh toán Stripe thành công");
            return View();
        }
    }
 
}
