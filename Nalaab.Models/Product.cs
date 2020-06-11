using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Nalaab.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [Display(Name = "اسم المنتج")]
        public string Title { get; set; }

        [Display(Name = "الوصف")]
        public string Description { get; set; }
        
        [Required]
        [Display(Name = "شركة الإنتاج")]
        public string ProductionCompany { get; set; }
        
        [Required]
        [Range(1, 10000)]
        [Display(Name = "السعر")]
        public double Price { get; set; }

        [Display(Name = "الصورة")]
        public string ImageUrl { get; set; }

        [Required]
        public int CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        [Display(Name = "التصنيف")]
        public Category Category { get; set; }

        [Required]
        public int AgeGroupId { get; set; }

        [ForeignKey("AgeGroupId")]
        [Display(Name = "الفئة العمرية")]
        public AgeGroup AgeGroup { get; set; }
    }
}
