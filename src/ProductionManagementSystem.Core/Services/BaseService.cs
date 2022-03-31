using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ProductionManagementSystem.Core.Repositories;

namespace ProductionManagementSystem.Core.Services
{
    public interface IBaseService<TItem> : IDisposable
    {
        public Task<List<TItem>> GetAllAsync();
        public Task<TItem> GetByIdAsync(int id);
        public Task<List<TItem>> Find(Func<TItem, bool> predicate);
        public Task CreateAsync(TItem item);
        public Task UpdateAsync(TItem item);
        public Task UpdateRangeAsync(List<TItem> items);
        public Task DeleteAsync(TItem item);
    }
    
    public abstract class BaseService<TItem, TIUnitOfWork> : IBaseService<TItem>
        where TIUnitOfWork : IBaseUnitOfWork
    {
        protected TIUnitOfWork _db;
        protected IRepository<TItem> _currentRepository;

        protected BaseService(TIUnitOfWork db)
        {
            _db = db;
        }

        public virtual async Task<List<TItem>> GetAllAsync()
        {
            return await _currentRepository.GetAllAsync();
        }

        public virtual async Task<TItem> GetByIdAsync(int id)
        {
            return await _currentRepository.GetByIdAsync(id);
        }

        public virtual async Task<List<TItem>> Find(Func<TItem, bool> predicate)
        {
            return await _currentRepository.FindAsync(predicate);
        }

        public virtual async Task CreateAsync(TItem item)
        {
            await _currentRepository.CreateAsync(item);
            await _db.SaveAsync();
        }

        public virtual async Task UpdateAsync(TItem item)
        {
            await _currentRepository.UpdateAsync(item);
            await _db.SaveAsync();
        }

        public virtual async Task UpdateRangeAsync(List<TItem> items)
        {
            await _currentRepository.UpdateRangeAsync(items);
            await _db.SaveAsync();
        }

        public virtual async Task DeleteAsync(TItem item)
        {
            _currentRepository.Delete(item);
            await _db.SaveAsync();
        }

        public void Dispose()
        {
            _db.Dispose();
        }
    }
}