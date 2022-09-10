using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace OfficeLeaveCore.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required(ErrorMessage = "This field is required")]
        [Display(Name = "Full Name")]
        public string FullName { get; set; }

        [Required]
        [EmailAddress(ErrorMessage = "This field must be a valid email address")]
        [MaxLength(255)]
        public override string Email { get; set; }

        [NotMapped]
        public string Role { get; set; }


    }
}
