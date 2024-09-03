using Microsoft.AspNetCore.Mvc;
using Web.Models;
using Web.Services.Interfaces;

namespace Web.Controllers
{
    public class AuthController : Controller
    {
        private readonly IIdentityService _ıdentityService;

        public AuthController(IIdentityService ıdentityService)
        {
            _ıdentityService = ıdentityService;
        }

        public IActionResult SignIn()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SignIn(SignInInput signInInput)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var response =await _ıdentityService.SignIn(signInInput);

            if(!response.IsSuccessful)
            {
                response.Errors.ForEach(x =>
                {
                    ModelState.AddModelError(String.Empty, x);
                });
 
                return View();               
            }
            return RedirectToAction(nameof(Index), "Home");
        }
    }
}
