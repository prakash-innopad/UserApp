using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;

using System.Threading.Tasks;
using UserApp.Web.Models;
using UserApp.Web.Service;
using UserApp.Web.Utility;

namespace UserApp.Web.Controllers
{
   
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public class HomeController : Controller
    {
        private readonly IBaseService _service;
        private readonly ITokenProvider _tokenProvider;
        public HomeController(IBaseService service, ITokenProvider tokenProvider)
        {
            _service = service;
            _tokenProvider = tokenProvider;
        }

        public IActionResult Index()
        {
            return View();
        }
        
       
        

      

        public IActionResult Privacy()
        {
            return View();
        }

        

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
