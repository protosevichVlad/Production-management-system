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
    }
}