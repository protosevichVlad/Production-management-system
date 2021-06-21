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
    public class LogRepository : IRepository<Log>
    {
        private readonly ApplicationContext _db;

        public LogRepository(ApplicationContext context)
        {
            _db = context;
        }

        public async Task<IEnumerable<Log>> GetAllAsync()
        {
            return await _db.Logs.ToListAsync();
        }

        public async Task<Log> GetAsync(int id)
        {
            return await _db.Logs.FindAsync(id);
        }

        public IEnumerable<Log> Find(Func<Log, bool> predicate)
        {
            return _db.Logs.Where(predicate).ToList();
        }

        public async Task CreateAsync(Log item)
        {
            await _db.Logs.AddAsync(item);
        }

        public void Update(Log item)
        {
            _db.Entry(item).State = EntityState.Modified;
        }

        public async Task DeleteAsync(int id)
        {
            var item = await _db.Logs.FindAsync(id);
            if (item != null)
                _db.Logs.Remove(item);
        }
    }
}