using System;
using System.ComponentModel.DataAnnotations;

namespace PinnovationTech.Models
{
    public class Login
    {
        [Required(ErrorMessage = "Please enter email")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        [StringLength(250)]
        [Display(Name = "Email")]
        public string EmailNo { set; get; }

        [Required(ErrorMessage = "Please enter password")]
        [StringLength(500)]
        [Display(Name = "Password")]
        public string Password { set; get; }
    }
}
