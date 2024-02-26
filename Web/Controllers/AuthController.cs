using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using UserApp.Web.Models;
using UserApp.Web.Service;

namespace UserApp.Web.Controllers
{
    public class AuthController : Controller
    {
        private readonly IBaseService _service;
        private readonly ITokenProvider _tokenProvider;
        public AuthController(IBaseService service, ITokenProvider tokenProvider)
        {
            _service = service;
            _tokenProvider = tokenProvider;
        }
        [HttpGet]
        public IActionResult Register()
        {
            return View(new RegisterModel() { DateOfBirth = DateTime.Now });
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterModel registerModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var response = await _service.RegisterAsync(registerModel);
                    if (response.IsSuccess)
                    {
                        TempData["success"] = "You have successfully Registerd.";
                        return RedirectToAction("Login");
                    }
                    else
                    {
                        TempData["error"] = response.Message;
                        return View(registerModel);
                    }
                }
                else
                {
                    return View(registerModel);
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error","Home");
            }

        }



        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View(new LoginModel());
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginModel loginModel, string returnUrl)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var response = await _service.LoginAsync(loginModel);
                    if (response.IsSuccess)
                    {
                        _tokenProvider.SetToken((string)response.Result, loginModel.RememberLogin);
                        var id = await SignInUser((string)response.Result, loginModel.RememberLogin);
                        TempData["success"] = response.Message;
                        if (!string.IsNullOrEmpty(returnUrl))
                        {
                            return LocalRedirect(returnUrl);
                        }
                        return RedirectToAction("Profile", "User");
                    }
                    else
                    {
                        TempData["error"] = response.Message;
                        //ModelState.AddModelError("Result", response.Message);
                        return View(loginModel);
                    }
                }
                else
                {
                    return View(loginModel);
                }
            }
            catch (Exception ex)
            {

                return RedirectToAction("Error","Home");
            }

        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            _tokenProvider.ClearToken();
            return RedirectToAction("Index", "Home");
        }

        [NonAction]
        private async Task<string> SignInUser(string token, bool rememberMe)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);
            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
            var list = jwt.Claims.ToList();
            identity.AddClaim(new Claim(ClaimTypes.Name, jwt.Claims.FirstOrDefault(u => u.Type == ClaimTypes.Name).Value));
            identity.AddClaim(new Claim(ClaimTypes.Email, jwt.Claims.FirstOrDefault(u => u.Type == ClaimTypes.Email).Value));
            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, jwt.Claims.FirstOrDefault(u => u.Type == "Id").Value));
            identity.AddClaim(new Claim(ClaimTypes.Role, jwt.Claims.FirstOrDefault(u => u.Type == ClaimTypes.Role).Value));

            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, new AuthenticationProperties() { IsPersistent = rememberMe });
            return jwt.Claims.FirstOrDefault(u => u.Type == "Id").Value;
        }
    }

}
