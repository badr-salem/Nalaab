using System;
using System.Collections.Generic;
using System.Text;

namespace Nalaab.Models.ViewModels
{
    public class IndexVM
    {
        public IEnumerable<Product> Product { get; set; }
        public IEnumerable<Coupon> Coupon { get; set; }
       
    }
}
