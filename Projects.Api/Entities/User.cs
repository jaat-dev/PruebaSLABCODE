using Microsoft.AspNetCore.Identity;
using Projects.Api.Enums;
using System.ComponentModel.DataAnnotations;

namespace Projects.Api.Entities
{
    public class User : IdentityUser
    {
        [MaxLength(50)]
        [Required]
        public string FirstName { get; set; }

        [MaxLength(50)]
        [Required]
        public string LastName { get; set; }

        public bool IsActive { get; set; }

        public UserType UserType { get; set; }
    }
}
