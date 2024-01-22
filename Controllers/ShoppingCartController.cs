using AspNetCoreHero.ToastNotification.Abstractions;
using BraintreeHttp;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using OnlineMarket.Extension;
using OnlineMarket.Helper;
using OnlineMarket.Models;
using OnlineMarket.ModelViews;
using OnlineMarket.Others;
using PayPal.Core;
using PayPal.v1.Payments;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;



namespace OnlineMarket.Controllers
{
    public class ShoppingCartController : Controller
    {
        private readonly OnlineShopContext _context;
        public INotyfService _notyfService { get; }
        private readonly string _clientId;
        private readonly string _secretKey;
        public int TyGiaUSD = 23300;//store in Database

        public ShoppingCartController(OnlineShopContext context, INotyfService notifyfService, IConfiguration config)
        {
            _context = context;
            _notyfService = notifyfService;
            _clientId = config["PaypalSettings:ClientId"];
            _secretKey = config["PaypalSettings:SecretKey"];

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


        [HttpPost]
        [Route("api/cart/add")]
        public IActionResult AddToCart(int productID, int? quantity, int? Discount)
        {
            List<CartItem> cart = GioHang;
            // thêm sản phảm vào giỏ hàng
            try
            {
                CartItem item = cart.SingleOrDefault(p => p.product.ProductId == productID);
                if (item != null)
                {
                    item.quantity = item.quantity + quantity.Value;
                    //Lưu lại session
                    HttpContext.Session.Set<List<CartItem>>("GioHang", cart);
                }
                else
                {
                    Product hh = _context.Products.SingleOrDefault(p => p.ProductId == productID);
                    if (hh.Discount > 0)
                    {
                        item = new CartItem()
                        {
                            product = hh,
                            quantity = quantity.HasValue ? quantity.Value : 1,
                            Discount = hh.Discount.Value
                        };

                    }
                    if (hh.Price > 0)
                    {
                        item = new CartItem()
                        {
                            product = hh,
                            quantity = quantity.HasValue ? quantity.Value : 1,
                            Discount = 0
                        };
                    }
                    cart.Add(item);

                }


                // lưu lại session
                HttpContext.Session.Set<List<CartItem>>("GioHang", cart);
                _notyfService.Success("Sản phẩm đã được thêm vào giỏ hàng");
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false });
            }

        }

        [HttpPost]
        [Route("api/cart/update")]
        public IActionResult UpdateCart(int productID, int? quantity)
        {
            var cart = HttpContext.Session.Get<List<CartItem>>("GioHang");
            try
            {
                if (cart != null)
                {
                    CartItem item = cart.SingleOrDefault(p => p.product.ProductId == productID);
                    if (item != null && quantity.HasValue)
                    {
                        item.quantity = quantity.Value;
                    }
                    // Lưu lại session
                    HttpContext.Session.Set<List<CartItem>>("GioHang", cart);
                }
                return Json(new { success = true });
            }
            catch
            {
                return Json(new { success = false });
            }
        }



        [HttpPost]
        [Route("api/cart/remove")]
        public IActionResult Remove(int productID)
        {
            try
            {
                List<CartItem> gioHang = GioHang;
                CartItem item = gioHang.SingleOrDefault(p => p.product.ProductId == productID);
                if (item != null)
                {
                    gioHang.Remove(item);
                }
                // lưu lại session
                HttpContext.Session.Set<List<CartItem>>("GioHang", gioHang);
                return Json(new { success = true });
            }
            catch
            {
                return Json(new { success = false });
            }

        }


        [Route("cart.html", Name = "Cart")]
        public IActionResult Index(string returnUrl = null)
        {
            // lấy giỏ hàng ra để xử lý
            var cart = HttpContext.Session.Get<List<CartItem>>("GioHang");
            var taikhoanID = HttpContext.Session.GetString("CustomerId");
            MuaHangVM model = new MuaHangVM();
            if (taikhoanID != null)
            {
                var khachhang = _context.Customers.AsNoTracking().SingleOrDefault(x => x.CustomerId == Convert.ToInt32(taikhoanID));
                model.CustomerId = khachhang.CustomerId;
                model.FullName = khachhang.FullName;
                model.Email = khachhang.Email;
                model.Phone = khachhang.Phone;
                model.Address = khachhang.Address;
            }
            ViewData["lsTinhThanh"] = new SelectList(_context.Locations.Where(x => x.Levels == 1).OrderBy(x => x.Type).ToList(), "LocationId", "Name");
            ViewData["lsQuanHuyen"] = new SelectList(_context.Locations.Where(x => x.Levels == 2).OrderBy(x => x.Type).ToList(), "LocationId", "Name");
            ViewData["lsPhuongXa"] = new SelectList(_context.Locations.Where(x => x.Levels == 3).OrderBy(x => x.Name).ToList(), "LocationId", "Name");
            ViewBag.GioHang = cart;
            if (cart == null)
            {
                _notyfService.Information("Bạn chưa có sản phẩm nào");
                return RedirectToAction("Index", "Home");

            }
            return View(GioHang);
        }
        [Authorize]
        public ActionResult Payment(int? id)
        {
            var hostname = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}";
            var total = GioHang.Sum(x => x.product.Discount > 0 && x.product.Discount < x.product.Price ? x.product.Discount.Value * x.quantity : x.product.Price.Value * x.quantity);
            //request params need to request to MoMo system
            string endpoint = "https://test-payment.momo.vn/gw_payment/transactionProcessor";
            string partnerCode = "MOMOF2FF20220326";
            string accessKey = "5B9WlRDysPguI40l";
            string serectkey = "8egsq1LLx17aOpOIYpm5jfV4W0FaMdWV";
            string nameproduct = "";
            var gh = HttpContext.Session.Get<List<CartItem>>("GioHang");
            foreach (var item in gh)
            {
                nameproduct = nameproduct + item.product.ProductName + ", ";
            };
            string orderInfo = nameproduct;
            string returnUrl = "http://tudang-001-site1.atempurl.com/success-momo.html";
            string notifyurl = "http://baladf48beba.ngrok.io/Home/SavePayment"; //lưu ý: notifyurl không được sử dụng localhost, có thể sử dụng ngrok để public localhost trong quá trình test
            string amount = total.ToString();
            string orderid = DateTime.Now.Ticks.ToString();
            string requestId = DateTime.Now.Ticks.ToString();
            string extraData = "";

            //Before sign HMAC SHA256 signature
            string rawHash = "partnerCode=" +
                partnerCode + "&accessKey=" +
                accessKey + "&requestId=" +
                requestId + "&amount=" +
                amount + "&orderId=" +
                orderid + "&orderInfo=" +
                orderInfo + "&returnUrl=" +
                returnUrl + "&notifyUrl=" +
                notifyurl + "&extraData=" +
                extraData;

            MoMoSecurity crypto = new MoMoSecurity();
            //sign signature SHA256
            string signature = crypto.signSHA256(rawHash, serectkey);

            //build body json request
            JObject message = new JObject
            {
                { "partnerCode", partnerCode },
                { "accessKey", accessKey },
                { "requestId", requestId },
                { "orderId", orderid },
                { "orderInfo", orderInfo },
                { "amount",amount},
                { "returnUrl", returnUrl },
                { "notifyUrl", notifyurl },
                { "extraData", extraData },
                { "requestType", "captureMoMoWallet" },
                { "signature", signature }

            };

            string responseFromMomo = PaymentRequest.sendPaymentRequest(endpoint, message.ToString());
            JObject jmessage = JObject.Parse(responseFromMomo);
            return Redirect(jmessage.GetValue("payUrl").ToString());

            
        }
        
        //Khi thanh toán xong ở cổng thanh toán Momo, Momo sẽ trả về một số thông tin, trong đó có errorCode để check thông tin thanh toán
        //errorCode = 0 : thanh toán thành công (Request.QueryString["errorCode"])
        //Tham khảo bảng mã lỗi tại: https://developers.momo.vn/#/docs/aio/?id=b%e1%ba%a3ng-m%c3%a3-l%e1%bb%97i
        
        [Route("success-momo.html")]          
        public IActionResult ConfirmPaymentClient()
        {
            HttpContext.Session.Remove("GioHang");
            _notyfService.Success("Thanh toán MoMo thành công");
            return View();
        }
       

        [HttpPost]
        public void SavePayment()
        {
            //cập nhật dữ liệu vào db

        }

    }
}
