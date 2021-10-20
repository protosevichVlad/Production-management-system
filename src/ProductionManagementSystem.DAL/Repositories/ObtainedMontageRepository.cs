using ProductionManagementSystem.DAL.EF;
using ProductionManagementSystem.Models.Tasks;

namespace ProductionManagementSystem.DAL.Repositories
{
    public interface IObtainedMontageRepository : IRepository<ObtainedMontage>
    {
        
    }

    public class ObtainedMontageRepository : Repository<ObtainedMontage>, IObtainedMontageRepository
    {
        public ObtainedMontageRepository(ApplicationContext db) : base(db)
        {
        }
    }
}