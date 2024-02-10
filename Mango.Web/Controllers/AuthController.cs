using Mango.Web.Models;
using Mango.Web.Services.Interface;
using Mango.Web.Utitlity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Mango.Web.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;
        private readonly ITokenProvider _tokenProvider;

        public AuthController(IAuthService authService, ITokenProvider tokenProvider)
        {
            this._authService = authService;
            this._tokenProvider = tokenProvider;
        }

        [HttpGet]
        public IActionResult Login()
        {
            LoginRequestDto requestDto = new();
            return View(requestDto);
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginRequestDto requestDto)
        {
            var result = await _authService.LoginAsync(requestDto);

            if (result != null && result.IsSuccess)
            {
                TempData["success"] = "Login Successfull";

                LoginResponseDto responseDto = JsonConvert.DeserializeObject<LoginResponseDto>(result.Result.ToString());

                await SignInUser(responseDto);
                _tokenProvider.SetToken(responseDto.Token);

                return RedirectToAction("Index", "Home");
            }
            else
            {
                TempData["error"] = result.Message;
                return View(requestDto);
            }
        }

        [HttpGet]
        public IActionResult Register()
        {
            var rolesList = new List<SelectListItem>
            {
                new SelectListItem{Text = StaticDetails.RoleAdmin, Value = StaticDetails.RoleAdmin},
                new SelectListItem{Text = StaticDetails.RoleCustomer, Value = StaticDetails.RoleCustomer},
            };

            ViewBag.RolesList = rolesList;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegistrationRequestDto requestDto)
        {
            var result = await _authService.RegisterAsync(requestDto);

            if (result != null && result.IsSuccess)
            {
                if (string.IsNullOrEmpty(requestDto.RoleName))
                {
                    requestDto.RoleName = StaticDetails.RoleCustomer;
                }

                var response = await _authService.AssignRoleAsync(requestDto);

                if (response != null && response.IsSuccess)
                {
                    TempData["success"] = "Registration Successfull";
                    return RedirectToAction(nameof(Login));
                }
            }
            else
            {
                TempData["error"] = result.Message;
            }

            var rolesList = new List<SelectListItem>
            {
                new SelectListItem{Text = StaticDetails.RoleAdmin, Value = StaticDetails.RoleAdmin},
                new SelectListItem{Text = StaticDetails.RoleCustomer, Value = StaticDetails.RoleCustomer},
            };

            ViewBag.RolesList = rolesList;

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            _tokenProvider.ClearToken();
            return RedirectToAction("Index", "Home");
        }

        public async Task SignInUser(LoginResponseDto responseDto)
        {
            var handler = new JwtSecurityTokenHandler();

            var jwtToken = handler.ReadJwtToken(responseDto.Token);

            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);

            identity.AddClaim(new Claim(JwtRegisteredClaimNames.Email, jwtToken.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Email).Value));
            identity.AddClaim(new Claim(JwtRegisteredClaimNames.Sub, jwtToken.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Sub).Value));
            identity.AddClaim(new Claim(JwtRegisteredClaimNames.Name, jwtToken.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Name).Value));

            identity.AddClaim(new Claim(ClaimTypes.Name, jwtToken.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Email).Value));
            identity.AddClaim(new Claim(ClaimTypes.Role, jwtToken.Claims.FirstOrDefault(u => u.Type == "role").Value));

            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(principal);
        }

    }
}
