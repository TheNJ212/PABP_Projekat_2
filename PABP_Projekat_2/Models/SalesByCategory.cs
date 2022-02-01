using System;
using System.Collections.Generic;

#nullable disable

namespace PABP_Projekat_2.Models
{
    public partial class SalesByCategory
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string ProductName { get; set; }
        public decimal? ProductSales { get; set; }
    }
}
