using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using System.Text;

namespace Nalaab.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "التصنيف")]
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }
    }
}
