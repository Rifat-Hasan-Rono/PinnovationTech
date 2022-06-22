using System;
using System.ComponentModel.DataAnnotations;

namespace PinnovationTech.Models
{
    public class User
    {
        [Key]
        public int UserId { set; get; }

        [Required(ErrorMessage = "Please enter first name")]
        [StringLength(100)]
        [Display(Name = "First Name")]
        public string FName { set; get; }

        [StringLength(100)]
        [Display(Name = "Last Name")]
        public string LName { set; get; }

        [Required(ErrorMessage = "Please enter phone number")]
        //[StringLength(15)]
        [Display(Name = "Phone Number")]
        public int PhoneNo { set; get; }

        [Required(ErrorMessage = "Please enter email")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        [StringLength(250)]
        [Display(Name = "Email")]
        public string EmailNo { set; get; }

        [Range(0, int.MaxValue, ErrorMessage = "Please select city")]
        public int UserCity { set; get; }

        [StringLength(3000)]
        [Display(Name = "User Image")]
        public string UserImg { set; get; }

        [StringLength(3000)]
        [Display(Name = "User CV")]
        public string UserCV { set; get; }

        [Required(ErrorMessage = "Please enter password")]
        [StringLength(500, ErrorMessage = "Must be between 5 and 500 characters", MinimumLength = 5)]
        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        public string Password { set; get; }

        [Required(ErrorMessage = "Please re-enter password")]
        [StringLength(500, ErrorMessage = "Must be between 5 and 500 characters", MinimumLength = 5)]
        [Display(Name = "Confirm Password")]
        [DataType(DataType.Password)]
        [Compare("Password")]
        public string ConfirmPassword { set; get; }

        [Display(Name = "Date of Birth")]
        [Required(ErrorMessage = "Please select Dob")]
        public DateTime Dob { set; get; }

        [Range(1, 3, ErrorMessage = "Please select Gender")]
        public int Gender { set; get; }
        public string GenderName { set; get; }
        public int Age { set; get; }
        public string Name { set; get; }
        public string CityName { set; get; }
        public string CountryName { set; get; }
        public int CountryId { set; get; }
        public int Total { set; get; }
    }
}
