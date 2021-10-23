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
        public void Update(TItem item);
        public void Delete(TItem item);
    }
    
    public class BaseService<TItem> : IBaseService<TItem>
    {
        protected IUnitOfWork _db;
        protected IRepository<TItem> _currentRepository;

        public BaseService(IUnitOfWork db)
        {
            _db = db;
            _currentRepository = _db.GetRepository<TItem>() ?? throw new NullReferenceException(nameof(_currentRepository));
        }

        public virtual IEnumerable<TItem> GetAll()
        {
            return _currentRepository.GetAll();
        }

        public virtual async Task<TItem> GetByIdAsync(int id)
        {
            return await _currentRepository.GetByIdAsync(id);
        }

        public virtual TItem GetById(int id)
        {
            return _currentRepository.GetById(id);
        }

        public virtual IEnumerable<TItem> Find(Func<TItem, bool> predicate)
        {
            return _currentRepository.Find(predicate);
        }

        public virtual async Task CreateAsync(TItem item)
        {
            await _currentRepository.CreateAsync(item);
        }

        public virtual void Create(TItem item)
        {
            _currentRepository.Create(item);
            _db.Save();
        }

        public virtual void Update(TItem item)
        {
            _currentRepository.Update(item);
            _db.Save();
        }

        public virtual void Delete(TItem item)
        {
            _currentRepository.Delete(item);
            _db.Save();
        }
    }
}