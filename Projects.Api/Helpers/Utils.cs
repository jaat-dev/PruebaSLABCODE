using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Projects.Api.Data;
using Projects.Api.Entities;
using Projects.Api.Models;
using Projects.Api.Models.Responses;

namespace Projects.Api.Helpers
{
    public class Utils
    {
        private readonly DataContext _context;
        private readonly UserManager<User> _userManager;

        public Utils()
        {
        }

        public Utils(
            DataContext context,
            UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public Response ValidateUrlParam(string userId, string token)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
            {
                return new Response { IsSuccess = false, Message = "Error 404, NOT FOUND!" };
            }

            User user = _context.Users.Find(userId);
            if (user == null)
            {
                return new Response { IsSuccess = false, Message = "Error 404, NOT FOUND!" };
            }

            var result = _userManager.ConfirmEmailAsync(user, token);
            if (!result.Result.Succeeded)
            {
                return new Response { IsSuccess = false, Message = "Error 404, NOT FOUND!" };
            }

            return new Response { IsSuccess = true, Message = "Email confirmed successfully!" };
        }
    }
}
