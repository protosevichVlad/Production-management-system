using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProductionManagementSystem.DAL.Interfaces
{
    public interface IRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> GetAsync(int id);
        IEnumerable<T> Find(Func<T, Boolean> predicate);
        Task CreateAsync(T item);
        void Update(T item);
        Task DeleteAsync(int id);
    }
}