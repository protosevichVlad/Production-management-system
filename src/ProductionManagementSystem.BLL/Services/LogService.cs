using ProductionManagementSystem.DAL.Repositories;
using ProductionManagementSystem.Models.Logs;

namespace ProductionManagementSystem.BLL.Services
{
    public interface ILogService : IBaseService<Log>
    {
        
    }
    
    public class LogService : BaseService<Log>, ILogService
    {
        public LogService(IUnitOfWork uow) : base(uow)
        {
            
        }
    }
}