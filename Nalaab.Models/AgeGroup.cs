using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Nalaab.Models
{
    public class AgeGroup
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "الفئة العمرية")]
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }
    }
}
