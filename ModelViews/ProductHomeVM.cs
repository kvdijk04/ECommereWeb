using OnlineMarket.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineMarket.ModelViews
{
    public class ProductHomeVM
    {
        public Category category { get; set; }
        public List<Product> lsProducts { get; set; }
    }
}
