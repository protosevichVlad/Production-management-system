using ProductionManagementSystem.DAL.EF;
using ProductionManagementSystem.Models.Components;

namespace ProductionManagementSystem.DAL.Repositories
{
    public interface IDesignRepository : IComponentBaseRepository<Design>
    {
        
    }
    public class DesignRepository : ComponentBaseRepository<Design>, IDesignRepository
    {
        public DesignRepository(ApplicationContext db) : base(db)
        {
        }
    }
}