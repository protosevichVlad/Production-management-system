using ProductionManagementSystem.Core.Data.EF;
using ProductionManagementSystem.Core.Models.Components;

namespace ProductionManagementSystem.Core.Repositories
{
    public interface IDesignRepository : IRepository<Design>
    {
        
    }
    public class DesignRepository : Repository<Design>, IDesignRepository
    {
        public DesignRepository(ApplicationContext db) : base(db)
        {
        }
    }
}