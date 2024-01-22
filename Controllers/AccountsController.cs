using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineMarket.Extension;
using OnlineMarket.Helper;
using OnlineMarket.Models;
using OnlineMarket.ModelViews;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace OnlineMarket.Controllers
{

    public class AccountsController : Controller
    {
        private readonly OnlineShopContext _context;
        public INotyfService _notyfService { get; }
        public AccountsController(OnlineShopContext context, INotyfService notifyfService)
        {
            _context = context;
            _notyfService = notifyfService;

        }
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ValidatePhone(string Phone)
        {
            try
            {
                var khachhang = _context.Customers.AsNoTracking().SingleOrDefault(x => x.Phone.ToLower() == Phone.ToLower());
                if(khachhang != null)
                {
                    return Json(data: "Số điện thoại : " + Phone + " Đã được sử dụng");
                }
                return Json(data: true);
            }
            catch
            {
                return Json(data: true);
            }
        }
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ValidateEmail(string Email)
        {
            try
            {
                var khachhang = _context.Customers.AsNoTracking().SingleOrDefault(x => x.Email.ToLower() == Email.ToLower());
                if (khachhang != null)
                {
                    return Json(data: "Email : " + Email + " Đã được sử dụng");
                }
                return Json(data: true);
            }
            catch
            {
                return Json(data: true);
            }
        }
        [Authorize]
        [Route("tai-khoan-cua-toi.html", Name = "Dashboard")]
        public IActionResult Dashboard()
        {
            var taikhoanID = HttpContext.Session.GetString("CustomerId");
            if (taikhoanID != null)
            {
                var khachhang = _context.Customers.AsNoTracking().SingleOrDefault(x => x.CustomerId == Convert.ToInt32(taikhoanID));
                if(khachhang != null)
                {
                    var lsOrder = _context.Orders
                        .AsNoTracking()
                        .Include(x => x.TransactStatus)
                        .Where(x => x.CustomerId == khachhang.CustomerId)
                        .OrderByDescending(x => x.OrderDate)
                        .ToList();
                    ViewBag.DonHang = lsOrder;
                    return View(khachhang);
                }
            }

            return RedirectToAction("Login");
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("dang-ky.html",Name ="DangKy")]
        public IActionResult DangKyTaiKhoan()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("dang-ky.html", Name = "DangKy")]
        public async Task<IActionResult> DangKyTaiKhoan(RegisterVM taikhoan)
        {
            try
            {
                if(ModelState.IsValid)
                {
                    string salt = Utilities.GetRandomKey();
                    Customer khachhang = new Customer
                    {
                        FullName = taikhoan.FullName,
                        Phone = taikhoan.Phone.Trim().ToLower(),
                        Email = taikhoan.Email.Trim().ToLower(),
                        Password = (taikhoan.Password + salt.Trim()).ToMD5(),
                        Active = true,
                        Salt = salt.Trim(),
                        CreateDate = DateTime.Now,
                        LastLogin=DateTime.Now,

                    };
                    try
                    {
                        Customer customer = _context.Customers.Where(x => x.Email == taikhoan.Email).FirstOrDefault();
                        if (customer != null)
                        {
                            _notyfService.Error("Email này đã được đăng ký");
                            return View();
                        }
                        else
                        {
                            _context.Add(khachhang);
                            await _context.SaveChangesAsync();
                            // Lưu Session MaKH
                            HttpContext.Session.SetString("CustomerId", khachhang.CustomerId.ToString());
                            var taikhoanID = HttpContext.Session.GetString("CustomerId");
                            // Identity
                            var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Name, khachhang.FullName),
                            new Claim("CustomerId", khachhang.CustomerId.ToString())
                        };
                            ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "login");
                            ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                            await HttpContext.SignInAsync(claimsPrincipal);
                            _notyfService.Success("Đăng ký tài khoản thành công ");
                            return RedirectToAction("Dashboard", "Accounts");
                        }

                    }
                    catch (Exception ex)
                    {
                        return RedirectToAction("DangKyTaiKhoan", "Accounts");
                    }
                }
                else
                {
                    return View(taikhoan);
                }
            }
            catch
            {
                return View(taikhoan);
            }

        }

        [AllowAnonymous]
        [Route("dang-nhap.html", Name ="DangNhap")]
        public IActionResult Login (string returnUrl = null)
        {
            var taikhoanID = HttpContext.Session.GetString("CustomerId");
            if (taikhoanID != null)
            {
                
                return RedirectToAction("Dashboard", "Accounts");
            }

            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("dang-nhap.html", Name = "DangNhap")]
        public async Task<IActionResult> Login (LoginViewModel customer)
        {
            try
            {
                if (ModelState.IsValid)
                {

                    bool isEmail = Utilities.IsValidEmail(customer.UserName);
                    if (!isEmail)
                    {
                        return View(customer);
                    }

                    var khachhang = _context.Customers.AsNoTracking().SingleOrDefault(x => x.Email.Trim() == customer.UserName);
                    var admin = _context.Accounts.AsNoTracking().SingleOrDefault(x => x.Email.Trim() == customer.UserName && x.RoleId == 1);
                    if (admin != null)
                    {
                        _notyfService.Success("Chào mừng admin");
                        return RedirectToAction("Index","AdminOrders", new { area = "Admin" });
                    }

                    if (khachhang == null)
                    {
                        return RedirectToAction("DangKyTaiKhoan");
                    }
                    string pass = (customer.Password + khachhang.Salt.Trim()).ToMD5();
                    if(khachhang.Password != pass)
                    {
                        _notyfService.Error("Thông tin đăng nhập không chính xác ");
                        return View(customer);
                    }

                    //Kiểm tra tài khoản có bị disable không?
                    if (khachhang.Active == false)
                    {
                        return RedirectToAction("ThongBao", "Accounts");
                    }
                    khachhang.LastLogin = DateTime.Now;
                    _context.Update(khachhang);
                    await _context.SaveChangesAsync();
                    //Lưu session vào MaKH
                    HttpContext.Session.SetString("CustomerId", khachhang.CustomerId.ToString());
                    var taikhoanID = HttpContext.Session.GetString("CustomerId");
                    //Identity
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name,khachhang.FullName),
                        new Claim("CustomerId", khachhang.CustomerId.ToString())
                        
                    };

                    ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "customer");
                    ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                    await HttpContext.SignInAsync(claimsPrincipal);

                    _notyfService.Success("Đăng nhập thành công");
                    
                    if (!string.IsNullOrEmpty(customer.ReturnUrl))
                    {
                        return Redirect(customer.ReturnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Dashboard", "Accounts");
                    }
                }
            }
            catch
            {
                return RedirectToAction("DangKyTaiKhoan", "Accounts");
            }
            return View(customer);
        }


        [HttpGet]
        [Route("dang-xuat.html",Name ="Logout")]
        public IActionResult Logout()
        {
            HttpContext.SignOutAsync();
            HttpContext.Session.Remove("CustomerId");
            return RedirectToAction("Index", "Home");
        }
        
        [HttpPost]
        public IActionResult ChangePassword(ChangePasswordViewModel model)
        {
            //check if old password is correct or not 
            var taikhoanID = HttpContext.Session.GetString("CustomerId");
            if (taikhoanID == null)
            {
                return RedirectToAction("DangNhap", "Accounts");
            }
            var khachhang = _context.Customers.Where(x => x.CustomerId == int.Parse(taikhoanID)).FirstOrDefault();
            if (khachhang == null)
            {
                return RedirectToAction("DangNhap", "Accounts");
            }
            string pass = (model.PasswordNow + khachhang.Salt.Trim()).ToMD5();
            if (khachhang.Password != pass)
            {
                _notyfService.Error("Mật khẩu cũ không chính xác");
                return RedirectToAction("Dashboard", "Accounts");
            }
            //check if new password and confirm password is match or not
            if (model.Password != model.ConfirmPassword)
            {
                _notyfService.Error("Mật khẩu mới không khớp");
                return RedirectToAction("Dashboard", "Accounts");
            }
            //update new password
            khachhang.Password = (model.Password + khachhang.Salt.Trim()).ToMD5();
            _context.Update(khachhang);
            _context.SaveChanges();
            _notyfService.Success("Đổi mật khẩu thành công");
            return RedirectToAction("Dashboard", "Accounts");
            
        }

    }
}
