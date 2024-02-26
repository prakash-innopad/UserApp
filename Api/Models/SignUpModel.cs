using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace UserApp.Api.Models
{
    public class SignUpModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Address { get; set; }
        public string? ImageLocalPath { get; set; }
        public IFormFile? Image { get; set; }

        public byte[]? ImageBase64 { get; set; }
        public string? Role { get; set; }
    }
}
