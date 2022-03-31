using ProductionManagementSystem.Core.Data.EF;
using ProductionManagementSystem.Core.Models.AltiumDB.Projects;

namespace ProductionManagementSystem.Core.Repositories.AltiumDB
{
    public interface IProjectRepository : IRepository<Project>
    {
        
    }
    
    public class ProjectRepository : Repository<Project>, IProjectRepository
    {
        public ProjectRepository(ApplicationContext db) : base(db)
        {
        }
    }
}