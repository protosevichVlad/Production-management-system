using Microsoft.AspNetCore.Identity;

namespace ProductionManagementSystem.Core.Models.Users
{
    public class User : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}