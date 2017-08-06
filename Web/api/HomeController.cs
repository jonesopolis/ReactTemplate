using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Web.api
{
    [Route("")]
    public class HomeController : Controller
    {
        private readonly IConfigurationRoot _configuration;

        public HomeController(IConfigurationRoot configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }
    
        [HttpGet("")]
        public IActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                return File("~/app.html", "text/html");
            }

            if (Request.Cookies.ContainsKey("auth"))
            {

                var parameters = new TokenValidationParameters
                                 {
                                     IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("AppConfiguration:Key").Value)),
                                     ValidAudience = _configuration.GetSection("AppConfiguration:SiteUrl").Value,
                                     ValidateIssuerSigningKey = true,
                                     ValidateLifetime = true,
                                     ValidIssuer = _configuration.GetSection("AppConfiguration:SiteUrl").Value
                                 };

                try
                {
                    new JwtSecurityTokenHandler().ValidateToken(Request.Cookies["auth"], parameters, out SecurityToken token);
                    return File("~/app.html", "text/html");
                }
                catch (Exception ex)
                {
                   
                }
            }

            return File("~/login.html", "text/html");
        }
    }
}
