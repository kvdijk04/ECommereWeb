using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineMarket.ModelViews
{
    public class RegisterVM
    {
        [Key]
        public int CustomerId { get; set; }
        [Display(Name = "Họ Và Tên")]
        [Required(ErrorMessage = "Vui lòng nhập Họ Tên")]
        //name don't have number but have unicode
        [RegularExpression(@"^[\p{L}\s'.-]+$", ErrorMessage = "Họ Tên không được chứa số")]
        [StringLength(50, MinimumLength = 5, ErrorMessage = "Tên phải từ 5 đến 50 kí tự")]
        public string FullName { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập Email")]
        [MaxLength(150)]
        [DataType(DataType.EmailAddress)]
        [Remote(action: "ValidateEmail", controller: "Accounts")]
        public string Email { get; set; }
        [MaxLength(11)]
        [Required(ErrorMessage = "Vui lòng nhập Số điện thoại")]
        [RegularExpression(@"^(\+84|0)\d{9,10}$", ErrorMessage = "Số Điện Thoại không hợp lệ")]
        [Display(Name = "Điện thoại")]
        [DataType(DataType.PhoneNumber)]
        [Remote(action: "ValidatePhone", controller: "Accounts")]
        public string Phone { get; set; }
        [Display(Name = "Mật khẩu")]
        [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
        [MinLength(5, ErrorMessage = "Bạn cần đặt mật khẩu tối thiểu 5 ký tự")]
        public string Password { get; set; }
        [MinLength(5, ErrorMessage = "Bạn cần đặt mật khẩu tối thiểu 5 ký tự")]
        [Display(Name = "Nhập lại mật khẩu")]
        [Compare("Password", ErrorMessage = "Vui lòng nhập mật khẩu giống nhau")]
        public string ConfirmPassword { get; set; }
    }
}
