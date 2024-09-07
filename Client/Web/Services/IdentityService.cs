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
        private readonly IHttpContextAccessor _httpContextAccessor; //for cookie
        private readonly ClientSettings _clientSettings;
        private readonly ServiceApiSettings _serviceApiSettings;

        public IdentityService(HttpClient httpClient, IHttpContextAccessor httpContextAccessor, IOptions<ClientSettings> clientSettings, IOptions<ServiceApiSettings> serviceApiSettings)
        {
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
            _clientSettings = clientSettings.Value;
            _serviceApiSettings = serviceApiSettings.Value;
        }

        public async Task<TokenResponse> GetAccessTokenByRefreshToken()
        {
            var disco = await _httpClient.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest
            {
                Address = _serviceApiSettings.IdentityBaseUri,
                Policy = new DiscoveryPolicy { RequireHttps = false }
            });

            if (disco.IsError)
            {
                throw disco.Exception ?? throw new System.Exception("Exception alınamadı");
            }

            var refreshToken = await _httpContextAccessor.HttpContext!.GetTokenAsync(OpenIdConnectParameterNames.RefreshToken);

            RefreshTokenRequest refreshTokenRequest = new()
            {
                ClientId = _clientSettings?.WebClientForUser?.ClientId!,
                ClientSecret = _clientSettings?.WebClientForUser?.ClientSecret!,
                RefreshToken = refreshToken != null ? refreshToken : throw new System.Exception("refresh token couldnt get"),
                Address = disco.TokenEndpoint
            };

            var token = await _httpClient.RequestRefreshTokenAsync(refreshTokenRequest);

            if (token.IsError)
            {
                throw new InvalidOperationException("Token Error!");
            };



            var authenticationToken = new List<AuthenticationToken>()
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
                    Name =  OpenIdConnectParameterNames.ExpiresIn,Value = DateTime.Now.AddSeconds(token.ExpiresIn).ToString("o",CultureInfo.InvariantCulture)
                }
            };

            var authenticationResult = await _httpContextAccessor?.HttpContext?.AuthenticateAsync()!;

            var properties = authenticationResult.Properties;

            properties!.StoreTokens(authenticationToken);

            await _httpContextAccessor.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, authenticationResult?.Principal!, properties);


            return token;

        }

        public async Task RevokeRefreshToken()
        {
            var disco = await _httpClient.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest
            {
                Address = _serviceApiSettings.IdentityBaseUri,
                Policy = new DiscoveryPolicy { RequireHttps = false }
            });

            if (disco.IsError)
            {
                throw disco.Exception ?? throw new System.Exception("Exception alınamadı");
            }

            var refreshToken = await _httpContextAccessor!.HttpContext!.GetTokenAsync(OpenIdConnectParameterNames.RefreshToken);

            TokenRevocationRequest tokenRevocationRequest = new()
            {
                ClientId = _clientSettings.WebClientForUser?.ClientId!,
                ClientSecret = _clientSettings.WebClientForUser?.ClientSecret,
                Address = disco.RegistrationEndpoint,
                Token = refreshToken!,
                TokenTypeHint = "refreshToken"

            };

            await _httpClient.RevokeTokenAsync(tokenRevocationRequest);

        }

        public async Task<ResponseDto<bool>> SignIn(SignInInput signInInput)
        {
            var disco = await _httpClient.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest
            {
                Address = _serviceApiSettings.IdentityBaseUri,
                Policy = new DiscoveryPolicy { RequireHttps = false }
            });

            if (disco.IsError)
            {
                throw disco.Exception ?? throw new System.Exception("Exception alınamadı");
            }

            var passwordTokenRequest = new PasswordTokenRequest
            {
                ClientId = _clientSettings?.WebClientForUser?.ClientId ?? "defaultClientId",
                ClientSecret = _clientSettings?.WebClientForUser?.ClientSecret,
                UserName = signInInput.Email ?? throw new System.Exception("Email is cannot be found"),
                Password = signInInput.Password,
                Address = disco.TokenEndpoint
            };

            var token = await _httpClient.RequestPasswordTokenAsync(passwordTokenRequest);

            if (token.IsError)
            {
                var responseContent = await token.HttpResponse!.Content.ReadAsStringAsync();

                ErrorDto? errorDto;

                errorDto = JsonSerializer.Deserialize<ErrorDto>(responseContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return ResponseDto<bool>.Failed(errorDto!.Errors, 400);
            }

            var userInfoRequest = new UserInfoRequest();
            userInfoRequest.Token = token.AccessToken;
            userInfoRequest.Address = disco.UserInfoEndpoint;

            var userInfo = await _httpClient.GetUserInfoAsync(userInfoRequest);
            if (userInfo.IsError)
            {
                throw userInfo.Exception != null ? userInfo.Exception : new InvalidOperationException("Failed to retrieve user information.");
            }

            ClaimsIdentity claimsIdentity = new ClaimsIdentity(userInfo.Claims, CookieAuthenticationDefaults.AuthenticationScheme, "name", "role");

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
