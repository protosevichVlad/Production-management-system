using ProductionManagementSystem.Core.Models.Users;

namespace ProductionManagementSystem.Core.Services
{
    public interface IUserService
    {
        public User User { get; set; }
    }

    public class UserService : IUserService
    {
        public User User { get; set; }
    }
}