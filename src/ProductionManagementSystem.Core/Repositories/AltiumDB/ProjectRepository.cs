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
        Task<List<Project>> SearchByKeyWordAsync(string keyWord);
    }
    
    public class ProjectRepository : Repository<Project>, IProjectRepository
    {
        public ProjectRepository(ApplicationContext db) : base(db)
        {
        }

        public async Task<List<Project>> GetProjectsWithEntityAsync(string partNumber)
        {
            var entity = await _db.Entities.FirstOrDefaultAsync(x => x.PartNumber == partNumber);
            if (entity == null) return new List<Project>();
            return await _dbSet.Include(x => x.Entities)
                .Where(x => x.Entities.Count(x => x.EntityId == entity.KeyId) > 0)
                .ToListAsync();
        }

        public async Task<List<Project>> SearchByKeyWordAsync(string keyWord)
        {
            if (string.IsNullOrEmpty(keyWord))
                return new List<Project>();
            return await _dbSet.Where(x => !string.IsNullOrEmpty(x.Name) && x.Name.Contains(keyWord)).ToListAsync();
        }
    }
}