using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using ProductionManagementSystem.Core.Models.Logs;
using ProductionManagementSystem.Core.Models.Users;
using ProductionManagementSystem.Core.Repositories;

namespace ProductionManagementSystem.Core.Services
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
            get => _user?.UserName;
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

        public override async Task<IEnumerable<Log>> GetAllAsync()
        {
            return (await base.GetAllAsync()).Reverse();
        }

        public override async Task<IEnumerable<Log>> Find(Func<Log, bool> predicate)
        {
            return (await base.Find(predicate)).Reverse();
        }
    }
}