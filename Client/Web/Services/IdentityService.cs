using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Shared.Dtos;
using System.Globalization;
using System.Security.Claims;
using System.Text.Json;
using Web.Models;
using Web.Services.Interfaces;

namespace Web.Services
{
    public class IdentityService : IIdentityService
    {
        private readonly HttpClient _httpClient;
        private readonly HttpContextAccessor _httpContextAccessor; //for cookie
        private readonly ClientSettings _clientSettings;
        private readonly ServiceApiSettings _serviceApiSettings;

        public IdentityService(HttpClient httpClient, HttpContextAccessor httpContextAccessor,IOptions<ClientSettings> clientSettings,IOptions<ServiceApiSettings> serviceApiSettings)
        {
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
            _clientSettings = clientSettings.Value;
            _serviceApiSettings = serviceApiSettings.Value;
        }

        public Task<TokenResponse> GetAccessTokenByRefreshToken()
        {
            throw new NotImplementedException();
        }

        public Task RevokeRefreshToken()
        {
            throw new NotImplementedException();
        }

        public async Task<ResponseDto<bool>> SignIn(SignInInput signInInput)
        {
            var disco = await _httpClient.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest
            {
                Address = _serviceApiSettings.BaseUri,
                Policy = new DiscoveryPolicy { RequireHttps = false}
            });

            if (disco.IsError)
            {
                throw disco.Exception ?? throw new Exception("Exception alınamadı");
            }

            var passwordTokenRequest = new PasswordTokenRequest
            {
                ClientId = _clientSettings?.WebClientForUser?.ClientId ?? "defaultClientId",
                ClientSecret = _clientSettings?.WebClientForUser?.ClientSecret,
                UserName = signInInput.Email ?? throw new Exception("Email is cannot be found"),
                Password = signInInput.Password,
                Address = disco.TokenEndpoint
            };

            var token = await _httpClient.RequestPasswordTokenAsync(passwordTokenRequest);

            if (token.IsError)
            {
                var responseContent = await token.HttpResponse!.Content.ReadAsStringAsync();

                var errorDto = JsonSerializer.Deserialize<ErrorDto>(responseContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return ResponseDto<bool>.Failed(errorDto!.Error, 400);
            }

            var userInfoRequest = new UserInfoRequest();
            userInfoRequest.Token = token.AccessToken;
            userInfoRequest.Address = disco.UserInfoEndpoint;

            var userInfo = await _httpClient.GetUserInfoAsync(userInfoRequest);
            if (userInfo.IsError)
            {
                throw userInfo.Exception ?? throw new Exception("userInfo.Exception");
            }

            ClaimsIdentity claimsIdentity = new ClaimsIdentity(userInfo.Claims,CookieAuthenticationDefaults.AuthenticationScheme,"name","role");

            ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            var authenticationProperties = new AuthenticationProperties();

            authenticationProperties.StoreTokens(new List<AuthenticationToken>()
            {
                new AuthenticationToken
                {
                    Name =  OpenIdConnectParameterNames.AccessToken,Value = token.AccessToken!
                },
                new AuthenticationToken
                {
                    Name =  OpenIdConnectParameterNames.AccessToken,Value = token.RefreshToken!
                },
                new AuthenticationToken
                {
                    Name =  OpenIdConnectParameterNames.ExpiresIn,Value = DateTime.Now.AddSeconds(token.ExpiresIn).ToString("O",CultureInfo.InvariantCulture)
                }
            });

            authenticationProperties.IsPersistent = signInInput.IsRemember;

            await _httpContextAccessor!.HttpContext!.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal, authenticationProperties);

            return ResponseDto<bool>.Success(200);

        }
    }
}
