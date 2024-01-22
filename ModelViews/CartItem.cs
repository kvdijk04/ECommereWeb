using OnlineMarket.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineMarket.ModelViews
{
    public class CartItem
    {
        public Product product { get; set; }
        public int quantity { get; set; }
        public int price { get; set; }
        public int? Discount { get; set; }
        public double TotalMoney => quantity * product.Price.Value;

    }
}
