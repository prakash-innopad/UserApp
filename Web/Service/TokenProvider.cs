using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserApp.Web.Service
{
    public class TokenProvider : ITokenProvider
    {
        public readonly IHttpContextAccessor _httpContextAccessor;

        public TokenProvider(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public void ClearToken()
        {
            _httpContextAccessor.HttpContext.Response.Cookies.Delete("JwtToken");
        }

        public string GetToken()
        {
            string token = null;
            bool hasToken =  _httpContextAccessor.HttpContext.Request.Cookies.TryGetValue("JwtToken", out token);
            return hasToken == true ? token : null;
        }

        public void SetToken(string token,bool rememberMe)
        {
            
            var cookieOptions = new CookieOptions();
            if (rememberMe)
            {
                cookieOptions.Expires = DateTime.UtcNow.AddMonths(1);
            }
            cookieOptions.HttpOnly = true;
            cookieOptions.Secure = true;
            cookieOptions.SameSite = SameSiteMode.Strict;
          
            _httpContextAccessor.HttpContext.Response.Cookies.Append("JwtToken", token, cookieOptions);
        }
    }
}
