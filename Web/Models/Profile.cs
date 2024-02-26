using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using UserApp.Web.Utility;

namespace UserApp.Web.Models
{
    public class Profile
    {
        public string  Id { get; set; }
        [Required(ErrorMessage = "Please enter First name."), StringLength(20)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Please enter Last name."), StringLength(20)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Please enter your email address."), StringLength(20)]
       
        [RegularExpression("^[a-z0-9_\\+-]+(\\.[a-z0-9_\\+-]+)*@[a-z0-9-]+(\\.[a-z0-9]+)*\\.([a-z]{2,4})$", ErrorMessage = "Invalid email format.")]
        public string Email { get; set; }

        [ValidateDateOfBirth]
        [DataType(DataType.Date)]
        [Display(Name = "Date of Birth")]
        public DateTime DateOfBirth { get; set; }

        public string Address { get; set; }
        [MaxFileSize(1)]
        [AllowedExtensions(new string []{ ".jpg", ".png"})]
        public IFormFile Image { get; set; }
        public string ImageLocalPath { get; set; }
        public string ImageBase64 { get; set; }
    }
}
