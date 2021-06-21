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
    public class DesignRepository : IRepository<Design>
    {
        private readonly ApplicationContext _db;

        public DesignRepository(ApplicationContext context)
        {
            _db = context;
        }

        public async Task<IEnumerable<Design>> GetAllAsync()
        {
            return await _db.Designs.ToListAsync();
        }

        public async Task<Design> GetAsync(int id)
        {
            return await _db.Designs.FindAsync(id);
        }

        public IEnumerable<Design> Find(Func<Design, bool> predicate)
        {
            return _db.Designs.Where(predicate).ToList();
        }

        public async Task CreateAsync(Design item)
        {
            await _db.Designs.AddAsync(item);
        }

        public void Update(Design item)
        {
            _db.Entry(item).State = EntityState.Modified;
        }

        public async Task DeleteAsync(int id)
        {
            var item = await _db.Designs.FindAsync(id);
            if (item != null)
                _db.Designs.Remove(item);
        }
    }
}