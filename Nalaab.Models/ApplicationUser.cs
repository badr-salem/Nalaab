using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Nalaab.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [Display(Name = "الإسم")]
        public string Name { get; set; }

        [Display(Name = "العنوان")]
        public string StreetAddress { get; set; }

        [Display(Name = "المدينة")]
        public string City { get; set; }

        [Display(Name = "الحي")]
        public string State { get; set; }

        [Display(Name = "الرمز البريدي")]
        public string PostalCode { get; set; }

        [NotMapped]
        [Display(Name = "الدور")]
        public string Role { get; set; }
    }
}
