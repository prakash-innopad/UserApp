using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace UserApp.Web.Models
{
    public class LoginModel
    {
        [Required(ErrorMessage = "Please enter user name.")]

        public string Email { get; set; }


        [Required(ErrorMessage = "Please enter password.")]
        [DataType(DataType.Password)]
        [MinLength(6,ErrorMessage = "Password length should be more than 6 character")]
        public string Password { get; set; }

        [Display(Name = "Remember login")]
        public bool RememberLogin { get; set; }
    }
}
