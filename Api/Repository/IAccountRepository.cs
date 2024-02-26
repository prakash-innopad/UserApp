using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserApp.Api.Models;

namespace UserApp.Api.Repository
{
   public interface IAccountRepository
    {
        public Task<ApiResponse> SignUpAsync(SignUpModel signUpModel);
        public Task<ApiResponse> LoginAsync(LoginModel loginModel);
        public Task<string> AddNewRole(string role);
        public IEnumerable<UsersModel> GetAllUsers();
        public Task<ApiResponse> GetUser(string id);
        public Task<ApiResponse> DeleteUser(string userId);
        public  Task<ApiResponse> EditUser(UsersModel user);
        public ApiResponse GetUsersSet(int pageIndex, int pageSize);
        public ApiResponse GetAllCountries();
        public ApiResponse GetStatesByCountryId(int id);
        public ApiResponse GetCitiesByStateId(int id);
    }
}
