using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace UserApp.Web.Models
{
    public class RegisterModel
    {

        [Required(ErrorMessage = "Please enter First name."), StringLength(20)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Please enter Last name."), StringLength(20)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Please enter your email address."), StringLength(20)]
        [DataType(DataType.EmailAddress)]
        [RegularExpression("^[a-zA-Z0-9_\\+-]+(\\.[a-z0-9_\\+-]+)*@[a-z0-9-]+(\\.[a-z0-9]+)*\\.([a-z]{2,4})$", ErrorMessage = "Invalid email format.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Please enter password.")]
        [DataType(DataType.Password)]
          [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,20}$",
            ErrorMessage = "Password must meet requirements")]
        [MinLength(8, ErrorMessage = "Password length should be more than 8 character")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Please confirm password."), StringLength(20)]
        [Compare("Password", ErrorMessage = "Password is not matching")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        public string ConfirmPassword { get; set; }

        [DataType(DataType.Date)]
        [DateOfBirthBeforeToday(ErrorMessage = "Date of birth must be before today's date.")]
        public DateTime DateOfBirth { get; set; }

        [Required(ErrorMessage = "Please enter address.")]
        [MinLength(6, ErrorMessage = "Adress should be more than 6 character")]
        [MaxLength(50, ErrorMessage = "maximum length of 50 chaaracters allowded")]
        public string Address { get; set; }

        public IFormFile? Image { get; set; }
    }










    public class DateOfBirthBeforeTodayAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            DateTime dateOfBirth = Convert.ToDateTime(value);

            if (dateOfBirth >= DateTime.Today)
            {
                return new ValidationResult("Date of birth must be before today's date.");
            }

            return ValidationResult.Success;
        }
    }
}
