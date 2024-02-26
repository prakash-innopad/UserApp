using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserApp.Web.Models;

namespace UserApp.Web.Service
{
    public interface IBaseService
    {
        public Task<ApiResponse> RegisterAsync (RegisterModel registerVM);
        public Task<ApiResponse> LoginAsync(LoginModel loginVM);
        public  Task<ApiResponse> GetProfile(string userId);
        public  Task<ApiResponse> GetAllUsers();
        public Task<ApiResponse> DeleteUser(string id);
        public Task<ApiResponse> EditUser(Profile profile);
        public Task<ApiResponse> GetUsersSet(int index, int size);
    }
}
