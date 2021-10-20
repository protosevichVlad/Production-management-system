using ProductionManagementSystem.DAL.EF;
using ProductionManagementSystem.Models.Logs;

namespace ProductionManagementSystem.DAL.Repositories
{
    public interface ILogRepository : IRepository<Log>
    {
        
    }
    public class LogRepository : Repository<Log>, ILogRepository
    {
        public LogRepository(ApplicationContext db) : base(db)
        {
        }
    }
}