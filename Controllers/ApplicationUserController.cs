using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using UserWebApi.Models;

namespace UserWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApplicationUserController : ControllerBase
    {
        // Define two private properties
        private UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationSettings _appSettings;
        private readonly ILogger _logger;

        public ApplicationUserController(
            UserManager<ApplicationUser> userManager, 
            SignInManager<ApplicationUser> signInManager, 
            IOptions<ApplicationSettings> appSettings, 
            ILogger<ApplicationUserController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _appSettings = appSettings.Value;
            _logger = logger;
        }

        // Web Api method for Signing user Up
        [HttpPost]
        [Route("SignUp")]
        public async Task<Object> PostApplicationUser(ApplicationUserModel model)
        {
            var ApplicationUser = new ApplicationUser()
            {
                UserName = model.UserName,
                Email = model.Email,
                FullName = model.FullName
            };

            // Log information into a mylog-{Date} file
            _logger.LogInformation($"New user {model.UserName} is about to be registered!");

            var result = await _userManager.CreateAsync(ApplicationUser, model.Password);
            return Ok(result);
        }

        // Web Api method for Signing user In
        [HttpPost]
        [Route("SignIn")]
        public async Task<IActionResult> SignIn(SignInModel model)
        {
            _logger.LogInformation($"Finding user with username: {model.UserName}");
            var user = await _userManager.FindByNameAsync(model.UserName);

            if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
            {
                var TokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                        new Claim("UserID", user.Id.ToString())
                    }),
                    Expires = DateTime.Now.AddHours(1),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appSettings.JWT_Secret)), SecurityAlgorithms.HmacSha256)
                };
                var tokenHandler = new JwtSecurityTokenHandler();
                var securityToken = tokenHandler.CreateToken(TokenDescriptor);
                var ExpirationTime = TokenDescriptor.Expires;
                var token = tokenHandler.WriteToken(securityToken);

                // Log info about logged in user
                _logger.LogInformation($"User {model.UserName} is about to be Logged In!");
                return Ok(new { token, ExpirationTime});
            }
            else
            {
                _logger.LogError("Something went wrong while trying to Log User In!");
                return BadRequest(new { message = "Username or password is incorrect!" });
            }
        }
    }
}