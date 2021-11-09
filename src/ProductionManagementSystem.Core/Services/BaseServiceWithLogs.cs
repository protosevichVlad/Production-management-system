using System.Threading.Tasks;
using ProductionManagementSystem.Core.Repositories;

namespace ProductionManagementSystem.Core.Services
{
    public abstract class BaseServiceWithLogs<TItem> : BaseService<TItem>, IBaseService<TItem>
    {
        protected BaseServiceWithLogs(IUnitOfWork db) : base(db)
        {
            _db = db;
        }

        public new virtual async Task CreateAsync(TItem item)
        {
            await _currentRepository.CreateAsync(item);
            await CreateLogForCreatingAsync(item);
            await _db.SaveAsync();
        }

        public new virtual async Task UpdateAsync(TItem item)
        {
            await _currentRepository.UpdateAsync(item);
            await CreateLogForUpdatingAsync(item);
            await _db.SaveAsync();
        }

        public new virtual async Task DeleteAsync(TItem item)
        {
            _currentRepository.Delete(item);
            await CreateLogForDeletingAsync(item);
            await _db.SaveAsync();
        }

        protected abstract Task CreateLogForCreatingAsync(TItem item);
        protected abstract Task CreateLogForUpdatingAsync(TItem item);
        protected abstract Task CreateLogForDeletingAsync(TItem item);
    }
}