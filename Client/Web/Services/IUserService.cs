using Web.Models;

namespace Web.Services
{
    public interface IUserService
    {
        Task<UserViewModel> GetUser();
    }
}
