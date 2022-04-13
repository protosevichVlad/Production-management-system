using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProductionManagementSystem.Core.Data.EF;
using ProductionManagementSystem.Core.Models.PCB;

namespace ProductionManagementSystem.Core.Repositories.AltiumDB
{
    public interface IPcbRepository : IRepository<Pcb>
    {
        Task<List<Pcb>> GetProjectsWithEntityAsync(string partNumber);
        Task<List<Pcb>> SearchByKeyWordAsync(string keyWord);
    }
    
    public class PcbRepository : Repository<Pcb>, IPcbRepository
    {
        public PcbRepository(ApplicationContext db) : base(db)
        {
        }

        public async Task<List<Pcb>> GetProjectsWithEntityAsync(string partNumber)
        {
            var entity = await _db.Entities.FirstOrDefaultAsync(x => x.PartNumber == partNumber);
            if (entity == null) return new List<Pcb>();
            return await _dbSet.Include(x => x.Entities)
                .Where(x => x.Entities.Count(x => x.EntityId == entity.KeyId) > 0)
                .ToListAsync();
        }

        public async Task<List<Pcb>> SearchByKeyWordAsync(string keyWord)
        {
            if (string.IsNullOrEmpty(keyWord))
                return new List<Pcb>();
            return await _dbSet.Where(x => !string.IsNullOrEmpty(x.Name) && x.Name.Contains(keyWord)).ToListAsync();
        }

        public override async Task UpdateAsync(Pcb item)
        {
            _db.Update(item);
        }
    }
}