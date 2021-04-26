using EventFully.Models;
using EventFully.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace EventFully.API.V1.Controllers
{
    /// <summary>
    /// Represents a RESTful service of events.
    /// </summary>
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]/[action]")]
    public class AuthController : ControllerBase
    {
        private readonly JWTSecurityToken _jwtSettings;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;
        /// <summary>
        /// Auth Controller
        /// </summary>
        public AuthController
            (
            IOptions<JWTSecurityToken> jwtSettings,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IConfiguration configuration,
            IEmailService emailService
            )
        {
            _jwtSettings = jwtSettings.Value;
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _emailService = emailService;
        }

        /// <summary>
        /// Gets a auth token.
        /// </summary>
        /// <returns>The requested token.</returns>
        /// <response code="200">The token was successfully retrieved.</response>
        /// <response code="404">The token does not exist.</response>
        [HttpPost]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> CreateToken([FromBody] LoginModel loginModel)
        {
            if (ModelState.IsValid)
            {
                var loginResult = await _signInManager.PasswordSignInAsync(loginModel.Username, loginModel.Password, isPersistent: false, lockoutOnFailure: false);

                if (!loginResult.Succeeded)
                {
                    return BadRequest();
                }

                var user = await _userManager.FindByNameAsync(loginModel.Username);

                return Ok(GetToken(user));
            }
            return BadRequest(ModelState);

        }

        //[Authorize]
        [HttpPost]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> RefreshToken()
        {
            var user = await _userManager.FindByNameAsync(
                User.Identity.Name ??
                User.Claims.Where(c => c.Properties.ContainsKey("unique_name")).Select(c => c.Value).FirstOrDefault()
                );
            return Ok(GetToken(user));

        }
        //[Authorize]
        [HttpGet]
        [ProducesResponseType(404)]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> GetAccessToken()
        {
            // authentication successful so generate jwt token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSettings.Key);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                        new Claim(ClaimTypes.Name, "eventfullyapp")
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = _jwtSettings.Issuer,
                Audience = _jwtSettings.Audience
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return Ok(new { token = tokenHandler.WriteToken(token) });
        }

        /// <summary>
        /// Registers a user
        /// </summary>
        /// <returns>The requested token.</returns>
        /// <response code="200">The token was successfully retrieved.</response>
        /// <response code="404">The token does not exist.</response>
        [HttpPost]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> Register([FromBody] RegisterModel registerModel)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    //TODO: Use Automapper instaed of manual binding

                    UserName = registerModel.UserName,
                    FirstName = registerModel.FirstName,
                    LastName = registerModel.LastName,
                    Email = registerModel.UserName
                };

                var identityResult = await _userManager.CreateAsync(user, registerModel.Password);
                if (identityResult.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return Ok(GetToken(user));
                }
                else
                {
                    return Ok(identityResult.Errors);
                }
            }
            return Ok(ModelState.Values.SelectMany(v => v.Errors).Select(modelError => modelError.ErrorMessage).ToList());
            //throw new Exception("There is an issue with the information you have entered. Please try again.");
        }

        /// <summary>
        /// Sends Password Reset Email
        /// </summary>
        /// <returns>Success or Failure</returns>
        /// <response code="200">The email was successfully sent.</response>
        /// <response code="404">The email was not successfully sent.</response>
        [HttpPost]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Username);
                if (user == null) // || !(await _userManager.IsEmailConfirmedAsync(user)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return BadRequest();
                }

                // For more information on how to enable account confirmation and password reset please 
                // visit https://go.microsoft.com/fwlink/?LinkID=532713
                var code = await _userManager.GeneratePasswordResetTokenAsync(user);

                //var callbackUrl = Url.Action(
                //        "ResetPassword",
                //        "Account",
                //        new { code = code },
                //        Url.ActionContext.HttpContext.Request.Scheme
                //        );
                var callbackUrl = $"https://admin.eventbx.com/account/resetpassword?code={code}";
                await _emailService.SendPasswordResetAsync(
                    model.Username,
                    "Reset Password",
                    HtmlEncoder.Default.Encode(callbackUrl));

                return Ok();
            }

            return BadRequest();
        }

        /// <summary>
        /// Gets a auth token.
        /// </summary>
        /// <returns>The requested token.</returns>
        /// <response code="200">The token was successfully retrieved.</response>
        /// <response code="404">The token does not exist.</response>
        private UserToken GetToken(IdentityUser user)
        {
            var utcNow = DateTime.UtcNow;

            var claims = new Claim[]
            {
                        new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                        new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        new Claim(JwtRegisteredClaimNames.Iat, utcNow.ToString())
            };

            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
            
            var signingCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
            var jwt = new JwtSecurityToken(
                signingCredentials: signingCredentials,
                claims: claims,
                notBefore: utcNow,
                expires: DateTime.UtcNow.AddDays(7),
                audience: _jwtSettings.Audience,
                issuer: _jwtSettings.Issuer
                );

            return new UserToken(){UserId = user.Id, Token= new JwtSecurityTokenHandler().WriteToken(jwt), TokenExpiration = jwt.ValidTo };

        }
    }
}
