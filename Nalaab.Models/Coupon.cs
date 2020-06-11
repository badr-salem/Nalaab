using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Nalaab.Models
{
    public class Coupon
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name ="كود الخصم")]
        public string Name { get; set; }

        [Required]
        [Display(Name ="نوع الخصم")]
        public string CouponType { get; set; }

        public enum ECouponType { Percent = 0, Riyal = 1 }

        [Required]
        [Display(Name = "الخصم")]
        public double Discount { get; set; }

        [Required]
        [Display(Name ="القيمة الأدنى")]
        public double MinimumAmount { get; set; }

        [Display(Name = "هل الكود فعال")]
        public bool IsActive { get; set; }
    }
}
