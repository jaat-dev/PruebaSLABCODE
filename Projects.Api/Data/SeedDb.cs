using Projects.Api.Entities;
using Projects.Api.Enums;
using Projects.Api.Helpers;
using System;
using System.Threading.Tasks;

namespace Projects.Api.Data
{
    public class SeedDb
    {
        private readonly DataContext _context;
        private readonly IUserHelper _userHelper;

        public SeedDb(
            DataContext context,
            IUserHelper userHelper)
        {

            _context = context;
            _userHelper = userHelper;
        }

        public async Task SeedAsync()
        {
            await _context.Database.EnsureCreatedAsync();
            await CheckRolesAsync();
            await CheckUserAsync("John","Arévalo", "john_arevalo@hotmail.com", UserType.Administrador);
            await _context.SaveChangesAsync();
        }

        private async Task<User> CheckUserAsync(
            string firstName,
            string lastName,
            string email,
            UserType userType)
        {
            User user = await _userHelper.GetUserAsyncByEmail(email);
            if (user == null)
            {
                user = new User
                {
                    FirstName = firstName,
                    LastName = lastName,
                    IsActive = true,
                    Email = email,
                    UserName = email,
                    UserType = userType
                };

                await _userHelper.AddUserAsync(user, "123456");
                await _userHelper.AddUserToRoleAsync(user, userType.ToString());
            }

            return user;
        }

        private async Task CheckRolesAsync()
        {
            await _userHelper.CheckRoleAsync(UserType.Administrador.ToString());
            await _userHelper.CheckRoleAsync(UserType.Operador.ToString());
        }
    }
}
