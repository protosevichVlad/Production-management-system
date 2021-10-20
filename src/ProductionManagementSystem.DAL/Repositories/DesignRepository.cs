using ProductionManagementSystem.DAL.EF;
using ProductionManagementSystem.Models.Components;

namespace ProductionManagementSystem.DAL.Repositories
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