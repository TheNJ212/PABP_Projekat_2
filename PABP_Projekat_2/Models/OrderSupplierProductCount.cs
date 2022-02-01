using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PABP_Projekat_2.Models
{
    public class OrderSupplierProductCount
    {
        public Order Order { get; set; }
        public Supplier Supplier { get; set; }
        public int ProductCount { get; set; }
    }
}
