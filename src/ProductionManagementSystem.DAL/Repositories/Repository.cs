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
        public IEnumerable<TItem> GetAll();
        public Task<TItem> GetByIdAsync(int id);
        public TItem GetById(int id);
        public IEnumerable<TItem> Find(Func<TItem, bool> predicate);
        public Task CreateAsync(TItem item);
        public void Create(TItem item);
        public Task UpdateAsync(TItem item);
        public void Update(TItem item);
        public void Delete(TItem item);
    }
    
    public class Repository<TItem> : IRepository<TItem> 
        where TItem : class
    {
        protected readonly ApplicationContext _db;
        protected readonly DbSet<TItem> _dbSet;

        public Repository(ApplicationContext db)
        {
            _db = db;
            _dbSet = db.Set<TItem>();
        }

        public IEnumerable<TItem> GetAll()
        {
            return _dbSet;
        }

        public async Task<TItem> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public TItem GetById(int id)
        {
            return _dbSet.Find(id);
        }

        public IEnumerable<TItem> Find(Func<TItem, bool> predicate)
        {
            return _dbSet.Where(predicate);
        }

        public async Task CreateAsync(TItem item)
        {
            await _dbSet.AddAsync(item);
            await _db.SaveChangesAsync();
        }

        public void Create(TItem item)
        {
            _dbSet.Add(item);
            _db.SaveChanges();
        }

        public async Task UpdateAsync(TItem item)
        {
            _db.Entry<TItem>(item).State = EntityState.Modified;
            await _db.SaveChangesAsync();
        }

        public void Update(TItem item)
        {
            _db.Entry<TItem>(item).State = EntityState.Modified;
            _db.SaveChanges();
        }

        public void Delete(TItem item)
        {
            _dbSet.Remove(item);
        }
    }
}