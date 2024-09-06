using Web.Models;

namespace Web.Services
{
    public class UserService : IUserService
    {
        private readonly HttpClient _httpClient;

        public UserService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<UserViewModel> GetUser()
        {
            var user = await _httpClient.GetFromJsonAsync<UserViewModel>("/api/user/getuser");

            return user!;
        }
    }
}
