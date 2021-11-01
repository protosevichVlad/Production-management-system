using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProductionManagementSystem.DAL.EF;

namespace ProductionManagementSystem.DAL.Repositories
{
    public interface IRepository<TItem>
    {
        public Task<IEnumerable<TItem>> GetAllAsync();
        public Task<TItem> GetByIdAsync(int id);
        public Task<IEnumerable<TItem>> FindAsync(Func<TItem, bool> predicate);
        public Task CreateAsync(TItem item);
        public Task UpdateAsync(TItem item);
        public void Delete(TItem item);
        public Task SaveAsync();
    }
    
    public class Repository<TItem> : IRepository<TItem> 
        where TItem : class
    {
        protected readonly ApplicationContext _db;
        protected readonly DbSet<TItem> _dbSet;

        protected Repository(ApplicationContext db)
        {
            _db = db;
            _dbSet = db.Set<TItem>();
        }

        public virtual async Task<IEnumerable<TItem>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public virtual async Task<TItem> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public virtual async Task<IEnumerable<TItem>> FindAsync(Func<TItem, bool> predicate)
        {
            return _dbSet.Where(predicate);
        }

        public virtual async Task CreateAsync(TItem item)
        {
            await _dbSet.AddAsync(item);
        }

        public virtual async Task UpdateAsync(TItem item)
        {
            _db.Entry<TItem>(item).State = EntityState.Modified;
        }

        public virtual void Delete(TItem item)
        {
            _dbSet.Remove(item);
        }

        public virtual async Task SaveAsync()
        {
            await _db.SaveChangesAsync();
        }
    }
}