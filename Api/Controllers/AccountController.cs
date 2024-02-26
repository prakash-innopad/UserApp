using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Security.Claims;
using System.Threading.Tasks;
using UserApp.Api.Models;
using UserApp.Api.Repository;

namespace UserApp.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountRepository _accountRepository;
        private readonly ApiResponse _response;

        public AccountController(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
            _response = new ApiResponse();
        }

        [HttpPost("signup")]
        public async Task<IActionResult> SignUp([FromForm] SignUpModel signUpModel)
        {
            try
            {
                var response = await _accountRepository.SignUpAsync(signUpModel);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500);
            }

        }

        [HttpGet("login")]
        public async Task<IActionResult> Login()
        {
            try
            {
                var credetnetials = this.Request.Headers["Credetentials"].ToString();
                string[] cred = credetnetials.Split(":");
                var loginModel = new LoginModel() { Email = cred[0], Password = cred[1] };
                var response = await _accountRepository.LoginAsync(loginModel);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500);
            }


        }

        [Authorize(Roles = "SUPER_ADMIN")]
        [HttpGet("addrole/{role}")]
        public async Task<IActionResult> AddRole(string role)
        {
            var result = await _accountRepository.AddNewRole(role);
            return Ok(result);
        }

        [Authorize]
        [HttpGet("user/{id}")]
        public async Task<IActionResult> GetUser(string id)
        {
            try
            {
                if (id == null)
                {
                    _response.Message = "Invalid user";
                    _response.IsSuccess = false;
                    _response.Result = "";
                    return BadRequest(_response);
                }
                else
                {
                    var response = await _accountRepository.GetUser(id);
                    return Ok(response);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500);
            }
        }

        [Authorize(Roles = "SUPER_ADMIN")]
        [HttpGet("users")]
        public IActionResult GetUsers()
        {
            try
            {
                var result = _accountRepository.GetAllUsers();
                if (result != null)
                {
                    _response.Message = "All users list";
                    _response.IsSuccess = true;
                    _response.Result = result;
                    return Ok(_response);
                }
                _response.Message = "No users found";
                _response.IsSuccess = false;
                _response.Result = "";
                return Ok(_response);
            }
            catch (Exception ex)
            {
                return StatusCode(500);
            }
        }
        [Authorize(Roles = "SUPER_ADMIN")]
        [HttpGet("userset/{index:int}/{size:int}")]
        public IActionResult GetUsersSet(int index,int size)
        {
            try
            {
                var result = _accountRepository.GetUsersSet(index,size);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500);
            }
        }

        [Authorize(Roles = "SUPER_ADMIN")]
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            try
            {
                var response = await _accountRepository.DeleteUser(id);
                return Ok(response);
            }
            catch(Exception ex)
            {
                return StatusCode(500);
            }
        }

       
        [HttpPost("edit")]
        public async Task<IActionResult> EditUser([FromForm] UsersModel userModel)
        {
            try
            {
                var response = await _accountRepository.EditUser(userModel);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500);
            }

        }

        [HttpGet("country")]
        public IActionResult GetCountries()
        {
            try
            {
                var result = _accountRepository.GetAllCountries();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500);
            }
        }

        [HttpGet("state/{id}")]
        public IActionResult GetStates(int id)
        {
            try
            {
                var result = _accountRepository.GetStatesByCountryId(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500);
            }
        }

        [HttpGet("city/{id}")]
        public IActionResult GetCities(int id)
        {
            try
            {
                var result = _accountRepository.GetCitiesByStateId(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500);
            }
        }
    }
}
