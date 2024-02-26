using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using UserApp.Web.Models;
using UserApp.Web.Utility;

namespace UserApp.Web.Service
{

    public class BaseService : IBaseService
    {
        //private readonly HttpClient _client;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly ITokenProvider _tokenProvider;
        private readonly HttpClient _client ;

        public BaseService(IHttpClientFactory httpClientFactory, IConfiguration configuration, ITokenProvider tokenProvider)
        {
            _configuration = configuration;
            //   _client = new HttpClient();
            //   _client.BaseAddress = new Uri(_configuration["ServiceURL:URL"]);
            _httpClientFactory = httpClientFactory;
            _client = _httpClientFactory.CreateClient();
            _client.BaseAddress = new Uri(_configuration["ServiceURL:URL"]);
            _tokenProvider = tokenProvider;
        }

        public async Task<ApiResponse> RegisterAsync(RegisterModel registerVM)
        {
            HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Post, _client.BaseAddress + "/account/signup");

            if (registerVM.Image != null)
            {
                message.Headers.Add("Accept", "*/*");
            }
            else
            {
                message.Headers.Add("Accept", "application/json");
            }
            var content = new MultipartFormDataContent();
           
            foreach (var prop in registerVM.GetType().GetProperties())
            {

                var value = prop.GetValue(registerVM);
                if (registerVM.Image != null)
                {
                    if (value is FormFile)
                    {
                        var file = (FormFile)value;
                        if (file != null)
                        {
                            content.Add(new StreamContent(file.OpenReadStream()), prop.Name, file.FileName);
                        }
                    }
                    else
                    {
                        content.Add(new StringContent(value == null ? "" : value.ToString()), prop.Name);
                    }
                }
                else
                {
                    content.Add(new StringContent(value == null ? "" : value.ToString()), prop.Name);
                }
            }
            message.Content = content;
            
            HttpResponseMessage responseMessage = await _client.SendAsync(message);
            if (responseMessage.IsSuccessStatusCode)
            {
                var apiContent = await responseMessage.Content.ReadAsStringAsync();
                var apiResponseObject = JsonConvert.DeserializeObject<ApiResponse>(apiContent);
                return apiResponseObject;
            }
            else
            {
                return new ApiResponse() { IsSuccess = false, Message = responseMessage.ReasonPhrase, Status = (int)responseMessage.StatusCode };
            }
        }

        public async Task<ApiResponse> LoginAsync(LoginModel loginVM)
        {
            string data = JsonConvert.SerializeObject(loginVM);
            // StringContent content = new StringContent(data, Encoding.UTF8, "application/json");
            //  HttpResponseMessage apiResponse = await _client.PostAsync(_client.BaseAddress + "/account/login", content);
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, _client.BaseAddress + "/account/login");
            request.Headers.Add("Credetentials", $"{loginVM.Email}:{loginVM.Password}");
            HttpResponseMessage responseMessage = await _client.SendAsync(request);
            if (responseMessage.IsSuccessStatusCode)
            {
                var apiContent = await responseMessage.Content.ReadAsStringAsync();
                var apiResponseObject = JsonConvert.DeserializeObject<ApiResponse>(apiContent);
                return apiResponseObject;
            }
            else
            {
                return new ApiResponse() { IsSuccess = false, Message = responseMessage.ReasonPhrase };
            }
        }

        public async Task<ApiResponse> GetProfile(string userId)
        {
            var token = _tokenProvider.GetToken();
            // HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, _client.BaseAddress + "/account/user");
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, $"{_client.BaseAddress}/account/user/{userId}");
            request.Headers.Add("Authorization", $"Bearer {token}");
            using (HttpResponseMessage responseMessage = await _client.SendAsync(request))
            {

                if (responseMessage.IsSuccessStatusCode)
                {
                    var apiContent = await responseMessage.Content.ReadAsStringAsync();
                    var apiResponseObject = JsonConvert.DeserializeObject<ApiResponse>(apiContent);
                    if (apiResponseObject.Result != null)
                    {
                        var user = JsonConvert.DeserializeObject<Profile>(apiResponseObject.Result.ToString());
                        apiResponseObject.Result = user;
                    }
                    return apiResponseObject;
                }
                else
                {
                    return new ApiResponse() { IsSuccess = false, Message = responseMessage.ReasonPhrase };
                }
            }
        }

        public async Task<ApiResponse> GetAllUsers()
        {
            var token = _tokenProvider.GetToken();
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, _client.BaseAddress + "/account/users");
            request.Headers.Add("Authorization", $"Bearer {token}");
            HttpResponseMessage apiResponse = await _client.SendAsync(request);

            if (apiResponse.IsSuccessStatusCode)
            {
                var apiContent = await apiResponse.Content.ReadAsStringAsync();
                var apiResponseObject = JsonConvert.DeserializeObject<ApiResponse>(apiContent);
                if (apiResponseObject.Result != null)
                {
                    var user = JsonConvert.DeserializeObject<List<Profile>>(apiResponseObject.Result.ToString());
                    apiResponseObject.Result = user;
                }
                return apiResponseObject;
            }
            else
            {
                return new ApiResponse() { IsSuccess = false, Message = apiResponse.ReasonPhrase, Status= (int)apiResponse.StatusCode};
            }
        }
        public async Task<ApiResponse> GetUsersSet(int index,int size)
        {
            var token = _tokenProvider.GetToken();
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, $"{_client.BaseAddress}/account/userset/{index}/{size}");
            request.Headers.Add("Authorization", $"Bearer {token}");
            HttpResponseMessage responseMessage = await _client.SendAsync(request);

            if (responseMessage.IsSuccessStatusCode)
            {
                var apiContent = await responseMessage.Content.ReadAsStringAsync();
                var apiResponseObject = JsonConvert.DeserializeObject<ApiResponse>(apiContent);
                if (apiResponseObject.Result != null) ////
                {
                    var user = JsonConvert.DeserializeObject<PaginatedListData>(apiResponseObject.Result.ToString());
                    apiResponseObject.Result = user;
                }
                return apiResponseObject;
            }
            else
            {
                return new ApiResponse() { IsSuccess = false, Message = responseMessage.ReasonPhrase , Status = (int)responseMessage.StatusCode };
            }
        }

        public async Task<ApiResponse> DeleteUser(string id)
        {
            var token = _tokenProvider.GetToken();
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Delete, $"{_client.BaseAddress}/account/delete/{id}");
            request.Headers.Add("Authorization", $"Bearer {token}");            
            HttpResponseMessage responseMessage = await _client.SendAsync(request);
           
            if (responseMessage.IsSuccessStatusCode)
            {
                var apiContent = await responseMessage.Content.ReadAsStringAsync();
                var apiResponseObject = JsonConvert.DeserializeObject<ApiResponse>(apiContent);
                return apiResponseObject;
            }
            else
            {
                return new ApiResponse() { IsSuccess = false, Message = responseMessage.ReasonPhrase, Status = (int)responseMessage.StatusCode };
            }
        }

        public async Task<ApiResponse> EditUser(Profile profile)
        {
            var token = _tokenProvider.GetToken();
           // string data = JsonConvert.SerializeObject(profile);          
            HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Post, _client.BaseAddress + "/account/edit");
            message.Headers.Add("Authorization", $"Bearer {token}");

            if (profile.Image != null)
            {
                message.Headers.Add("Accept", "*/*");
            }
            else
            {
                message.Headers.Add("Accept", "application/json");
            }
            var content = new MultipartFormDataContent();
            //if (profile.Image != null)
            //{
                // var content = new MultipartFormDataContent();
                foreach (var prop in profile.GetType().GetProperties())
                {

                    var value = prop.GetValue(profile);
                    if (profile.Image != null)
                    {
                        if (value is FormFile)
                        {
                            var file = (FormFile)value;
                            if (file != null)
                            {
                                content.Add(new StreamContent(file.OpenReadStream()), prop.Name, file.FileName);
                            }
                        }
                        else
                        {
                           content.Add(new StringContent(value == null ? "" : value.ToString()), prop.Name);
                        }
                    }
                    else
                    {
                        content.Add(new StringContent(value == null ? "" : value.ToString()), prop.Name);
                    }
                }
                message.Content = content;
            //}
           // else {

              //  message.Content = new StringContent(JsonConvert.SerializeObject(profile), Encoding.UTF8, "application/json");
            //}
            HttpResponseMessage responseMessage = await _client.SendAsync(message);
            if (responseMessage.IsSuccessStatusCode)
            {
                var apiContent = await responseMessage.Content.ReadAsStringAsync();
                var apiResponseObject = JsonConvert.DeserializeObject<ApiResponse>(apiContent);
                return apiResponseObject;
            }
            else
            {
                return new ApiResponse() { IsSuccess = false, Message = responseMessage.ReasonPhrase, Status = (int)responseMessage.StatusCode };
            }
        }
    }  
}
