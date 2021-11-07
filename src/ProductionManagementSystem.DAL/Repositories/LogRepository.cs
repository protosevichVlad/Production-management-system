using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProductionManagementSystem.DAL.EF;
using ProductionManagementSystem.Models.Logs;
using ProductionManagementSystem.Models.Users;

namespace ProductionManagementSystem.DAL.Repositories
{
    public interface ILogRepository : IRepository<Log>
    {
        User CurrentUser { get; set; }
    }
    public class LogRepository : Repository<Log>, ILogRepository
    {
        public LogRepository(ApplicationContext db) : base(db)
        {
        }

        public User CurrentUser { get; set; }

        public override Task CreateAsync(Log item)
        {
            item.UserId = CurrentUser.Id;
            return base.CreateAsync(item);
        }

        public override async Task<IEnumerable<Log>> GetAllAsync()
        {
            var logs = (await base.GetAllAsync()).ToList();
            foreach (var log in logs)
                log.User = await _db.Users.FindAsync(log.UserId);

            return logs;
        }
        
        public override async Task<Log> GetByIdAsync(int id)
        {
            var log = await base.GetByIdAsync(id);
            log.User = await _db.Users.FindAsync(log.UserId);
            return log;
        }
        
        public override async Task<IEnumerable<Log>> FindAsync(Func<Log, bool> predicate)
        {
            var logs = (await base.FindAsync(predicate)).ToList();
            foreach (var log in logs)
                log.User = await _db.Users.FindAsync(log.UserId);

            return logs;
        }
    }
}