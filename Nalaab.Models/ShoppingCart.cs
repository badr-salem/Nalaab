using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Nalaab.Models
{
    public class ShoppingCart
    {
        public ShoppingCart()
        {
            Count = 1;
        }
        [Key]
        public int Id { get; set; }

        public string ApplicationUserId { get; set; }
        [ForeignKey("ApplicationUserId")]
        public ApplicationUser ApplicationUser { get; set; }

        [Display(Name = "رقم المنتج")]
        public int ProductId { get; set; }
        [ForeignKey("ProductId")]
        public Product Product { get; set; }

        [Range(1, 1000, ErrorMessage = "من فضلك أدخل قيمة بين 1 و 1000")]
        [Display(Name = "العدد")]
        public int Count { get; set; }

        [NotMapped]
        [Display(Name = "السعر")]
        public double Price { get; set; }

    }
}
