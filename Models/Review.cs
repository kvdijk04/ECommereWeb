using System;
using System.ComponentModel.DataAnnotations;

#nullable disable

namespace OnlineMarket.Models
{
    public partial class Review
    {
        public int Id { get; set; }
        [StringLength(60, MinimumLength = 3)]
        [Required(ErrorMessage = "Vui lòng nhập Họ và tên")]
        public string FullName { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập Email")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        public string Email { get; set; }
        [MaxLength(11)]
        [Required(ErrorMessage = "Vui lòng nhập Số điện thoại")]
        [Display(Name = "Điện thoại")]
        [DataType(DataType.PhoneNumber)]
        public string Phone { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập bình luận")]
        public string Review1 { get; set; }
        public DateTime? CreateDay { get; set; }
    }
}
