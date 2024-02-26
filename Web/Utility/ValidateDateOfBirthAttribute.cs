using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace UserApp.Web.Utility
{
    public class ValidateDateOfBirthAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            DateTime dateOfBirth = (DateTime)value;
            if (dateOfBirth >= DateTime.Today)
            {
                return new ValidationResult("Date of Birth must be before today");
            }
            return ValidationResult.Success;
        }
    }
}
