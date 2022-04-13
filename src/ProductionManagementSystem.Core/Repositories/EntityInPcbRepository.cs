using ProductionManagementSystem.Core.Data.EF;
using ProductionManagementSystem.Core.Models.PCB;

namespace ProductionManagementSystem.Core.Repositories
{
    public interface IEntityInPcbRepository : IRepository<EntityInPcb>
    {
        
    }

    public class EntityInPcbRepository : Repository<EntityInPcb>, IEntityInPcbRepository
    {
        public EntityInPcbRepository(ApplicationContext db) : base(db)
        {
        }
    }
}