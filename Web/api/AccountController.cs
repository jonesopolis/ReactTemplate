using System;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Db;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Web.api
{
    [Route("api/[controller]")]
    public class AccountController : Controller
    {
        private readonly IConfigurationRoot _configuration;
        private readonly UserManager<User> _userManager;

        public AccountController(UserManager<User> userManager, IConfigurationRoot configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        }

        [HttpPost("login")]
        public async Task<IActionResult> SignInAsync([FromBody]SignInViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                return Ok(new { Errors= ViewData.ModelState.Values.SelectMany(m => m.Errors).Select(e => e.ErrorMessage) });
            }

            var user = await _userManager.FindByNameAsync(vm.Username);

            if (user == null || !await _userManager.CheckPasswordAsync(user, vm.Password))
            {
                return Ok(new { Errors = new [] { "Username or password invalid" }});
            }

            var claims = await _userManager.GetClaimsAsync(user);

            var token =  new JwtSecurityToken(
                issuer: _configuration.GetSection("AppConfiguration:SiteUrl").Value,
                audience: _configuration.GetSection("AppConfiguration:SiteUrl").Value,
                claims: claims,
                expires: DateTime.Now.AddHours(10),
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("AppConfiguration:Key").Value)), SecurityAlgorithms.HmacSha256)
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            return Ok(new
                      {
                          token = tokenString,
                          expiration = token.ValidTo
                      });
        }

        [HttpPost("signup")]
        public async Task<IActionResult> SignUpAsync([FromBody]SignInViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                return Ok(new { Errors = ViewData.ModelState.Values.SelectMany(m => m.Errors).Select(e => e.ErrorMessage) });
            }

            var user = new User { UserName = vm.Username, };

            var result = await _userManager.CreateAsync(user, vm.Password);

            if (result != IdentityResult.Success)
            {
                return Ok(new { Errors = result.Errors.Select(e => e.Description) });
            }

            //await _userManager.AddClaimAsync(user, new Claim(ClaimTypes.Email, email));

            return RedirectToAction("SignInAsync", new {vm});
        }

        [HttpGet("secret")]
        [Authorize]
        public IActionResult SuperSecret()
        {
            return Ok("SECRET SCRET");
        }
    }

    public sealed class SignInViewModel
    {
        [Required(AllowEmptyStrings = false)]
        public string Username { get; set; }
        [Required(AllowEmptyStrings = false)]
        public string Password { get; set; }
    }
}
