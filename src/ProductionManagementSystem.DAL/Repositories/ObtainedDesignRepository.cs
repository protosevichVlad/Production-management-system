using ProductionManagementSystem.DAL.EF;
using ProductionManagementSystem.Models.Tasks;

namespace ProductionManagementSystem.DAL.Repositories
{
    public interface IObtainedDesignRepository : IRepository<ObtainedDesign>
    {
        
    }
    public class ObtainedDesignRepository : Repository<ObtainedDesign>, IObtainedDesignRepository
    {
        public ObtainedDesignRepository(ApplicationContext db) : base(db)
        {
        }
    }
}