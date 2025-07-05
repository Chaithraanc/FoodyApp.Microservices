using Foody.Web.Models;
using Foody.Web.Service.IService;
using Foody.Web.Utility;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Foody.Web.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;
        private readonly ITokenProvider _tokenProvider;

        public AuthController(IAuthService authService, ITokenProvider tokenProvider)
        {
            _authService = authService;
            _tokenProvider = tokenProvider;
        }

        [HttpGet]
        public IActionResult Login()
        {
            LoginRequestDto loginRequestDto = new LoginRequestDto();
            return View(loginRequestDto);
        }

        [HttpGet]
        public IActionResult Register()
        {
            var roleList = new List<SelectListItem>()
            {
                new SelectListItem()
                {
                    Text = SD.RoleAdmin,
                    Value = SD.RoleAdmin
                },
                new SelectListItem()
                {
                    Text = SD.RoleCustomer,
                    Value = SD.RoleCustomer
                }
            };
            ViewBag.RoleList = roleList;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegistrationRequestDto obj)
        {
            ResponseDto result = await _authService.RegisterAsync(obj);
            ResponseDto assignRole;

            if (result != null && result.IsSuccess)
            {
                if (string.IsNullOrEmpty(obj.Role))
                {
                    obj.Role = SD.RoleCustomer;
                }
                assignRole = await _authService.AssignRoleAsync(obj);

                if (assignRole != null && assignRole.IsSuccess)
                {
                    TempData["success"] = "Registration successful";
                    return RedirectToAction(nameof(Login));
                }
                else
                {
                    ModelState.AddModelError(string.Empty, assignRole?.Message ?? "Role assignment failed.");
                }
            }
            else
            {
                TempData["error"] = result?.Message ?? "Registration failed.";
                //ModelState.AddModelError(string.Empty, result?.Message ?? "Registration failed.");
            }

            var roleList = new List<SelectListItem>()
                {
                    new SelectListItem()
                    {
                        Text = SD.RoleAdmin,
                        Value = SD.RoleAdmin
                    },
                    new SelectListItem()
                    {
                        Text = SD.RoleCustomer,
                        Value = SD.RoleCustomer
                    }
                };
            ViewBag.RoleList = roleList;
            return View();

        }


        [HttpPost]
        public async Task<IActionResult> Login(LoginRequestDto obj)
        {
            ResponseDto responseDto = await _authService.LoginAsync(obj);


            if (responseDto != null && responseDto.IsSuccess)
            {
                // Replace the problematic line in the Login method with the following:
                LoginResponseDto loginResponseDto =
                    JsonConvert.DeserializeObject<LoginResponseDto>(Convert.ToString(responseDto.Result));

                if (loginResponseDto != null && !string.IsNullOrEmpty(loginResponseDto.Token))
                {
                    //HttpContext.Session.SetString(SD.SessionToken, loginResponseDto.Token);
                    //HttpContext.Session.SetString(SD.SessionUserRole, loginResponseDto.Role);
                    await SignInUser(loginResponseDto);
                    _tokenProvider.SetToken(loginResponseDto.Token);
                    TempData["success"] = "Login successful";
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login response.");
                }

            }
            else
            {
               // ModelState.AddModelError(string.Empty, responseDto?.Message ?? "Login failed.");
                TempData["error"] = responseDto?.Message ?? "Login failed.";
                return View(obj);
            }

            var roleList = new List<SelectListItem>()
                {
                    new SelectListItem()
                    {
                        Text = SD.RoleAdmin,
                        Value = SD.RoleAdmin
                    },
                    new SelectListItem()
                    {
                        Text = SD.RoleCustomer,
                        Value = SD.RoleCustomer
                    }
                };
            ViewBag.RoleList = roleList;
            return View();

        }




      
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            _tokenProvider.ClearToken();
            TempData["success"] = "Logout successful";
            return RedirectToAction("Index", "Home"); 
        }

        private async Task SignInUser(LoginResponseDto loginResponseDto)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(loginResponseDto.Token);

            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
            identity.AddClaim(new Claim (JwtRegisteredClaimNames.Email ,
                jwt.Claims.FirstOrDefault(u=>u.Type == JwtRegisteredClaimNames.Sub).Value));
            identity.AddClaim(new Claim(JwtRegisteredClaimNames.Sub,
               jwt.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Name).Value));
            identity.AddClaim(new Claim(JwtRegisteredClaimNames.Name,
               jwt.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Email).Value));


            identity.AddClaim(new Claim(ClaimTypes.Name,
               jwt.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Email).Value));

            identity.AddClaim(new Claim(ClaimTypes.Role,
                jwt.Claims.FirstOrDefault(u => u.Type == "role").Value));

            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
               
        }
    }
}

