using OnlineMarket.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineMarket.ModelViews
{
    public class ReportViewModel
    {
        public string DimensionOne { get; set; }
        public int Quantity { get; set; }

        public virtual Comment Comment { get; set; }
    }
}
