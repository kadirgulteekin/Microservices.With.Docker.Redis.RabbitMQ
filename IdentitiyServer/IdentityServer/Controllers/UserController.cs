using IdentityServer.Dtos;
using IdentityServer.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Shared.Dtos;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;


        public class Response<T>
        {
            public bool Success { get; set; }
            public List<string> Errors { get; set; }
            public int StatusCode { get; set; }

            public static Response<T> Fail(List<string> errors, int statusCode)
            {
                return new Response<T> { Success = false, Errors = errors, StatusCode = statusCode };
            }
        }

        [HttpPost]
        public async Task<IActionResult> SignUp(SignupDto signupDto)
        {
            var user = new ApplicationUser();
            user.UserName = signupDto.UserName;
            user.Email = signupDto.Email;
            user.City = signupDto.City;

            var result = await _userManager.CreateAsync(user, signupDto.Password);

            if (!result.Succeeded)
            {
                var response =  Response<NoContent>.Fail(result.Errors.Select(x=>x.Description).ToList(),400);
                return BadRequest(response);
            }
            return NoContent();
        }

    }
}
