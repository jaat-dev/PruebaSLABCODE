using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Projects.Api.Entities;
using Projects.Api.Enums;
using Projects.Api.Helpers;
using Projects.Api.Models;
using Projects.Api.Models.Requests;
using Projects.Api.Models.Responses;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Projects.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IUserHelper _userHelper;
        private readonly IConfiguration _configuration;
        private readonly IMailHelper _mailHelper;

        public AccountController(
            IUserHelper userHelper,
            IConfiguration configuration,
            IMailHelper mailHelper)
        {
            _userHelper = userHelper;
            _configuration = configuration;
            _mailHelper = mailHelper;
        }

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest login)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new Response
                {
                    IsSuccess = false,
                    Message = "Bad request.",
                    Result = ModelState
                });
            }

            User user = await _userHelper.GetUserAsyncByEmail(login.Username);
            if (user == null)
            {
                return NotFound(new Response { IsSuccess = false, Message = "User does not exist" });
            }

            if (!user.IsActive)
            {
                return BadRequest(new Response { IsSuccess = false, Message = "The user is not active" });
            }

            Microsoft.AspNetCore.Identity.SignInResult result = await _userHelper
                .ValidatePasswordAsync(user, login.Password);
            if (!result.Succeeded)
            {
                return BadRequest(new Response { IsSuccess = false, Message = "User/Password is Wrong" });
            }

            object results = GetToken(user);

            return Created(string.Empty, results);
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Authorize(Roles = "Administrador")]
        [Route("AddUser")]
        public async Task<IActionResult> PostUser([FromBody] UserRequest request)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            IList<Claim> claims = identity.Claims.ToList();
            var userName = claims[0].Value;
            User user = await _userHelper.GetUserAsyncByEmail(userName);
            if (user == null)
            {
                return BadRequest(new Response
                {
                    IsSuccess = false,
                    Message = "User does not exist"
                });
            }

            IList<string> roles = await _userHelper.GetUserRolAsync(user);
            if (!roles.Contains("Administrador"))
            {
                return NotFound(new Response
                {
                    IsSuccess = false,
                    Message = "You do not have permissions to create users"
                });
            }


            if (!ModelState.IsValid)
            {
                return BadRequest(new Response
                {
                    IsSuccess = false,
                    Message = "Bad request",
                    Result = ModelState
                });
            }

            user = await _userHelper.GetUserAsyncByEmail(request.Email);
            if (user != null)
            {
                return BadRequest(new Response
                {
                    IsSuccess = false,
                    Message = "A user with that email is already registered."
                });
            }

            user = new User
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                IsActive = true,
                UserName = request.Email,
                Email = request.Email,
                UserType = UserType.Operador
            };

            IdentityResult result = await _userHelper.AddUserAsync(user, request.Password);
            if (result != IdentityResult.Success)
            {
                return BadRequest(result.Errors.FirstOrDefault().Description);
            }

            User userNew = await _userHelper.GetUserAsyncByEmail(request.Email);
            await _userHelper.AddUserToRoleAsync(userNew, user.UserType.ToString());

            EmailMessageModel message = new()
            {
                To = request.Email,
                Subject = "Email Confirmation",
                Content = $"<h1>Email Confirmation</h1> " +
                          $"<h1>Below you will find the access data:</h1> " +
                          $"<h3>UserName: {request.Email}</h3> " +
                          $"<h3>Password: {request.Password}</h3> "

            };

            _mailHelper.SendMail(message);

            return Ok(new Response { IsSuccess = true, Message = "User created successfully", Result = userNew });
        }

        private object GetToken(User user)
        {
            Claim[] claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Role, user.UserType.ToString())
            };

            SymmetricSecurityKey key = new(Encoding.UTF8.GetBytes(_configuration["Tokens:Key"]));
            SigningCredentials credentials = new(key, SecurityAlgorithms.HmacSha256);
            JwtSecurityToken token = new(
                _configuration["Tokens:Issuer"],
                _configuration["Tokens:Audience"],
                claims,
                expires: DateTime.UtcNow.AddMinutes(120),
                signingCredentials: credentials);

            return new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token),
                expiration = token.ValidTo,
                user
            };
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost]
        [Route("ChangePassword")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new Response
                {
                    IsSuccess = false,
                    Message = "Bad request.",
                    Result = ModelState
                });
            }

            string email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
            User user = await _userHelper.GetUserAsyncByEmail(email);
            if (user == null)
            {
                return NotFound("User no found.");
            }

            IdentityResult result = await _userHelper.ChangePasswordAsync(user, request.OldPassword, request.NewPassword);
            if (!result.Succeeded)
            {
                return BadRequest(new Response
                {
                    IsSuccess = false,
                    Message = "The current password is incorrect."
                });
            }

            return Ok(new Response { IsSuccess = true, Message = "Password changed successfully" });
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost]
        [Authorize(Roles = "Administrador")]
        [Route("ActiveDeactiveUser")]
        public async Task<IActionResult> ActiveDeactiveUser([FromBody] ActiveDeactiveUserRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new Response
                {
                    IsSuccess = false,
                    Message = "Bad request.",
                    Result = ModelState
                });
            }

            User user = await _userHelper.GetUserAsyncByEmail(request.Email);
            if (user == null)
            {
                return NotFound(new Response { IsSuccess = false, Message = "User no found." });
            }

            user.IsActive = request.IsActive;
            IdentityResult result = await _userHelper.UpdateUserAsync(user);
            if (!result.Succeeded)
            {
                return BadRequest(new Response
                {
                    IsSuccess = false,
                    Message = "Error deactivating user."
                });
            }

            return Ok(new Response
            {
                IsSuccess = true,
                Message = request.IsActive ? "User activated successfully" : "User deactivated successfully"
            });
        }
    }
}
