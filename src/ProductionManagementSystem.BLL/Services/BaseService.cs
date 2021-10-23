using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ProductionManagementSystem.DAL.Repositories;

namespace ProductionManagementSystem.BLL.Services
{
    public interface IBaseService<TItem>
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
    
    public class BaseService<TItem> : IBaseService<TItem>
    {
        protected IRepository<TItem> _baseRepository;

        public virtual IEnumerable<TItem> GetAll()
        {
            return _baseRepository.GetAll();
        }

        public virtual async Task<TItem> GetByIdAsync(int id)
        {
            return await _baseRepository.GetByIdAsync(id);
        }

        public virtual TItem GetById(int id)
        {
            return _baseRepository.GetById(id);
        }

        public virtual IEnumerable<TItem> Find(Func<TItem, bool> predicate)
        {
            return _baseRepository.Find(predicate);
        }

        public virtual async Task CreateAsync(TItem item)
        {
            await _baseRepository.CreateAsync(item);
        }

        public virtual void Create(TItem item)
        {
            _baseRepository.Create(item);
        }

        public virtual async Task UpdateAsync(TItem item)
        {
            await _baseRepository.UpdateAsync(item);
        }

        public virtual void Update(TItem item)
        {
            _baseRepository.Update(item);
        }

        public virtual void Delete(TItem item)
        {
            _baseRepository.Delete(item);
        }
    }
}