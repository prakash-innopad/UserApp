using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using UserApp.Web.Models;
using UserApp.Web.Service;
using UserApp.Web.Utility;

namespace UserApp.Web.Controllers
{
    public class UserController : Controller
    {
        private readonly IBaseService _service;
        private readonly ITokenProvider _tokenProvider;
        public UserController(IBaseService service, ITokenProvider tokenProvider)
        {
            _service = service;
            _tokenProvider = tokenProvider;
        }
        // [HttpGet("Profile/{id}")]
        [HttpGet]
        public async Task<IActionResult> Profile(string? id)
        {
            string userId = id == null ? User.FindFirstValue(ClaimTypes.NameIdentifier) : id;
            var response = await _service.GetProfile(userId);
            if (response.IsSuccess)
            {
                if (response.Result is Profile profile)
                {
                    return View(profile);
                }
            }
            if (response.Status == 401)
            {
                return RedirectToAction("Login", "Auth");
            }
            TempData["error"] = response.Message;
            return NotFound();
        }

        

        public async Task<IActionResult> Users(int pageNumber)
        {
            var response = await _service.GetUsersSet(pageNumber, 3);
            // var response = await _service.GetAllUsers();
            int pagesize = 3;
            if (response.IsSuccess)
            {
                if (response.Result is PaginatedListData paginatedData)
                {
                    var paginatedList = PaginatedList<Profile>.Create(paginatedData.items, pageNumber, pagesize, paginatedData.DbCount);
                    return View(paginatedList);
                }
            }
            if (response.Status == 401)
            {
                return RedirectToAction("Login", "Auth");
            }
            else
            {
                TempData["error"] = response.Message;
                return NotFound();
            }

        }

        public async Task<IActionResult> Delete(string userId)
        {
            var response = await _service.DeleteUser(userId);
            if (response.IsSuccess)
            {
                TempData["success"] = response.Message;
                return RedirectToAction("Users");
            }
            if (response.Status == 401)
            {
                return RedirectToAction("Login", "Auth");
            }
            else
            {
                TempData["error"] = response.Message;
                //ModelState.AddModelError("Result", response.Message);
                return RedirectToAction("Users");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string? id)
        {
            string userId = id == null ? User.FindFirstValue(ClaimTypes.NameIdentifier) : id;
            var response = await _service.GetProfile(userId);
            if (response.IsSuccess == true && response.Result is Profile profile)
            {
                return View(profile);
            }
            if (response.Status == 401)
            {
                return RedirectToAction("Login", "Auth");
            }
            else
            {
                TempData["error"] = "user not found";
                return NotFound();
            }
        }
        [HttpPost]
        public async Task<IActionResult> Edit(Profile profile)
        {
            var response = await _service.EditUser(profile);
            if (response.IsSuccess)
            {
                TempData["success"] = response.Message;
                return RedirectToAction("Users");
            }
            if (response.Status == 401)
            {
                return RedirectToAction("Login", "Auth");
            }
            else
            {
                TempData["error"] = response.Message;
                //ModelState.AddModelError("Result", response.Message);
                return View();
            }
        }
    }
}
