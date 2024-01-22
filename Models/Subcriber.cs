using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineMarket.Models
{
    public partial class Subcriber
    {
        public int Id { get; set; }
        [MaxLength(150)]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        public string FullName { get; set; }
        public string Phone { get; set; }

    }
}
