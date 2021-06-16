using JWTAuthentication_with_Refresh_Token.Abstract;
using JWTAuthentication_with_Refresh_Token.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;

namespace JWTAuthentication_with_Refresh_Token.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class LoginController : Controller
    {
        private readonly IUserService _userService;
        private readonly IJWTService _JWTService;

        public LoginController(IUserService userService, IJWTService JWTService)
        {
            _userService = userService;
            _JWTService = JWTService;
        }

        [HttpPost]
        public IActionResult Login([FromBody] User user)
        {
            IActionResult response = Unauthorized();
            var _user = _userService.Login(user);

            //we can add claim in three different ways
            if (user != null)
            {
                var claims = new[] {
                new Claim(ClaimTypes.NameIdentifier, "ilyas"),
                new Claim(ClaimTypes.Name, "ilyas"),
                new Claim(JwtRegisteredClaimNames.Email, "ilyas.varol@outlook.com.tr"),
                new Claim("Surname", "Varol"),
                };

                var JWTToken = _JWTService.GenerateJSONWebToken(claims);
                response = Ok(JWTToken);
            }

            return response;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return Ok();
        }

        //Step 3: Testing Authorize
        [HttpGet]
        [Authorize]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] { "value1", "value2", "value3", "value4", "value5" };
        }

        [HttpGet]
        [Authorize]
        public string HandleToken()
        {
            var currentUser = HttpContext.User;

            if (currentUser.HasClaim(c => c.Type == "Surname"))
            {
                var mail = currentUser.Claims.FirstOrDefault(c => c.Type == "Surname").Value;
                return mail;
            }
            else
            {
                return "Something went wrong!";
            }
        }

        [HttpPost]
        public IActionResult RefreshToken([FromBody] RefreshTokenRequest request)
        {
            var token = _JWTService.RefreshJSONWebToken(request);
            return Ok(token);
        }
    }
}
