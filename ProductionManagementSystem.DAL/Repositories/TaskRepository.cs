using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using ProductionManagementSystem.DAL.EF;
using ProductionManagementSystem.DAL.Entities;
using ProductionManagementSystem.DAL.Interfaces;

namespace ProductionManagementSystem.DAL.Repositories
{
    public class TaskRepository : IRepository<Task>
    {
        private ApplicationContext _db;

        public TaskRepository(ApplicationContext context)
        {
            _db = context;
        }

        public async System.Threading.Tasks.Task<IEnumerable<Task>> GetAllAsync()
        {
            return await _db.Tasks
                .Include(t => t.Device)
                .Include(t => t.ObtainedDesigns)
                .ThenInclude(t => t.Design)
                .Include(t => t.ObtainedComponents)
                .ThenInclude(t => t.Component)
                .ToListAsync();
        }

        public async System.Threading.Tasks.Task<Task> GetAsync(int id)
        {
            return await _db.Tasks
                .Include(t => t.Device)
                .Include(t => t.ObtainedDesigns)
                .ThenInclude(t => t.Design)
                .Include(t => t.ObtainedComponents)
                .ThenInclude(t => t.Component)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public IEnumerable<Task> Find(Func<Task, bool> predicate)
        {
            return _db.Tasks
                .Include(t => t.Device)
                .Include(t => t.ObtainedDesigns)
                .ThenInclude(t => t.Design)
                .Include(t => t.ObtainedComponents)
                .ThenInclude(t => t.Component)
                .Where(predicate).ToList();
        }

        public async System.Threading.Tasks.Task CreateAsync(Task item)
        {
            await _db.Tasks.AddAsync(item);
        }

        public void Update(Task item)
        {
            // _db.Entry(item).State = EntityState.Modified;
            _db.Tasks.Update(item);
        }

        public async System.Threading.Tasks.Task DeleteAsync(int id)
        {
            var item = await _db.Tasks.FindAsync(id);
            if (item != null)
                _db.Tasks.Remove(item);
        }
    }
}