using AspNetCoreHero.ToastNotification.Abstractions;
using BraintreeHttp;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using OnlineMarket.Extension;
using OnlineMarket.Helper;
using OnlineMarket.Models;
using OnlineMarket.ModelViews;
using PayPal.Core;
using PayPal.v1.Payments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Security.Policy;
using static System.Net.WebRequestMethods;
using Azure.Core;
using HttpClient = BraintreeHttp.HttpClient;
using OnlineMarket.VnPay;
using System.Runtime.CompilerServices;

namespace OnlineMarket.Controllers
{
    [Authorize]
    public class CheckOutController : Controller
    {
        private readonly OnlineShopContext _context;
        public INotyfService _notyfService { get; }
        private readonly string _clientId;
        private readonly string _secretKey;
        string url;
        string returnUrl ;
        string tmnCode ;
        string hashSecret ;
        public int TyGiaUSD = 24800;//store in Database
        private readonly IHttpContextAccessor _contextAccessor;
        public CheckOutController(OnlineShopContext context, INotyfService notifyfService, IConfiguration config, IHttpContextAccessor contextAccessor)
        {
            _context = context;
            _notyfService = notifyfService;
            _contextAccessor = contextAccessor;
            _clientId = config["PaypalSettings:ClientId"];
            _secretKey = config["PaypalSettings:SecretKey"];
            string url = config["Url"];
            string returnUrl = config["ReturnUrl"];
            string tmnCode = config["TmnCode"];
            string hashSecret = config["HashSecret"];
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

        [Route("checkout.html", Name = "Checkout")]
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
            return View(model);
        }

        [HttpPost]
        [Authorize]
        [Route("checkout.html", Name = "Checkout")]
        public IActionResult Index(MuaHangVM muaHang)
        {
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

                khachhang.LocationId = muaHang.TinhThanh;
                khachhang.District = muaHang.QuanHuyen;
                khachhang.Ward = muaHang.PhuongXa;
                khachhang.Address = muaHang.Address;

                model.TinhThanh = Convert.ToInt32(khachhang.LocationId);
                model.QuanHuyen = Convert.ToInt32(khachhang.District);
                model.PhuongXa = Convert.ToInt32(khachhang.Ward);
                model.Address = khachhang.Address;
                _context.Update(khachhang);
            }
            if (muaHang.CustomerId == 0)
            {
                _notyfService.Error("Bạn chưa đăng nhập");
                return RedirectToAction("Index", "Home");
            }


            if (muaHang.FullName == null && muaHang.Email == null && muaHang.Phone == null && muaHang.Address == null)
            {
                _notyfService.Error("Bạn chưa nhập đầy đủ thông tin");
                return RedirectToAction("Index", "CheckOut");

            }

            if (muaHang.TinhThanh == 0)
            {
                _notyfService.Error("Bạn chưa chọn tỉnh thành");
                return RedirectToAction("Index", "CheckOut");
            }

            else
            {
                _context.SaveChanges();
            }

            try
            {
                if (ModelState.IsValid)
                {
                    var ex = ModelState.IsValid;
                    // khởi tạo đơn hàng
                    Models.Order donhang = new Models.Order();
                    donhang.CustomerId = model.CustomerId;
                    donhang.Address = model.Address;
                    donhang.LocationId = model.TinhThanh;
                    donhang.District = model.QuanHuyen;
                    donhang.Ward = model.PhuongXa;

                    donhang.OrderDate = DateTime.Now;
                    donhang.TransactStatusId = 1;
                    donhang.Deleted = false;
                    donhang.Paid = false;
                    donhang.Note = Utilities.StripHTML(model.Note);
                    donhang.TotalMoney = Convert.ToInt32(cart.Sum(x => x.TotalMoney));
                    _context.Add(donhang);
                    _context.SaveChanges();

                    // tạo mới ds đơn hàng
                    foreach (var item in cart)
                    {
                        OrderDetail orderDetail = new OrderDetail();
                        orderDetail.OrderId = donhang.OrderId;
                        orderDetail.ProductId = item.product.ProductId;
                        orderDetail.Quantity = item.quantity;
                        orderDetail.TotalMoney = donhang.TotalMoney;
                        orderDetail.Price = item.product.Price;
                        orderDetail.CreateDate = DateTime.Now;
                        _context.Add(orderDetail);
                        /* _context.OrderDetails.Attach(orderDetail);
                         _context.Entry(orderDetail).State = EntityState.Modified;*/
                    }

                    _context.SaveChanges();
                    // clear giỏ hàng
                    HttpContext.Session.Remove("GioHang");

                    _notyfService.Success("Đặt hàng thành công");

                    return RedirectToAction("Success");
                }

            }
            catch (Exception ex)
            {
                ViewData["lsTinhThanh"] = new SelectList(_context.Locations.Where(x => x.Levels == 1).OrderBy(x => x.Type).ToList(), "LocationId", "Name");
                ViewBag.GioHang = cart;
                return View(model);
            }
            ViewData["lsTinhThanh"] = new SelectList(_context.Locations.Where(x => x.Levels == 1).OrderBy(x => x.Type).ToList(), "LocationId", "Name");
            ViewBag.GioHang = cart;
            return View(model);
        }


        [Route("dat-hang-thanh-cong.html", Name = "Success")]
        public IActionResult Success()
        {
            try
            {
                var taikhoanID = HttpContext.Session.GetString("CustomerId");
                if (string.IsNullOrEmpty(taikhoanID))
                {
                    return RedirectToAction("Login", "Accounts", new { returnUrl = "/dat-hang-thanh-cong.html" });
                }
                var khachhang = _context.Customers.AsNoTracking().SingleOrDefault(x => x.CustomerId == Convert.ToInt32(taikhoanID));
                var donhang = _context.Orders.Where(x => x.CustomerId == Convert.ToInt32(taikhoanID)).OrderByDescending(x => x.OrderDate).FirstOrDefault();
                MuaHangSuccessVM successVM = new MuaHangSuccessVM();
                successVM.FullName = khachhang.FullName;
                successVM.DonHangID = donhang.OrderId;
                successVM.Phone = khachhang.Phone;
                successVM.Address = khachhang.Address;
                successVM.PhuongXa = GetNameLocation(donhang.Ward.Value);
                successVM.QuanHuyen = GetNameLocation(donhang.District.Value);
                successVM.TinhThanh = GetNameLocation(donhang.LocationId.Value);
                return View(successVM);
            }
            catch
            {
                return View();
            }
        }
        public async System.Threading.Tasks.Task<IActionResult> PaypalCheckout(MuaHangVM muaHang, string PayerID, string paymentId)
        {
            var environment = new SandboxEnvironment(_clientId, _secretKey);
            var client = new PayPalHttpClient(environment);
            
            var paymentExecution = new PaymentExecution()
            {
                PayerId = PayerID
            };
            var payment1 = new Payment()
            {
                Id = paymentId
            };
            var paypalOrderId = DateTime.Now.Ticks;
            var hostname = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}";
            #region Create Paypal Order
            var itemList = new ItemList()
            {
                Items = new List<Item>()
            };
            var amount = new Amount()
            {
                Details = new AmountDetails(),
            };
            //var total = Math.Round(GioHang.Sum(p => p.TotalMoney) / TyGiaUSD, 2);
            var total = GioHang.Sum(x => x.product.Discount > 0 && x.product.Discount < x.product.Price ? x.product.Discount.Value * x.quantity : x.product.Price.Value * x.quantity) / TyGiaUSD;
            foreach (var item in GioHang)
            {
                itemList.Items.Add(new Item()
                {
                    Name = item.product.ProductName,
                    Currency = "USD",
                    //Price = Math.Round(Convert.ToDecimal(item.product.Price) / TyGiaUSD, 2).ToString(),
                    Price = ((item.product.Discount > 0 && item.product.Discount < item.product.Price ? item.product.Discount.Value : item.product.Price.Value) / TyGiaUSD, 2).ToString(),
                    Quantity = item.quantity.ToString(),
                    Tax = "0",
                    Sku = "sku",

                });
            }
            #endregion
            var payment = new Payment()
            {
                Intent = "sale",
                Transactions = new List<Transaction>()
                {
                    new Transaction()
                    {
                        Amount = new Amount()
                        {
                            Total=total.ToString(),
                            Currency = "USD",
                            Details = new AmountDetails
                            {
                                Shipping = "0",
                                Tax="0",
                                Subtotal = total.ToString(),

                            },

                        },
                        //ItemList = itemList,
                        Description = $"Invoice #{paypalOrderId}",
                        InvoiceNumber = paypalOrderId.ToString()
                    }
                },
                RedirectUrls = new RedirectUrls()
                {
                    CancelUrl = $"{hostname}/thanh-toán-paypal-thất-bại",
                    //ReturnUrl = $"{hostname}/CheckOut/CheckoutSuccess"
                    ReturnUrl = $"{hostname}/success-paypal.html"
                },
                Payer = new Payer()
                {
                    PaymentMethod = "paypal"
                }
            };

         

            PaymentCreateRequest request = new PaymentCreateRequest();
            request.RequestBody(payment);        
            try
            {
                var response = await client.Execute(request);
                request.Content = new StringContent(JsonConvert.SerializeObject(payment), Encoding.UTF8, "application/json");
                var statusCode = response.StatusCode;
                Payment result = response.Result<Payment>();
                var links = result.Links.GetEnumerator();
                string paypalRedirectUrl = null;
            
                while (links.MoveNext())
                {
                    LinkDescriptionObject lnk = links.Current;
                    if (lnk.Rel.ToLower().Trim().Equals("approval_url"))
                    {
                        paypalRedirectUrl = lnk.Href;
                        
                    }
                }
                return Redirect(paypalRedirectUrl);
            }
            catch (HttpException httpException)
            {
                var statusCode = httpException.StatusCode;
                var debugId = httpException.Headers.GetValues("PayPal-Debug-Id").FirstOrDefault();

                //Process when Checkout with Paypal fails
                return Redirect("/ShoppingCart/CheckoutFail");
            }
                  
        }
        
    
 
        // private static async Task<MuaHangVM> ExecutePaypalPaymentAsync(HttpClient http, string paymentId, string payerId, string accessToken)
        // {
        //     var url = $"https://api.sandbox.paypal.com/v1/payments/payment/{paymentId}/execute";
        //     var request = new HttpRequestMessage(HttpMethod.Post, url);
        //     request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        //     request.Content = new StringContent($"{{\"payer_id\":\"{payerId}\"}}", Encoding.UTF8, "application/json");
        //     var response = await http.SendAsync(request);
        //     var content = await response.Content.ReadAsStringAsync();
        //     return JsonConvert.DeserializeObject<MuaHangVM>(content);
        // }


        [Route("thanh-toán-paypal-thất-bại")]
        public IActionResult CheckoutFail()
        {
            _notyfService.Error("Lỗi kết nối. Vui lòng thử lại!");
            return RedirectToAction("Index", "Home");
            //return View();
        }

        [Route("success-paypal.html")]
        public IActionResult CheckoutSuccess()
        {

            HttpContext.Session.Remove("GioHang");
            _notyfService.Success("Thanh toán PayPal thành công");
            return View();
        }

     
        public string GetNameLocation(int idlocation)
        {
            try
            {
                var location = _context.Locations.AsNoTracking().SingleOrDefault(x => x.LocationId == idlocation);
                if (location != null)
                {
                    return location.NameWithType;
                }
            }
            catch
            {
                return string.Empty;
            }
            return string.Empty;
        }

        public ActionResult VnPayPayment()
        {

            PayLib pay = new PayLib();
            var total = GioHang.Sum(x => x.product.Discount > 0 && x.product.Discount < x.product.Price ? x.product.Discount.Value * x.quantity : x.product.Price.Value * x.quantity);
            string orderInfo = null;
            foreach (var item in GioHang)
            {
                orderInfo += item.product.ProductName + ", SL: "+item.quantity.ToString()+ " - " ;
            }
            pay.AddRequestData("vnp_Version", "2.1.0"); //Phiên bản api mà merchant kết nối. Phiên bản hiện tại là 2.1.0
            pay.AddRequestData("vnp_Command", "pay"); //Mã API sử dụng, mã cho giao dịch thanh toán là 'pay'
            pay.AddRequestData("vnp_TmnCode", "S2322Q4I"); //Mã website của merchant trên hệ thống của VNPAY (khi đăng ký tài khoản sẽ có trong mail VNPAY gửi về)
            pay.AddRequestData("vnp_Amount", (total*100).ToString()); //số tiền cần thanh toán, công thức: số tiền * 100 - ví dụ 10.000 (mười nghìn đồng) --> 1000000
            pay.AddRequestData("vnp_BankCode", ""); //Mã Ngân hàng thanh toán (tham khảo: https://sandbox.vnpayment.vn/apis/danh-sach-ngan-hang/), có thể để trống, người dùng có thể chọn trên cổng thanh toán VNPAY
            pay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss")); //ngày thanh toán theo định dạng yyyyMMddHHmmss
            pay.AddRequestData("vnp_CurrCode", "VND"); //Đơn vị tiền tệ sử dụng thanh toán. Hiện tại chỉ hỗ trợ VND
            pay.AddRequestData("vnp_IpAddr", Util.GetClientIP(_contextAccessor)); //Địa chỉ IP của khách hàng thực hiện giao dịch
            pay.AddRequestData("vnp_Locale", "vn"); //Ngôn ngữ giao diện hiển thị - Tiếng Việt (vn), Tiếng Anh (en)
            pay.AddRequestData("vnp_OrderInfo", orderInfo); //Thông tin mô tả nội dung thanh toán
            pay.AddRequestData("vnp_OrderType", "other"); //topup: Nạp tiền điện thoại - billpayment: Thanh toán hóa đơn - fashion: Thời trang - other: Thanh toán trực tuyến
            pay.AddRequestData("vnp_ReturnUrl", "https://localhost:44379/thanh-toan-vnpay-thanh-cong"); //URL thông báo kết quả giao dịch khi Khách hàng kết thúc thanh toán
            pay.AddRequestData("vnp_TxnRef", DateTime.Now.Ticks.ToString()); //mã hóa đơn

            string paymentUrl = pay.CreateRequestUrl("https://sandbox.vnpayment.vn/paymentv2/vpcpay.html", "PCPFRQRJTIYXHWULGVXUYPCKVEVAWICB");

            return Redirect(paymentUrl);
        }
        [Route("/thanh-toan-vnpay-thanh-cong")]
        public IActionResult PaymentConfirm()
        {

            if (Request.QueryString.ToString().Length > 0)
            {
                string hashSecret = "PCPFRQRJTIYXHWULGVXUYPCKVEVAWICB"; //Chuỗi bí mật
                var vnpayData = _contextAccessor.HttpContext.Request.Query;
                PayLib pay = new PayLib();

                //lấy toàn bộ dữ liệu được trả về
                foreach (string s in vnpayData.Keys)
                {
                    if (!string.IsNullOrEmpty(s) && s.StartsWith("vnp_"))
                    {
                        pay.AddResponseData(s, vnpayData[s]);
                    }
                }
                long orderId = Convert.ToInt64(pay.GetResponseData("vnp_TxnRef")); //mã hóa đơn
                long vnpayTranId = Convert.ToInt64(pay.GetResponseData("vnp_TransactionNo")); //mã giao dịch tại hệ thống VNPAY
                string vnp_ResponseCode = pay.GetResponseData("vnp_ResponseCode"); //response code: 00 - thành công, khác 00 - xem thêm https://sandbox.vnpayment.vn/apis/docs/bang-ma-loi/
                string vnp_SecureHash = _contextAccessor.HttpContext.Request.Query["vnp_SecureHash"]; //hash của dữ liệu trả về

                bool checkSignature = pay.ValidateSignature(vnp_SecureHash, hashSecret); //check chữ ký đúng hay không?

                if (checkSignature)
                {
                    if (vnp_ResponseCode == "00")
                    {
                        //Thanh toán thành công
                        ViewBag.Message = "Thanh toán thành công hóa đơn " + orderId + " | Mã giao dịch: " + vnpayTranId;
                    }
                    else
                    {
                        //Thanh toán không thành công. Mã lỗi: vnp_ResponseCode
                        ViewBag.Message = "Có lỗi xảy ra trong quá trình xử lý hóa đơn " + orderId + " | Mã giao dịch: " + vnpayTranId + " | Mã lỗi: " + vnp_ResponseCode;
                    }
                }
                else
                {
                    ViewBag.Message = "Có lỗi xảy ra trong quá trình xử lý";
                }
            }

            return View();
        }
    }
}


