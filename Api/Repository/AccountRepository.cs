using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using UserApp.Api.Models;

namespace UserApp.Api.Repository
{
    public class AccountRepository : IAccountRepository
    {

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration; 
        private readonly UserAppContext _userAppContext;
        public AccountRepository(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration, UserAppContext userAppContext)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _configuration = configuration;           
            _userAppContext = userAppContext;
        }

        public async Task<ApiResponse> SignUpAsync(SignUpModel signUpModel)
        {           
            var user = new ApplicationUser();
            user.FirstName = signUpModel.FirstName;
            user.LastName = signUpModel.LastName;
            user.Email = signUpModel.Email.ToLower();
            user.UserName = signUpModel.Email.ToLower();
            user.DateOfBirth = signUpModel.DateOfBirth;
            user.Address = signUpModel.Address;

            string filePath = string.Empty;
            if (signUpModel.Image != null)
            {
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(signUpModel.Image.FileName);
                filePath = @"Utility\Images\" + fileName;
                user.ImageLocalPath = filePath;
            }
            
            var result = await _userManager.CreateAsync(user, signUpModel.Password);
            if (result.Succeeded)
            {
                if (signUpModel.Image != null)
                {
                    var filePathDirectory = Path.Combine(Directory.GetCurrentDirectory(), filePath);
                    using (var fileStream = new FileStream(filePathDirectory, FileMode.Create))
                    {
                        signUpModel.Image.CopyTo(fileStream);
                    }
                }
                var userD = await _userManager.FindByEmailAsync(signUpModel.Email.ToLower());
                await _userManager.AddToRoleAsync(userD, "USER");
                return new ApiResponse() { Status = 201, IsSuccess = true, Message = "User Registered successfully" };
            }
            else
            {
                return new ApiResponse() { Status = 400, IsSuccess = false, Message = result.Errors.FirstOrDefault().Description };
            }

        }

        public async Task<ApiResponse> LoginAsync(LoginModel loginModel)
        {
            var user = await _userManager.FindByEmailAsync(loginModel.Email.ToLower());
            if (user == null)
            {
                return new ApiResponse() { Status = 400, IsSuccess = false, Message = "Invalid Email" };
            }

            var result = await _signInManager.PasswordSignInAsync(loginModel.Email, loginModel.Password, false, false);

            if (result.Succeeded)
            {
                var roles = await _userManager.GetRolesAsync(user);
                var authClaims = new List<Claim>
            {
                new Claim("Id", user.Id),
                new Claim(ClaimTypes.Name, user.FirstName),
                new Claim(ClaimTypes.Email, loginModel.Email),
                new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
            };
                foreach (var role in roles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, role));
                }
                var authSignInkey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

                var token = new JwtSecurityToken(
                    issuer: _configuration["JWT:ValidIssuer"],
                    audience: _configuration["JWT:ValidAudience"],
                    expires: DateTime.UtcNow.AddMinutes(20),
                    claims: authClaims,
                    signingCredentials: new SigningCredentials(authSignInkey, SecurityAlgorithms.HmacSha256Signature)
                    );
                var Jwtresult = new JwtSecurityTokenHandler().WriteToken(token);

                return new ApiResponse() { Status = 200, IsSuccess = true, Message = "User Logged in", Result = Jwtresult };
            }
            else
            {
                return new ApiResponse() { Status = 400, IsSuccess = false, Message = "Invalid Password" };
            }

        }

        public async Task<ApiResponse> GetUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                var userModel = new UsersModel()
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    DateOfBirth = user.DateOfBirth,
                    Address = user.Address,
                    ImageLocalPath = user.ImageLocalPath
                };
                if (!string.IsNullOrEmpty(user.ImageLocalPath))
                {
                    userModel.ImageBase64 = System.IO.File.ReadAllBytes(user.ImageLocalPath);

                }
                return new ApiResponse() { Status = 200, IsSuccess = true, Message = "User Details", Result = userModel };
            }
            else
            {
                return new ApiResponse() { Status = 400, IsSuccess = false, Message = "User not found" };
            }
        }

        public IEnumerable<UsersModel> GetAllUsers()
        {
            List<ApplicationUser> users = _userManager.Users.ToList();
            List<UsersModel> usersList = new List<UsersModel>();
            foreach (var user in users)
            {
                byte[] imageBytes = null;

                if (!string.IsNullOrEmpty(user.ImageLocalPath))
                {
                    imageBytes = System.IO.File.ReadAllBytes(user.ImageLocalPath);
                }

                usersList.Add(new UsersModel()
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    ImageBase64 = imageBytes
                });
            }
            return usersList;
        }

        public ApiResponse GetUsersSet(int pageIndex, int pageSize)
        {
            List<ApplicationUser> users = _userManager.Users.Skip((pageIndex) * pageSize).Take(pageSize).ToList();
            var count = _userManager.Users.Count();
            List<UsersModel> usersList = new List<UsersModel>();
            if (users != null)
            {
                foreach (var user in users)
                {
                    byte[] imageBytes = null;

                    if (!string.IsNullOrEmpty(user.ImageLocalPath))
                    {
                        imageBytes = System.IO.File.ReadAllBytes(user.ImageLocalPath);
                    }

                    usersList.Add(new UsersModel()
                    {
                        Id = user.Id,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Email = user.Email,
                        ImageBase64 = imageBytes
                    });
                }
                return new ApiResponse() { Status = 200, IsSuccess = true, Message = "Users Details", Result = new { items = usersList, DbCount = count } };
            }
            else
            {
                return new ApiResponse() { Status = 400, IsSuccess = false, Message = "Users not found" };
            }
        }

        public async Task<ApiResponse> DeleteUser(string userId)
        {
            if (userId == null)
            {
                return new ApiResponse() { Status = 400, IsSuccess = false, Message = "Please provide Id" };
            }
            else
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user != null)
                {
                    var result = await _userManager.DeleteAsync(user);
                    if (result.Succeeded)
                    {
                        return new ApiResponse() { Status = 200, IsSuccess = true, Message = $"User {user.FirstName} is deleted" };
                    }
                    else
                    {
                        return new ApiResponse() { Status = 400, IsSuccess = false, Message = "Error while deleting user" };
                    }
                }
                else
                {
                    return new ApiResponse() { Status = 400, IsSuccess = false, Message = "User not found" };
                }

            }
        }

        public async Task<ApiResponse> EditUser(UsersModel user)
        {
            if (user.Image != null)
            {
                HandleImage(user);
            }
            var userEdit = await _userManager.FindByIdAsync(user.Id);
            if (userEdit != null)
            {
                userEdit.FirstName = user.FirstName;
                userEdit.LastName = user.LastName;
                userEdit.Email = user.Email;
                userEdit.UserName = user.Email;
                userEdit.DateOfBirth = user.DateOfBirth;
                userEdit.Address = user.Address;
                userEdit.ImageLocalPath = user.ImageLocalPath;
                var result = await _userManager.UpdateAsync(userEdit);
                if (result.Succeeded)
                {
                    return new ApiResponse() { Status = 200, IsSuccess = true, Message = "User details updated" };
                }
                else
                {
                    return new ApiResponse() { Status = 400, IsSuccess = false, Message = result.Errors.FirstOrDefault().Description };
                }
            }
            else
            {
                return new ApiResponse() { Status = 400, IsSuccess = false, Message = "User not found" };
            }
        }

        public void HandleImage(UsersModel usersModel)
        {

            if (!string.IsNullOrEmpty(usersModel.ImageLocalPath))
            {
                var oldFilePathDirectory = Path.Combine(Directory.GetCurrentDirectory(), usersModel.ImageLocalPath);
                FileInfo file = new FileInfo(oldFilePathDirectory);
                if (file.Exists)
                {
                    file.Delete();
                }
            }
            string fileName = usersModel.Id + Path.GetExtension(usersModel.Image.FileName);
            string filePath = @"Utility\Images\" + fileName;
            var filePathDirectory = Path.Combine(Directory.GetCurrentDirectory(), filePath);
            using (var fileStream = new FileStream(filePathDirectory, FileMode.Create))
            {
                usersModel.Image.CopyTo(fileStream);
            }
            
            usersModel.ImageLocalPath = filePath;
        }

        public async Task<string> AddNewRole(string role)
        {
            if (!await _roleManager.RoleExistsAsync(role))
            {
                var result = await _roleManager.CreateAsync(new IdentityRole(role.ToUpper()));

                return "Role Added";
            }
            else
            {
                return "Role already exist";
            }
        }

        public ApiResponse GetAllCountries()
        {
            var countries = _userAppContext.Countries.OrderBy(x => x.Name).ToList();
            return new ApiResponse() { Status = 200, IsSuccess = true, Message = "Country List", Result = countries };
        }
        public ApiResponse GetStatesByCountryId(int id)
        {
            var states = _userAppContext.States.Where(x => x.CountryId == id).ToList();
            return new ApiResponse() { Status = 200, IsSuccess = true, Message = "State List", Result = states };
        }
        public ApiResponse GetCitiesByStateId(int id)
        {
            var cities = _userAppContext.Cities.Where(x=>x.StateId==id).ToList();
            return new ApiResponse() { Status = 200, IsSuccess = true, Message = "City List", Result = cities };
        }
    }
}
