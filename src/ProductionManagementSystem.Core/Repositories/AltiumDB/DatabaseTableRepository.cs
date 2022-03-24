using ProductionManagementSystem.Core.Data.EF;
using ProductionManagementSystem.Core.Models.AltiumDB;

namespace ProductionManagementSystem.Core.Repositories.AltiumDB
{
    public interface IDatabaseTableRepository : IRepository<DatabaseTable>
    {
        
    }
    
    public class DatabaseTableRepository : Repository<DatabaseTable>, IDatabaseTableRepository
    {
        public DatabaseTableRepository(ApplicationContext db) : base(db)
        {
        }
    }
}