using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using ProductionManagementSystem.DAL.Repositories;
using ProductionManagementSystem.Models.Logs;
using ProductionManagementSystem.Models.Users;

namespace ProductionManagementSystem.BLL.Services
{
    public interface ILogService : IBaseService<Log>
    {
        string CurrentUserName { get; set; }
    }
    
    public class LogService : BaseService<Log>, ILogService
    {
        private readonly UserManager<User> _userManager;
        private User _user;
     
        public string CurrentUserName
        {
            get => _user.UserName;
            set
            {
                _user = _userManager.FindByNameAsync(value).Result;
                ((ILogRepository) _currentRepository).CurrentUser = _user;
            }
        }

        public LogService(IUnitOfWork uow) : base(uow)
        {
            _currentRepository = _db.LogRepository;
        }
        
        public LogService(IUnitOfWork uow, UserManager<User> userManager) : base(uow)
        {
            _currentRepository = _db.LogRepository;
            _userManager = userManager;
        }

        public override Task CreateAsync(Log item)
        {
            item.UserId = _user.Id;
            return base.CreateAsync(item);
        }
    }
}