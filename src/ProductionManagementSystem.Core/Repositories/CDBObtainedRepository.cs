using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProductionManagementSystem.Core.Data.EF;
using ProductionManagementSystem.Core.Models;

namespace ProductionManagementSystem.Core.Repositories
{
    public interface ICDBObtainedRepository : IRepository<CDBObtained>
    {
        
    }

    public class CDBObtainedRepository : Repository<CDBObtained>, ICDBObtainedRepository
    {
        private readonly IEntityExtRepository _entityExtRepository;

        public CDBObtainedRepository(ApplicationContext db) : base(db)
        {
            _entityExtRepository = new EntityExtRepository(db);
        }

        public override async Task<CDBObtained> GetByIdAsync(int id)
        {
            return await _dbSet
                .Include(x => x.UsedItem)
                .Include(x => x.Task)
                .FirstOrDefaultAsync(x => x.Id == id);

        }

        public override async Task<List<CDBObtained>> GetAllAsync()
        {
            return await _dbSet.Include(x => x.UsedItem).Include(x => x.Task).ToListAsync();
        }
    }
}