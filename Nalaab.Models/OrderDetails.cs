using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Nalaab.Models
{
   
        public class OrderDetails
        {
            [Key]
            public int Id { get; set; }

            [Required]
            public int OrderId { get; set; }
            [ForeignKey("OrderId")]
            public OrderHeader OrderHeader { get; set; }


            [Required]
            public int ProductId { get; set; }
            [ForeignKey("ProductId")]
            public Product Product { get; set; }

            [Display(Name ="العدد")]
            public int Count { get; set; }

           [Display(Name = "السعر")]
           public double Price { get; set; }
        }
   
}
