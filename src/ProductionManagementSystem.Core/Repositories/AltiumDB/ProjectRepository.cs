using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProductionManagementSystem.Core.Data.EF;
using ProductionManagementSystem.Core.Models.AltiumDB.Projects;

namespace ProductionManagementSystem.Core.Repositories.AltiumDB
{
    public interface IProjectRepository : IRepository<Project>
    {
        Task<List<Project>> GetProjectsWithEntityAsync(string partNumber);
    }
    
    public class ProjectRepository : Repository<Project>, IProjectRepository
    {
        public ProjectRepository(ApplicationContext db) : base(db)
        {
        }

        public async Task<List<Project>> GetProjectsWithEntityAsync(string partNumber)
        {
            return await _dbSet.Include(x => x.Entities).Where(x => x.Entities.Count(x => x.PartNumber == partNumber) > 0)
                .ToListAsync();
        }
    }
}