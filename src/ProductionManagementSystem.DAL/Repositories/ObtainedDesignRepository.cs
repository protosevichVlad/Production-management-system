using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProductionManagementSystem.DAL.EF;
using ProductionManagementSystem.DAL.Entities;
using ProductionManagementSystem.DAL.Interfaces;
using Task = System.Threading.Tasks.Task;

namespace ProductionManagementSystem.DAL.Repositories
{
    public class ObtainedDesignRepository : IRepository<ObtainedDesign>
    {
        private readonly ApplicationContext _db;

        public ObtainedDesignRepository(ApplicationContext context)
        {
            _db = context;
        }

        public async Task<IEnumerable<ObtainedDesign>> GetAllAsync()
        {
            return await _db.ObtainedDesigns
                .AsNoTracking()
                .Include(c => c.Task)
                .Include(c => c.Design)
                .ToListAsync();
        }

        public async Task<ObtainedDesign> GetAsync(int id)
        {
            return await _db.ObtainedDesigns
                .AsNoTracking()
                .Include(c => c.Task)
                .Include(c => c.Design)
                .FirstOrDefaultAsync(d => d.Id == id);
        }

        public IEnumerable<ObtainedDesign> Find(Func<ObtainedDesign, bool> predicate)
        {
            return _db.ObtainedDesigns
                .Include(c => c.Task)
                .Include(c => c.Design)
                .Where(predicate)
                .ToList();
        }

        public async Task CreateAsync(ObtainedDesign item)
        {
            await _db.ObtainedDesigns.AddAsync(item);
        }

        public void Update(ObtainedDesign item)
        {
            var obtDes = _db.ObtainedDesigns.FirstOrDefault(obtainedDesign => obtainedDesign.Id == item.Id);
            if (obtDes == null) return;
            obtDes.Obtained = item.Obtained;
            _db.SaveChanges();
        }

        public async Task DeleteAsync(int id)
        {
            var item = await _db.ObtainedDesigns.FindAsync(id);
            if (item != null)
                _db.ObtainedDesigns.Remove(item);
        }
    }
}