using ProductionManagementSystem.Core.Data.EF;
using ProductionManagementSystem.Core.Models.AltiumDB;

namespace ProductionManagementSystem.Core.Repositories.AltiumDB
{
    public interface IEntityInProjectRepository : IRepository<EntityInProject>
    {
        
    }

    public class EntityInProjectRepository : Repository<EntityInProject>, IEntityInProjectRepository
    {
        public EntityInProjectRepository(ApplicationContext db) : base(db)
        {
        }
    }
}