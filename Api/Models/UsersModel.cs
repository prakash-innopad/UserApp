using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserApp.Api.Models
{
    public class UsersModel
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
       
        public DateTime DateOfBirth { get; set; }
        public string Address { get; set; }
       
        public string? ImageLocalPath { get; set; }
        public IFormFile? Image { get; set; }

        public byte []? ImageBase64 { get; set; }
    }
}
