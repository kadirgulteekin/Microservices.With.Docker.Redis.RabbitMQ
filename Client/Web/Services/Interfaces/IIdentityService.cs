using IdentityModel.Client;
using Shared.Dtos;
using Web.Models;

namespace Web.Services.Interfaces
{
    public interface IIdentityService
    {
        Task<ResponseDto<bool>> SignIn(SignInInput signInInput);

        Task<TokenResponse> GetAccessTokenByRefreshToken();
        Task RevokeRefreshToken();

    }
}
