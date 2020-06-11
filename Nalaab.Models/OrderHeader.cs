using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Nalaab.Models
{
    public class OrderHeader
    {
        [Key]
        public int Id { get; set; }

        public string ApplicationUserId { get; set; }
        [ForeignKey("ApplicationUserId")]
        public ApplicationUser ApplicationUser { get; set; }

        [Required]
        [Display(Name = "تاريخ الطلب")]
        public DateTime OrderDate { get; set; }
        [Required]
        [Display(Name = "تاريخ الشحن")]
        public DateTime ShippingDate { get; set; }

        [Required]
        [Display(Name = "إجمالي مبلغ الطلب المبدئي")]
        public double OrderTotalOriginal { get; set; }

        [Display(Name = "كود الخصم")]
        public string CouponCode { get; set; }

        [Display(Name = "نسبة الخصم")]
        public double CouponCodeDiscount { get; set; }

        [Required]
        [Display(Name = "إجمالي مبلغ الطلب النهائي")]
        public double OrderTotal { get; set; }

        [Display(Name = "رقم التتبع")]
        public string TrackingNumber { get; set; }

        [Display(Name = "شركة الشحن")]
        public string Carrier { get; set; }

        [Display(Name = "حالة الطلب")]
        public string OrderStatus { get; set; }

        [Display(Name = "حالة السداد")]
        public string PaymentStatus { get; set; }

        [Display(Name = "تاريخ السداد")]
        public DateTime PaymentDate { get; set; }

        [Display(Name = "رقم الحوالة")]
        public string TransactionId { get; set; }

        [Display(Name = "رقم الجوال")]
        [Required]
        public string PhoneNumber { get; set; }
        
        [Display(Name = "العنوان")]
        [Required]
        public string StreetAddress { get; set; }

        [Display(Name = "المدينة")]
        [Required]
        public string City { get; set; }

        [Display(Name = "الحي")]
        [Required]
        public string State { get; set; }

        [Display(Name = "الرمز البريدي")]
        [Required]
        public string PostalCode { get; set; }

        [Display(Name = "الإسم")]
        [Required]
        public string Name { get; set; }


    }
}
