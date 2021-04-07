using Microsoft.AspNetCore.Identity;
using Projects.Api.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Projects.Api.Helpers
{
    public interface IUserHelper
    {
        Task<IdentityResult> AddUserAsync(User user, string password);
        Task AddUserToRoleAsync(User user, string roleName);
        Task CheckRoleAsync(string roleName);
        Task<User> GetUserAsyncByEmail(string email);
        Task<SignInResult> ValidatePasswordAsync(User user, string password);
        Task<string> GenerateEmailConfirmationTokenAsync(User user);
        Task<User> GetUserAsyncById(string userId);
        Task<IdentityResult> ChangePasswordAsync(User user, string oldPassword, string newPassword);
        Task<IList<string>> GetUserRolAsync(User user);
        Task<IdentityResult> UpdateUserAsync(User user);
    }
}
