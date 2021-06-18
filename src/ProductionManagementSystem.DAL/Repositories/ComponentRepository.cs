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
    public class ComponentRepository : IRepository<Component>
    {
        private readonly ApplicationContext _db;

        public ComponentRepository(ApplicationContext context)
        {
            _db = context;
        }

        public async Task<IEnumerable<Component>> GetAllAsync()
        {
            return await _db.Components.ToListAsync();
        }

        public async Task<Component> GetAsync(int id)
        {
            return await _db.Components.FirstOrDefaultAsync(c => c.Id == id);
        }

        public IEnumerable<Component> Find(Func<Component, bool> predicate)
        {
            return _db.Components.Where(predicate).ToList();
        }

        public async Task CreateAsync(Component item)
        {
            await _db.Components.AddAsync(item);
        }

        public void Update(Component item)
        {
            _db.Entry(item).State = EntityState.Modified;
        }

        public async Task DeleteAsync(int id)
        {
            Component component = await _db.Components.FindAsync(id);
            if (component != null)
                _db.Components.Remove(component);
        }
    }
}